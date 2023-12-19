module Day14
open System
open System.Security.Cryptography
open Utils
open AoC2023.Inputs.Day14

let incrementRowCol rowLength row col (inc : int -> int) =    
    let nextIdx = inc (row * rowLength + col) 
    let nextRow = nextIdx / rowLength
    let nextCol = nextIdx % rowLength
    nextRow,nextCol

let debugPrint (input : string array array) (row : int) (col : int) (newRow : int) =
    if newRow <> row then
        printfn $"Moved rock from %d{row},%d{col} to %d{newRow},%d{col}"
        input |> Array.map (String.concat "") |> Array.iter (printfn "%s")
        printfn ""
        printfn ""
        
let printCycle (cycle : int) (input : string array array) =
    printfn $"Cycle %d{cycle}"
    input |> Array.map (String.concat "") |> Array.iter (printfn "%s")
    printfn ""
    printfn ""
    input

let rec findRollDownIdx (chars : string array) idx =
    if idx = 0 then
        0
    else if chars[idx - 1] = "." then
        findRollDownIdx chars (idx - 1)
    else
        idx

let rec findRollUpIdx (chars : string array) idx =
    if idx = chars.Length - 1 then
        idx
    else if chars[idx + 1] = "." then
        findRollUpIdx chars (idx + 1)
    else
        idx
        
findRollUpIdx [| "O"; "."; "#"; "."; |] 0 |> printfn "%d"

let getRowChars (input: string array array) row = input.[row]
let getColChars (input: string array array) col = input |> Array.map (fun x -> x.[col])

let northIdx (input: string array array) row col =
    findRollDownIdx (getColChars input col) row, col
    
let southIdx (input: string array array) row col =
    findRollUpIdx (getColChars input col) row, col
    
let westIdx (input: string array array) row col =
    row, findRollDownIdx (getRowChars input row) col
    
let eastIdx (input: string array array) row col =
    row, findRollUpIdx (getRowChars input row) col

let roll directionFn (input : string array array) =
    let rowLength = input[0].Length
    
    let rec loop (input : string array array) (row : int) (col : int) =
        if row >= input.Length then
            input
        else
            let curr = input[row][col]
            let nextRow,nextCol = incrementRowCol rowLength row col ((+) 1)         
            if curr = "." || curr = "#" then
                loop input nextRow nextCol
            else if curr = "O" then
                // determine where this can be moved
                let newRow,newCol = directionFn input row col
                input[row].[col] <- "."
                input[newRow].[newCol] <- "O"
                //debugPrint input row col newRow                   
                loop input nextRow nextCol
            else
                failwith "unknown char"
    loop input 0 0

let rollBackwards directionFn (input : string array array) =
    let rowLength = input[0].Length
    let colLength = input.Length
    
    let rec loop (input : string array array) (row : int) (col : int) =
        if row < 0 || col < 0 then
            input
        else
            let curr = input[row][col]
            let nextRow,nextCol = incrementRowCol rowLength row col (flip (-) 1)   
            if curr = "." || curr = "#" then
                loop input nextRow nextCol
            else if curr = "O" then
                // determine where this can be moved
                let newRow,newCol = directionFn input row col
                input[row].[col] <- "."
                input[newRow].[newCol] <- "O"
                //debugPrint input row col newRow                   
                loop input nextRow nextCol
            else
                failwith "unknown char"
    loop input (rowLength - 1) (colLength - 1)

let score (input : string array array) =
    input |> Array.rev |> Array.mapi (fun rowIdx row -> row |> Array.mapi (fun col char -> if char = "O" then (rowIdx + 1) else 0)) |> Array.concat |> Array.sum

let parse = StringExtras.characterSquare
    
let solve1 = StringExtras.characterSquare >> roll northIdx >> score

let cycle cycleNum input =
    //let pr = printCycle cycleNum
    let pr = id
    let out = roll northIdx >> pr >> roll westIdx >> pr >> rollBackwards southIdx >> pr >> rollBackwards eastIdx >> pr <| input
    out
    
    
let stringify input = input |> Array.map (String.concat "") |> String.concat ""
let rec runCycles detectCycles cycleNum cycleLimit foundCycles input =
    if cycleNum % 1000 = 0 then
        printfn $"cycle %d{cycleNum}" |> ignore
    if cycleNum = cycleLimit then
        input, -1, -1
    else
        let afterCycle = cycle cycleNum input
        let afterString = stringify afterCycle
        
        let isKnownCycle = Map.tryFind afterString foundCycles |> Option.isSome
        let newCycles = Map.add afterString cycleNum foundCycles
        
        printfn $"""cycle %d{cycleNum} has score %d{score afterCycle}. hash is %s{BitConverter.ToString(MD5.HashData(System.Text.Encoding.ASCII.GetBytes afterString)).Replace("-", "")}"""
        
        if detectCycles && isKnownCycle then
            printfn $"finished at %d{cycleNum}"
            let prevCycle = Map.find afterString foundCycles
            let cyclePeriod = cycleNum - prevCycle
            let cycleStart = prevCycle
            printfn $"""cycle start was at %d{prevCycle}. period is %d{cyclePeriod}."""
            
            input, cycleNum, cyclePeriod
        else
            runCycles detectCycles (cycleNum + 1) cycleLimit newCycles afterCycle
    
let solve2 raw =
    let input = StringExtras.characterSquare raw
    let input2 = StringExtras.characterSquare raw
    let _, cycleStart, cyclePeriod = runCycles true 1 10000 Map.empty input
    let cycleNum = ((1_000_000_000 - cycleStart) % cyclePeriod) + cycleStart + 1
    printfn $"using cycle %d{cycleNum}"
    
    runCycles false 1 cycleNum Map.empty input2 |> fst3 |> score
    

let printAnswer = printAnswersWithSameInputs2 solve1 solve2 example1 p1
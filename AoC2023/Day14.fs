module Day14
open Utils
open AoC2023.Inputs.Day14

let incrementRowCol rowLength row col =    
    let nextIdx = (row * rowLength + col) + 1
    let nextRow = nextIdx / rowLength
    let nextCol = nextIdx % rowLength
    nextRow,nextCol

let debugPrint (input : string array array) (row : int) (col : int) (newRow : int) =
    if newRow <> row then
        printfn $"Moved rock from %d{row},%d{col} to %d{newRow},%d{col}"
        input |> Array.map (String.concat "") |> Array.iter (printfn "%s")
        printfn ""
        printfn ""

let findIndexToRollTo (idx : int) (chars : string array) =
    let rec loop endIdx =
        if endIdx = 0 then
            0
        else if chars[endIdx - 1] = "." then
            loop (endIdx - 1)
        else
            endIdx
    loop idx

let rollNorth (input : string array array) =
    let rowLength = input[0].Length
    
    let rec loop (input : string array array) (row : int) (col : int) =
        if row >= input.Length then
            input
        else
            let curr = input[row][col]
            let nextRow,nextCol = incrementRowCol rowLength row col            
            if curr = "." || curr = "#" then
                loop input nextRow nextCol
            else if curr = "O" then
                // determine where this can be moved
                let newRow = input |> Array.map (fun x -> x.[col]) |> findIndexToRollTo row 
                input[row].[col] <- "."
                input[newRow].[col] <- "O"
                debugPrint input row col newRow                   
                loop input nextRow nextCol
            else
                failwith "unknown char"
    loop input 0 0

let score (input : string array array) =
    input |> Array.rev |> Array.mapi (fun rowIdx row -> row |> Array.mapi (fun col char -> if char = "O" then (rowIdx + 1) else 0)) |> Array.concat |> Array.sum

let parse = StringExtras.characterSquare
    
let solve1 = StringExtras.characterSquare >> rollNorth >> score



let solve2 = StringExtras.characterSquare

let printAnswer = printAnswersWithSameInputs1 solve1 solve2 example1 p1
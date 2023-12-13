module Day13
open Utils
open AoC2023.Inputs.Day13

type FoldPoint = Horizontal of int | Vertical of int | NoFoldPoint

let transpose (pattern:_ [][]) =
    if pattern.Length = 0 then failwith "Invalid matrix"  
    Array.init pattern.[0].Length (fun i -> 
        Array.init pattern.Length (fun j -> 
            pattern.[j].[i]))

// checks horizontal fold. Invert matrix to do vertical
let isValidFold foldPoint (pattern : string array array) =
    let rows = Array.length pattern
    let rowsBelow = foldPoint
    let rowsAbove = rows - foldPoint
    let offsetsToCheck = System.Math.Min (rowsBelow, rowsAbove)
    
    let rowsMatch offset =
        let rowAbove = Array.item (foldPoint - offset) pattern
        let rowBelow = Array.item (foldPoint + offset - 1) pattern
        // printfn $"%A{rowAbove}"
        // printfn $"%A{rowBelow}"
        // printfn ""
        rowAbove = rowBelow
        
    seq { 0..offsetsToCheck } |> List.ofSeq |> ListExtras.every rowsMatch    
    
    
let wrapper fpType = Horizontal
    
let findFold pattern =
    let findFoldPoint wrap doTranspose =
        let pattern' = if doTranspose then transpose pattern else pattern
        seq { 1..Array.length pattern' - 1 }
        |> Seq.filter (flip isValidFold pattern')
        |> Seq.tryHead
        |> Option.map wrap
        
    findFoldPoint Horizontal false
    |> Option.orElse (findFoldPoint Vertical true)
    |> Option.defaultValue NoFoldPoint  

let parse = splitInputByDoubleNewLines >> Seq.map charactersAs2dArray

let score fp =
    match fp with
    | Horizontal s -> 100 * s
    | Vertical s -> s
    | NoFoldPoint -> failwith "invalid fold point"
    
let solve1 = parse >> Seq.map findFold >> Seq.sumBy score
let solve2 = id

let printAnswer = printAnswersWithSameInputs1 solve1 solve2 example1 p1
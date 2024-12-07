module Day04

open System
open System.Text.RegularExpressions
open Utils
open AoC2023.Inputs.Day04

let readRows =
    splitInputByNewLines
    >> List.ofSeq
    >> List.map (chars >> List.ofArray)

let readColumns = readRows >> List.transpose

let safeGet (arr: _ [,]) x y =
    if x < 0 || y < 0 || x >= arr.GetLength(0) || y >= arr.GetLength(1) then
        None
    else
        Some arr.[x, y]

let diag x y mutX mutY (mat: _ [,]) =
    let l = max (mat.GetLength(0)) (mat.GetLength(1)) - 1
    [| for i in 0..l ->
        let newX = mutX x i
        let newY = mutY y i
        safeGet mat newX newY
    |]
    |> Array.choose id
    |> String.concat ""


let readDiagonals (input: string) =
    let lettersArr = charactersAs2dArray input
    let letter2dArray = array2D lettersArr
    // these do the top left to bottom right diagonals
    let topLtr = [for i in 0..(letter2dArray.GetLength(1)-1) -> diag 0 i (+) (+) letter2dArray]
    let leftLtr= [for i in 1..(letter2dArray.GetLength(0)-1) -> diag i 0 (+) (+) letter2dArray]

    //these do the right to left diagonals
    let topRtl = [for i in 1..(letter2dArray.GetLength(1)-1) -> diag 0 i (+) (-) letter2dArray]
    let rightRtl =
        [for i in 1..(letter2dArray.GetLength(0)-1) ->
            diag i (letter2dArray.GetLength(0)-1) (+) (-) letter2dArray]

    let all = List.concat [leftLtr; topLtr; topRtl; rightRtl]
    all

let countWord (word: string) (word2: string) (line: string) =
    let c = Regex(word).Matches(line).Count
    let c2 = Regex(word2).Matches(line).Count
    //tapValue2 (c + c2) line
    c + c2

let countStringInMatrix input =
    let rows = readRows input |> List.map (String.concat "")
    let columns = readColumns input |> List.map (String.concat "")
    let diagonals = readDiagonals input
    let lines = List.concat [rows; columns; diagonals;]
    let count = lines |> List.sumBy (countWord "XMAS" "SAMX")
    count

let print1 =
    Console.WriteLine(countStringInMatrix simpleExample1)
    Console.WriteLine(countStringInMatrix example1)
    Console.WriteLine(countStringInMatrix p1)
    ()



let isXmas arr (x, y) =
    let diag1 = [(safeGet arr x y); (safeGet arr (x+1) (y+1)); (safeGet arr (x+2) (y+2))] |> List.choose id |> String.concat ""
    let diag2 = [(safeGet arr (x+2) (y)); (safeGet arr (x+1) (y+1)); (safeGet arr (x) (y+2))] |> List.choose id |> String.concat ""
    (diag1 = "MAS" || diag1 = "SAM") && (diag2 = "MAS" || diag2 = "SAM")

// shouldve followed a similar pattern for p1. wouldve been a LOT simpler.
let solve2 input =
    let arr = charactersAs2dArray input |> array2D
    let indices =
        [for i in 0..(arr.GetLength(0)-1) ->
            [for j in 0..(arr.GetLength(1)-1) ->
                (i, j)]] |> List.concat
    let found = indices |> List.filter (isXmas arr)
    List.length found |> tapValue |> ignore
    found |> List.map string |> String.concat " | " |> tapValue


let print2 =
    Console.WriteLine(solve2 example2)
    Console.WriteLine(solve2 p1)
    ()
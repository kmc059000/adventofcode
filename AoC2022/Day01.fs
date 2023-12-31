﻿module Day01

open System
open AoC2022.Inputs

let parseInt str = Int32.Parse(str)

let parseElf (str : String) =
    str.Split("\n", StringSplitOptions.RemoveEmptyEntries)
    |> Array.map parseInt
    |> Array.toSeq

let elves =
    day01_01.Split("\n\n", StringSplitOptions.RemoveEmptyEntries)
    |> Array.map parseElf
    
let elfSums = elves |> Array.map Seq.sum
    
let maxSum = elfSums |> Seq.max
    
let solveDay0101 = printfn $"%i{maxSum}"

let topXSum sums count =
    sums
    |> Seq.sortDescending
    |> Seq.take count
    |> Seq.sum

let solveDay0102 =
    let solution = topXSum elfSums 3
    printfn $"%i{solution}"
    
//Code Golf version

let elves2 =
    let parseElf (str : String) =
        str.Split("\n", StringSplitOptions.RemoveEmptyEntries)
        |> Seq.map parseInt
    day01_01.Split("\n\n", StringSplitOptions.RemoveEmptyEntries)
    |> Seq.map parseElf
    |> Seq.map Seq.sum
let solveDay0101Variant = elves2 |> Seq.max |> printfn "%i"
let solveDay0102Variant = elves2 |> Seq.sortDescending |> Seq.take 3 |> Seq.sum |> printfn "%i"
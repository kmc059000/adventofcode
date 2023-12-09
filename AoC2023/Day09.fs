module Day09
open System.Text.RegularExpressions
open Utils
open AoC2023.Inputs.Day09

let parse = splitInputByNewLines >> List.ofArray >> List.map (splitByList " " >> List.map int)

let nextInts = List.pairwise >> List.map (fun (x,y) -> y - x)

let rec solve ints =
    if List.exists ((<>) 0) ints then
        let last = List.last ints
        last + solve (nextInts ints)
    else
        0

let solve1 = parse >> List.map solve >> List.reduce (+)
let solve2 = id

let printAnswer = printAnswers1 solve1 example1 p1 solve2 example2 p2
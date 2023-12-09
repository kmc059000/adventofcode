module Day09
open System
open System.Text.RegularExpressions
open Utils
open AoC2023.Inputs.Day09

let parse = splitInputByNewLines >> List.ofArray >> List.map (splitByList " " >> List.map int)

let nextInts = List.pairwise >> List.map (fun (x,y) -> y - x)

let rec solve ints =
    if List.exists ((<>) 0) ints then
        let last = List.last ints
        Console.WriteLine(last)
        last + solve (nextInts ints)
    else
        0
        
let rec solveFront ints =
    if List.exists ((<>) 0) ints then
        let first = List.head ints
        first - solveFront (nextInts ints)
    else
        0

let solve1 = parse >> List.map solve >> List.reduce (+)
let solve2 = parse >> List.map solveFront >> List.reduce (+)

let printAnswer = printAnswersWithSameInputs2 solve1 solve2 example1 p1
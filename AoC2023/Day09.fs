module Day09
open Utils
open AoC2023.Inputs.Day09

let parse = splitInputByNewLines >> List.ofArray >> List.map (splitByList " " >> List.map int)
let isAllZeros = List.exists ((<>) 0) >> not
let nextInts = List.pairwise >> List.map (TupleExtras.applyBack (-))

let rec solveBack ints =
    
    if isAllZeros ints then
        0
    else
        List.last ints + solveBack (nextInts ints)
        
let rec solveFront ints =
    if isAllZeros ints then
        0
    else
        List.head ints - solveFront (nextInts ints)

let solve lineSolver = parse >> List.map lineSolver >> List.reduce (+)
let solve1 = solve solveBack
let solve2 = solve solveFront

let printAnswer = printAnswersWithSameInputs solve1 solve2 example1 p1
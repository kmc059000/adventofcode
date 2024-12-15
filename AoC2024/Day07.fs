module Day07

open System
open System.Globalization
open Microsoft.FSharp.Core
open Utils
open AoC2023.Inputs.Day07

type Equation = {
    testValue: uint64
    nums: uint64 list
}

let rec sumAndProduct allowConcat (prev: uint64 list) (rest: uint64 list) =
    match rest with
    | [] -> prev
    | h::t ->
        match prev with
        | [] -> sumAndProduct allowConcat [h] t
        | _ ->
            let both = fun value ->
                let sandp = [value + h; value * h]
                if allowConcat then
                    sandp @ [(string value) + (string h) |> uint64]
                else
                    sandp
            let newPrev = List.collect both prev
            sumAndProduct allowConcat newPrev t

let test allowConcat (equation: Equation) =
    let results = sumAndProduct allowConcat [] equation.nums
    let satisfies = List.exists ((=) equation.testValue) results
    satisfies

let solve allowConcat (equations: Equation list) =
    let matchingEquations = List.filter (test allowConcat) equations

    matchingEquations |> List.sumBy _.testValue


let parse  =
    splitInputByNewLinesList >> List.map (fun line ->
        let tokens = splitBy ":" line
        let testValue = uint64 tokens.[0]
        let nums = splitBySpaces tokens.[1] |> List.ofArray |> List.map uint64
        { testValue = testValue; nums = nums }
    )

let parseAndSolve = parse >> solve false

let print1 =
    Console.WriteLine(parseAndSolve example1)
    Console.WriteLine(parseAndSolve p1)
    ()

let parseAndSolve2 = parse >> solve true
   
let print2 =
    Console.WriteLine(parseAndSolve2 example1)
    Console.WriteLine(parseAndSolve2 p1)
    ()
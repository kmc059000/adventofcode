module Day01

open System
open Utils
open AoC2023.Inputs.Day01

let p1Cases = [1..9] |> List.map (fun x -> (x.ToString(), x))
let p2Cases =
    [ "one"; "two"; "three"; "four"; "five"; "six"; "seven"; "eight"; "nine" ]
    |> List.mapi (fun i x -> (x.ToString(), (i + 1)))
    |> List.append p1Cases
    
let findMatch findIndex matchPicker cases str =
    cases
    |> List.map (fun (matchText, value) -> (findIndex matchText str, value))
    |> List.filter (fst >> isPositive)
    |> matchPicker fst
    |> snd
   
let firstMatch = findMatch indexOf List.minBy
let lastMatch = findMatch lastIndexOf List.maxBy

let calibrateLine cases str = (firstMatch cases str * 10) + (lastMatch cases str)
let calibrateLines calibrateLine = splitInputByNewLines >> Seq.sumBy calibrateLine

let solve1 = calibrateLines <| calibrateLine p1Cases
let solve2 = calibrateLines <| calibrateLine p2Cases

let print1 =
    Console.WriteLine(solve1 day1p1Example)
    Console.WriteLine(solve1 day1p1)
    ()
   
let print2 =
    Console.WriteLine(solve2 day1p2example)
    Console.WriteLine(solve2 day1p1)
    ()
module Day02

open System
open Utils
open AoC2023.Inputs.Day02

let isSafe (level : int list) =
    let differences = level |> List.windowed 2 |> List.map (List.reduce (-))
    let smallEnough = differences |> List.filter (abs >> (<) 3) |> List.isEmpty
    let sameDirection = differences |> List.countBy sign |> List.length = 1

    smallEnough && sameDirection

let countSafeReports = splitInputByNewLinesList >> List.map splitIntsBySpacesList >> List.filter isSafe >> List.length

let print1 =
    Console.WriteLine(countSafeReports example1)
    Console.WriteLine(countSafeReports p1)
    ()

let isSafe2 (level : int list) =
    let length = List.length level
    let allCombos = [0..length-1] |> List.map (flip List.removeAt level)
    allCombos |> List.exists isSafe

let countSafeReports2 =
    splitInputByNewLinesList
    >> List.map splitIntsBySpacesList
    >> List.filter isSafe2
    >> List.length

let print2 =
    Console.WriteLine(countSafeReports2 example1)
    Console.WriteLine(countSafeReports2 p1)
    ()
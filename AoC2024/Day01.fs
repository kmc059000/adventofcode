module Day01

open System
open Utils
open AoC2023.Inputs.Day01

let distance str =
    let getList col = readColumn col >> List.map int >> List.sort
    let leftLocations = getList 0 str
    let rightLocations = getList 1 str
    let distances = Seq.zip leftLocations rightLocations |> Seq.map absDifference |> Seq.sum
    distances

let distance2 str =
    let getList col = readColumn col >> List.map int >> List.sort
    let rightOccurrencesByLeftLocation = getList 1 str |> List.countBy id |> Map.ofList |> (flip Map.tryFind) >> Option.defaultValue 0
    let leftLocations = getList 0 str
    let distances = leftLocations |> List.map (fun location -> location * rightOccurrencesByLeftLocation location) |> Seq.sum
    distances

let print1 =
    Console.WriteLine(readColumns example1)
    Console.WriteLine(distance example1)
    Console.WriteLine(distance p1)
    ()

let print2 =
    Console.WriteLine(distance2 example1)
    Console.WriteLine(distance2 p1)
    ()
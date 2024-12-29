module Day19

open System
open Utils
open AoC2023.Inputs.Day19

module P1 =
    let isValidPattern (allowedPatterns: string list) (pattern: string) : bool =
        let mutable cache : Map<string, bool> = Map.empty
        let rec loop (pattern: string) : bool =
            let isDecided = Map.tryFind pattern cache
            match pattern, isDecided with
            | "", _ -> true
            | _, Some valid -> valid
            | _, None ->
                let prefixedWith = allowedPatterns |> List.filter pattern.StartsWith
                if List.isEmpty prefixedWith
                then
                    cache <- Map.add pattern false cache
                    false
                else
                    let validPattern = prefixedWith |> List.exists (fun p -> loop (pattern.Substring(p.Length)))
                    cache <- Map.add pattern validPattern cache
                    validPattern
        loop pattern

    let countAllValid (allowedPatterns: string list) (patterns: string list) =
        patterns
        |> List.filter (isValidPattern allowedPatterns)
        |> List.length
        
    let solve input =
        let tokens = splitInputByDoubleNewLines input
        let allowedStripes = tokens[0] |> splitByCommaList
        let patterns = tokens[1] |> splitInputByNewLinesList
        countAllValid allowedStripes patterns

let print1 =
    Console.WriteLine(P1.solve example1)
    Console.WriteLine(P1.solve p1)
    ()
    
module P2 =
    let countValidPatterns (allowedPatterns: string list) (pattern: string) : uint64 =
        let mutable cache : Map<string, uint64> = Map.empty
        let rec loop (pattern: string) : uint64 =
            let isDecided = Map.tryFind pattern cache
            match pattern, isDecided with
            | "", _ -> 1UL
            | _, Some valid -> valid
            | _, None ->
                let prefixedWith = allowedPatterns |> List.filter pattern.StartsWith
                if List.isEmpty prefixedWith
                then
                    cache <- Map.add pattern 0UL cache
                    0UL
                else
                    let counts = prefixedWith |> List.map (fun p -> loop (pattern.Substring(p.Length)))
                    let sum = List.sum counts
                    cache <- Map.add pattern sum cache
                    sum
        loop pattern

    let countAllValid (allowedPatterns: string list) (patterns: string list) =
        patterns
        |> List.sumBy (countValidPatterns allowedPatterns)
        
    let solve input =
        let tokens = splitInputByDoubleNewLines input
        let allowedStripes = tokens[0] |> splitByCommaList
        let patterns = tokens[1] |> splitInputByNewLinesList
        countAllValid allowedStripes patterns

let print2 =
    Console.WriteLine(P2.solve example1)
    Console.WriteLine(P2.solve p1)
    ()
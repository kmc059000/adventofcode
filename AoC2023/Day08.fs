module Day08
open System.Text.RegularExpressions
open Utils
open AoC2023.Inputs.Day08

type Node = {
    name: string;
    left: string;
    right: string;
}

let parseDirections = StringExtras.characters >> List.ofArray
    
let parseGraph lines =
    let parseNode line =
        let tokens = line |> StringExtras.matches (Regex("[\dA-Z]{3}", RegexOptions.Compiled))
        { name = tokens[0]; left = tokens[1]; right = tokens[2] }
        
    splitInputByNewLines lines
    |> Seq.map (parseNode >> TupleExtras.replicate >> TupleExtras.mapFst _.name)
    |> Map.ofSeq
    
let parse =
    splitInputByDoubleNewLines
    >> SeqExtras.pluckFirst2ToTuple
    >> TupleExtras.mapFst parseDirections
    >> TupleExtras.mapSnd parseGraph

let findPath anyEnd directions map start =
    let rec loop nodeName remainingDirection steps =
        match Set.contains nodeName anyEnd with
        | true -> steps
        | false ->
               let node = Map.find nodeName map
               match remainingDirection with
               // loop the directions back around
               | [] -> loop nodeName directions steps
               | "L"::rest ->
                   loop node.left rest (steps + 1)
               | "R"::rest ->
                   loop node.right rest (steps + 1)
               | _ -> failwith "invalid character"
    loop start directions 0
    
let answer1 (directions,map) = findPath (Set.ofList ["ZZZ"]) directions map "AAA"

let answer2 (directions,map) =
    let findNodesWithChar (char : string) = map |> Map.values |> Seq.filter (_.name.EndsWith(char)) |> Seq.map _.name
    let startingNodes = findNodesWithChar "A" |> Seq.toList
    let finalNodes = findNodesWithChar "Z" |> Set.ofSeq
    
    startingNodes
    |> Seq.map (findPath finalNodes directions map)
    // ugh this burned me so bad in past iterations because of overflow. gotta be a large enough data type.
    |> tapValues
    |> Seq.map uint64
    |> Seq.reduce lcmU64

let solve1 = parse >> answer1
let solve2 = parse >> answer2

let printAnswer = printAnswers solve1 example1 p1 solve2 example2 p2
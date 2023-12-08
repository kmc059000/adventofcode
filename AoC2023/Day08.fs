module Day08
open System.Text.RegularExpressions
open Utils
open AoC2023.Inputs.Day08

type Direction = Left | Right

type Node = {
    name: string;
    left: string;
    right: string;
}

module Node =
    let id = id

let parseDirections line =
    let parseDirection d =
        match d with
        | "L" -> Left
        | "R" -> Right
        | _ -> failwith "Invalid direction"
    line
    |> StringExtras.characters
    |> Seq.map parseDirection
    |> Seq.toList
    
let parseMap lines =
    let parseNode line =
        let parts = line |> StringExtras.matches (Regex("[A-Z]{3}", RegexOptions.Compiled)) |> Seq.toArray
        let name = parts[0]
        let left = parts[1]
        let right = parts[2]
        name,{ name = name; left = left; right = right }
    
    splitInputByNewLines lines
    |> Seq.map parseNode
    |> tapValues
    |> Map.ofSeq
    

let parse = splitInputByDoubleNewLines >> SeqExtras.pluckFirst2ToTuple >> TupleExtras.mapFst parseDirections >> TupleExtras.mapSnd parseMap

let findPath directions map =
    let rec loop node directions' path =
        match node.name with
        | "ZZZ" -> path
        | _ -> match directions' with
               | [] -> loop node directions path
               | Left::rest ->
                   let nextNode = (Map.find node.left map)
                   loop nextNode rest (nextNode::path)
               | Right::rest ->
                   let nextNode = (Map.find node.right map)
                   loop nextNode rest (nextNode::path)
    loop (Map.find "AAA" map) directions []

let breadthFirstSearch directions map =
    let rec loop queue visited remainingDirections =
        match queue with
        | [] -> failwith "Asdf"
        | (node, path)::rest ->
            if node = "ZZZ" then path
            else
                let node = Map.find node map
                let nextNode,nextDirections = 
                    match directions with
                    | Left::rest -> node.left, rest
                    | Right::rest -> node.right, rest
                    | _ -> failwith "Invalid directions"
                    |> TupleExtras.mapFst (flip Map.find map)
                let visited = Set.add node visited
                let queue = 
                    if Set.contains nextNode visited then queue
                    else (nextNode.name, path + "->" + nextNode.name)::queue
                loop rest visited nextDirections
    loop [("AAA", "")] Set.empty directions
    
let solve (directions,map) = findPath directions map |> Seq.length

let solve1 = parse >> solve
let solve2 = id

let printAnswer = printAnswersWithSameInputs1 solve1 solve2 example1 p1
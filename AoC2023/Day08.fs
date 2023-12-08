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
        let parts = line |> StringExtras.matches (Regex("[\dA-Z]{3}", RegexOptions.Compiled)) |> Seq.toArray
        let name = parts[0]
        let left = parts[1]
        let right = parts[2]
        name,{ name = name; left = left; right = right }
    
    splitInputByNewLines lines
    |> Seq.map parseNode
    |> Map.ofSeq
    

let parse = splitInputByDoubleNewLines >> SeqExtras.pluckFirst2ToTuple >> TupleExtras.mapFst parseDirections >> TupleExtras.mapSnd parseMap

let recLimit = 1000000

let findPath start anyEnd directions map =
    let rec loop node directions' steps =
        match steps = recLimit || Set.contains node.name anyEnd with
        | true -> steps
        | false ->
               match directions' with
               | [] -> loop node directions steps
               | Left::rest ->
                   let nextNode = (Map.find node.left map)
                   loop nextNode rest (steps + 1)
               | Right::rest ->
                   let nextNode = (Map.find node.right map)
                   loop nextNode rest (steps + 1)
    loop (Map.find start map) directions 0
    
let solve (directions,map) = findPath "AAA" (Set.ofList ["ZZZ"]) directions map

//This takes too long but is interesting.
[<TailCall>]
let rec loop map finalNodes allDirections currentNodes directions' steps =
    if steps % 1000000 = 0 then printfn $"step %d{steps}"
    match (Set.ofSeq currentNodes) = finalNodes with
    | true -> steps
    | false ->
        let findNextNodes selectNode =
            currentNodes
            |> List.map (fun n -> (Map.find (selectNode n) map))
            
        let nextNodes,nextDirections,nextSteps =
            match directions' with
            | [] ->
                currentNodes,allDirections,steps
            | Left::rest -> (findNextNodes _.left),rest,(steps + 1)
            | Right::rest -> (findNextNodes _.right),rest,(steps + 1)
        loop map finalNodes allDirections nextNodes nextDirections nextSteps
       

let answer2 (directions,map) =
    let startingNodes = map |> Map.values |> Seq.filter (_.name.EndsWith("A")) |> Seq.toList
    let finalNodes = map |> Map.values |> Seq.filter (_.name.EndsWith("Z")) |> Seq.map _.name |> Set.ofSeq 
            
    // loop map finalNodes directions startingNodes directions 0
    
    //this finds the shortest path for each node, and then multiplies them together
    //so so wrong because the paths dont continually loop around
    // startingNodes
    // |> Seq.map (fun n -> findPath n.name finalNodes directions map)
    // |> Seq.map Seq.length
    // |> Seq.map uint64
    // |> tapValues
    // |> List.ofSeq
    // |> Seq.reduce (*)
    // |> ignore
    
    startingNodes
    |> Seq.map (fun n ->
        finalNodes
        |> Seq.map (fun finalNode -> findPath n.name (Set.ofList [finalNode]) directions map)
        |> Seq.filter ((>) recLimit)
        |> List.ofSeq)
    |> tapValues
    |> Seq.length


let solve1 = parse >> solve
let solve2 = parse >> answer2

let printAnswer = printAnswers2 solve1 example1 p1 solve2 example2 p2
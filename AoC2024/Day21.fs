module Day21

open System
open Utils
open AoC2023.Inputs.Day21

// need to consider all paths since different ones may be more optimal for subsequent controls

let numPadGraph =
    [
      'A', '0'
      'A', '3'
      '0', '2'
      '3', '2'
      '3', '6'
      '2', '1'
      '2', '5'
      '1', '4'
      '6', '5'
      '6', '9'
      '5', '4'
      '5', '8'
      '4', '7'
      '9', '8'
      '8', '7'
    ]
    |> List.collect (fun (a,b) -> [(a,b); (b,a)])

let flipDir d =
    match d with
    | '^' -> 'v'
    | 'v' -> '^'
    | '<' -> '>'
    | '>' -> '<'
    | _ -> failwith "invalid direction"

let numPadDirections =
    [
        ('A', '0'), '<'
        ('A', '3'), '^'
        ('0', '2'), '^'
        ('3', '2'), '<'
        ('3', '6'), '^'
        ('2', '1'), '<'
        ('2', '5'), '^'
        ('1', '4'), '^'
        ('6', '5'), '<'
        ('6', '9'), '^'
        ('5', '4'), '<'
        ('5', '8'), '^'
        ('4', '7'), '^'
        ('9', '8'), '<'
        ('8', '7'), '<'
    ]
    |> List.collect (fun ((from,to_),dir) ->
        [
          ((from,to_),dir)
          ((to_,from), flipDir dir)
        ])
    |> Map.ofList

let arrowDirections =
    [
        ('>', 'A'), '^'
        ('>', 'v'), '<'
        ('v', '<'), '<'
        ('v', '^'), '^'
        ('A', '^'), '<'
        
    ]
    |> List.collect (fun ((from,to_),dir) ->
        [
          ((from,to_),dir)
          ((to_,from), flipDir dir)
        ])
    |> Map.ofList

let arrowGraph =
    [
      '>', 'A'
      '>', 'v'
      'v', '<'
      'v', '^'
      'A', '^'
    ]
    |> List.collect (fun (a,b) -> [(a,b); (b,a)])

let allNumbers = numPadGraph |> List.collect (fun (a,b) -> [a;b]) |> List.distinct
let allArrows = arrowGraph |> List.collect (fun (a,b) -> [a;b]) |> List.distinct

let allCombos lst =
    [for a in lst do
        for b in lst do
            yield (a,b)]
let allLetterCombos = allCombos allNumbers
let allArrowCombos = allCombos allArrows

let allPathsForPair graph (start: char,end_: char) =
    if start = end_ then
        [""]
    else
        let rec allPaths' visited path a b =
            if a = b then
                [List.rev path]
            else
                graph
                |> List.filter (fun (x,y) -> x = a && not (List.contains y visited))
                |> List.collect (fun (x,y) -> allPaths' (y::visited) (y::path) y b)
        allPaths' [start] [start] start end_
        |> List.groupBy List.length
        |> List.sortBy fst
        |> List.head
        |> snd
        |> List.map (fun chars -> String.Concat(Array.ofList(chars)))

let allPathSegments graph combos =
    [for c in combos do
         c, allPathsForPair graph c]
    |> Map.ofList

let pathToArrowPath (directions: Map<char*char, char>) (path: string) =
    path.ToCharArray()
    |> List.ofArray
    |> List.pairwise
    |> List.map (fun (a,b) ->
        Map.find (a,b) directions)
    |> (fun chars -> String.Concat(Array.ofList(chars)))
    
let numPadShortestPaths_ =
    allPathSegments numPadGraph allLetterCombos
    |> Map.map (fun _ numPaths ->
        List.map (pathToArrowPath numPadDirections) numPaths)
    
let arrowShortestPaths_ =
    allPathSegments arrowGraph allArrowCombos
    |> Map.map (fun _ numPaths ->
            List.map (pathToArrowPath arrowDirections) numPaths)
    
let allPaths map code =
    let paths =
        ("A" + code).ToCharArray()
        |> Seq.ofArray
        |> Seq.pairwise
        |> Seq.map (flip Map.find map)
        |> List.ofSeq
    
    // segments has all shortest paths for a given segment
    // parents includes all combinations of preceeding segments
    // combine the current segment's paths with all of the parents
    let rec allPaths' (segments: string list list) (parents: string list): string seq =
        match segments with
        | [] -> parents
        | segmentPaths::rest ->
            segmentPaths
            |> Seq.collect
                   (fun segmentPath ->
                        let newParents =
                            parents |> List.map (fun parent -> parent + segmentPath + "A")
                        allPaths' rest newParents)
    allPaths' paths [""]

let codesToArrows = allPaths numPadShortestPaths_
let arrowsToArrows code = allPaths arrowShortestPaths_ code

let path = codesToArrows >> Seq.collect arrowsToArrows >> Seq.collect arrowsToArrows

//"379A" |> path |> List.minBy _.Length |> _.Length |> tapValue |> ignore

let totalShortestPath (pathFn: string -> string seq) = pathFn >> Seq.minBy _.Length >> _.Length

let score pathFn code =
    let num = readInts code |> List.head
    let score = totalShortestPath pathFn code
    score, num
    
let parseAndSolve pathfn = splitInputByNewLinesList >> List.map (score pathfn) >> tapValues >> List.ofSeq >> List.map (fun (a,b) -> a * b) >> List.sum

//totalShortestPath "029A" |> printfn "%i"
//printfn "<v<A>>^AvA^A<vA<AA>>^AAvA<^A>AAvA^A<vA>^AA<A>A<v<A>A>^AAAvA<^A>A"

let print1 =
    // Console.WriteLine(parseAndSolve path example1)
    // Console.WriteLine(parseAndSolve path p1)
    ()

let progress a =
    printfn $"here %s{Guid.NewGuid().ToString()}"
    a

let level = Seq.collect arrowsToArrows >> progress

let path2 = codesToArrows
            >> level >> level >> level >> level >> level
            >> level >> level >> level >> level >> level
            >> level >> level >> level >> level >> level
            >> level >> level >> level >> level >> level
            >> level >> level >> level >> level >> level
 
"029A" |> path2 |> tapValues |> Seq.minBy _.Length |> _.Length |> tapValue |> ignore

let print2 =
    //Console.WriteLine(parseAndSolve path example1)
    Console.WriteLine("")
    ()
    
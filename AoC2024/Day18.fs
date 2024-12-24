module Day18

open System
open System.Xml.Schema
open Utils
open AoC2023.Inputs.Day18

type Coord = { row: int; col: int }

let parse input =
    let parseLine = splitBy "," >> Array.map int >> (fun arr -> { col = arr[0]; row = arr[1] })
    let coords = input |> (splitInputByNewLinesList >> List.map parseLine)
    
    let rec getAllSteps mapsAtStep rest i =
        match rest with
        | coord::rest ->
            let prev = Map.tryFind (i - 1) mapsAtStep |> Option.defaultValue Set.empty
            let next = Set.add coord prev
            let nextMap = Map.add i next mapsAtStep
            getAllSteps nextMap rest (i + 1)
        | [] -> mapsAtStep
    getAllSteps Map.empty coords 1    
    
let exitSample = { col = 6; row = 6; }
let exit = { col = 70; row = 70; }

let printStatus exit current step coordsAtSteps =
    Console.WriteLine("Trying  " + current.ToString() + " at step " + step.ToString())
    for row in 0..exit.row do
        for col in 0..exit.col do
            let coord = { row = row; col = col }
            let c = 
                if coord = current then
                    "X"
                elif Set.contains coord (Map.find step coordsAtSteps) then
                    "#"
                else
                    "."
            Console.Write(c)
        Console.WriteLine()

let getNeighbors exit coords coord =
    let neighbors coord =
        seq {
            { row = coord.row - 1; col = coord.col }
            { row = coord.row + 1; col = coord.col }
            { row = coord.row; col = coord.col - 1 }
            { row = coord.row; col = coord.col + 1 }
        }
        
    let inBounds exit coord =
        coord.row >= 0 && coord.row <= exit.row
        && coord.col >= 0 && coord.col <= exit.col
        
    let fn =
        neighbors
        >> Seq.filter (inBounds exit)
        >> Seq.filter (flip Set.contains coords >> not)
        
    let ns = fn coord |> List.ofSeq
    ns

let hCost coord = manhattanDistanceInt (exit.row, exit.col) (coord.row, coord.col)

let aStarSearch start exit (getNeighbors: Coord -> Coord list) hCost maxStep =
    let rec getPath parents current =
        seq {
            yield current
            match Map.tryFind current parents with
            | None -> ()
            | Some nextParent -> yield! getPath parents nextParent
        }
    
    let rec loop (openList : Coord list) (closedSet: Set<Coord>) fScores gScores parents =
        let sortedByFScore = List.sortBy (flip Map.find fScores) openList
        
        match sortedByFScore with
        | current::_ when current = exit ->
            // we reached the end, so go get the full path back to the start.
            // let path = getPath parents current |> List.ofSeq
            // if path |> Seq.length >= maxStep then
            //     []
            // else
            //     path
            getPath parents current
        // keep looking
        | current::rest ->
            // current node established so put in the closed set. 
            let closedSet = Set.add current closedSet
            let currentG = Map.find current gScores
          
            let inClosed = flip Set.contains closedSet
            
            let nextOpenList, nextGScores, nextFScores, nextLast =
                current
                |> getNeighbors
                |> Seq.filter (inClosed >> not)
                |> Seq.fold (fun (openList, gScores, fScores, parents) neighbor ->
                    let neighborG = currentG + 1
                    if List.contains neighbor openList |> not then
                        // we've never seen this neighbor before, so add it to the open list
                        // and make it's the parent the current path.
                        
                        let openList = neighbor::openList
                        let parents = Map.add neighbor current parents
                        let gScores = Map.add neighbor neighborG gScores
                        let fScores = Map.add neighbor (neighborG + hCost neighbor) fScores
                        openList, gScores, fScores, parents
                    elif neighborG < Map.find neighbor gScores then
                        // we've seen this neighbor before, but this path is better
                        // so update the path and parent.
                        let parents = Map.add neighbor current parents
                        let gScores = Map.add neighbor neighborG gScores
                        let fScores = Map.add neighbor (neighborG + hCost neighbor) fScores
                        openList, gScores, fScores, parents
                    else
                        openList, gScores, fScores, parents
                ) (rest, gScores, fScores, parents)
            loop nextOpenList closedSet nextFScores nextGScores nextLast
        | _ -> []
            
    
    let fScores = Map.ofList [start, 1]
    let gScores = Map.ofList [start, 1]
    let openList = [start]
    let closedSet = Set.empty
    loop openList closedSet fScores gScores Map.empty


let printPath bestPath coordsAtSteps exit =
    let lastStep = List.length bestPath
    Console.WriteLine("Found path with length " + lastStep.ToString())
    for row in 0..exit.row do
        for col in 0..exit.col do
            let coord = { row = row; col = col }
            let c = 
                if List.contains coord bestPath then
                    "O"
                elif Set.contains coord (Map.find lastStep coordsAtSteps) then
                    "#"
                else
                    "."
            Console.Write(c)
        Console.WriteLine()

let solve input exit step =
    let coordsAtSteps = parse input
    
    
    let rec findShortestPath step =
        let coords = Map.find step coordsAtSteps
        let bestPath = aStarSearch { row = 0; col = 0; } exit (getNeighbors exit coords) hCost step |> List.ofSeq |> List.rev
        match bestPath with
        | [] -> findShortestPath (step + 1)
        | _ -> bestPath
    
    let bestPath = findShortestPath step
    printPath bestPath coordsAtSteps exit
    bestPath |> Seq.length

let print1 =
    Console.WriteLine(solve example1 exitSample 12)
    Console.WriteLine(solve p1 exit 1024)
    ()

let parse2 input =
    let parseLine = splitBy "," >> Array.map int >> (fun arr -> { col = arr[0]; row = arr[1] })
    let coords = input |> (splitInputByNewLinesList >> List.map parseLine)
    coords
    
let solve2 input exit step =
    let coordsAtSteps = parse input
    
    let coords = parse2 input
    let bestPath = aStarSearch { row = 0; col = 0; } exit (getNeighbors exit (Map.find step coordsAtSteps)) hCost step |> List.ofSeq |> List.rev
    
    // printPath bestPath coords exit
    
    let remainingCoords = coords |> List.skip (List.length bestPath - 1)
    
    let first = remainingCoords |> Seq.ofList |> Seq.filter (fun c -> List.contains c bestPath) |> Seq.head
    first
   
let print2 =
    Console.WriteLine(solve2 example1 exitSample 12)
    Console.WriteLine("")
    ()
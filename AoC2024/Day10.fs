module Day10

open System
open Utils
open AoC2023.Inputs.Day10

let neighbors (row,col) = seq {
    yield row, col - 1
    yield row, col + 1
    yield row - 1, col
    yield row + 1, col
}

let validatedNeighbor currentValue map (row, col) =
    map
    |> Map.tryFind (row, col)
    |> Option.filter (fun v -> v <> "." && (int v) = ((int currentValue) + 1))
    |> Option.map (fun _ -> row, col)

let nextNeighbors map (row, col) =
    let currValue = Map.find (row, col) map
    neighbors (row, col)
    |> Seq.choose (validatedNeighbor currValue map)

type State = {
    totalPaths: int
    visited: Set<(int * int)>
}

let rec findAllPaths visitOnce state currPos map =
    let currValue = Map.find currPos map
    if visitOnce && Set.contains currPos state.visited then
        state
    elif currValue = "9"
    then
        { state with
                totalPaths = state.totalPaths + 1
                visited = Set.add currPos state.visited }
    else
        currPos
        |> nextNeighbors map
        |> Seq.fold (fun acc nextPos ->
            findAllPaths visitOnce acc nextPos map) state

let findAllTrailheads map =
    map
    |> Map.toSeq
    |> Seq.filter (snd >> ((=) "0"))
    |> Seq.map fst

let parseAsMap str =
    charactersAs2dArray str
    |> Seq.mapi (fun rowIdx row ->
        row
        |> Seq.mapi (fun colIdx value -> (rowIdx,colIdx), value))
    |> Seq.collect id
    |> Map.ofSeq

let solve input =
    let map = parseAsMap input
    let trailheads = findAllTrailheads map
    trailheads
    |> Seq.sumBy (fun th -> findAllPaths true { totalPaths = 0; visited = Set.empty } th map |> _.totalPaths)

let print1 =
    Console.WriteLine(solve example1)
    Console.WriteLine(solve p1)
    ()

let solve2 input =
    let map = parseAsMap input
    let trailheads = findAllTrailheads map
    trailheads
    |> Seq.sumBy (fun th -> findAllPaths false { totalPaths = 0; visited = Set.empty } th map |> _.totalPaths)

let print2 =
    Console.WriteLine(solve2 example2)
    Console.WriteLine(solve2 example1)
    Console.WriteLine(solve2 p1)
    ()
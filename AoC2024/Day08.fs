module Day08

open System
open Utils
open AoC2023.Inputs.Day08

type Coord = {
    row: int
    col: int
}

type Antenna = {
    pos: Coord
    value: String
}

type Map = {
    antennas: Antenna list
    height: int
    width: int
}

let makeAntenna row col value = { pos = { row = row; col = col }; value = value }

let antinode a b =
    let run = a.col - b.col
    let rise = a.row - b.row
    { col = a.col + run; row = a.row + rise }

let bothAntinodes a b =
    [antinode a b; antinode b a]
let bothAntinodesTuple (a,b) = bothAntinodes a b


let rec pairs l = seq {
    match l with
    | h::t ->
      for e in t do
        yield h, e
      yield! pairs t
    | _ -> ()
    }

let pairsList = pairs >> List.ofSeq

let isValidPos map pos =
    pos.row >= 0 && pos.row < map.height && pos.col >= 0 && pos.col < map.width

let allAntinodesOfGroup = pairsList >> List.map bothAntinodesTuple >> List.concat

let antennaGroups map =
    map.antennas |> List.groupBy _.value |> List.map snd |> List.ofSeq

let allAntinodes map =
     map
     |> (antennaGroups
         >> List.collect (List.map _.pos >> List.ofSeq >> allAntinodesOfGroup)
         >> List.filter (isValidPos map))


let parse str =
    let grid = charactersAs2dArray str
    let antennas = Seq.mapi (fun row -> Seq.mapi (makeAntenna row))
                   >> Seq.collect id
                   >> Seq.filter (fun antenna -> antenna.value <> ".")
                   >> List.ofSeq
                   <| grid

    let grid = { antennas = antennas; height = Array.length grid; width = Array.length grid.[0] }
    grid

let solve = allAntinodes >> List.distinct >> List.length

let solveAndParse = parse >> solve

let print1 =
    Console.WriteLine(solveAndParse example1)
    Console.WriteLine(solveAndParse p1)
    ()
   
let print2 =
    Console.WriteLine("")
    Console.WriteLine("")
    ()
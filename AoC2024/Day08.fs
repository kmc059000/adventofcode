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

type Board = {
    antennas: Antenna list
    rows: int
    columns: int
}

let makeAntenna row col value = { pos = { row = row; col = col }; value = value }

let allAntinodes a b =
    let antinode a b =
        let run = a.col - b.col
        let rise = a.row - b.row
        { col = a.col + run; row = a.row + rise }
    [antinode a b; antinode b a]


let rec pairs l = seq {
    match l with
    | h::t ->
      for e in t do
        yield h, e
      yield! pairs t
    | _ -> ()
    }

let isValidPos map pos =
    pos.row >= 0 && pos.row < map.rows && pos.col >= 0 && pos.col < map.columns

let antinodesOfGroup antinodeFn = pairs >> List.ofSeq >> List.map (fun a -> a ||> antinodeFn) >> List.concat

let antennaGroups map =
    map.antennas |> List.groupBy _.value |> List.map snd |> List.ofSeq

let mapAntinodes antinodeFn map =
     map
     |> (antennaGroups
         >> List.collect (List.map _.pos >> List.ofSeq >> antinodesOfGroup antinodeFn)
         >> List.filter (isValidPos map))


let parse str =
    let grid = charactersAs2dArray str
    let antennas = Seq.mapi (fun row -> Seq.mapi (makeAntenna row))
                   >> Seq.collect id
                   >> Seq.filter (fun antenna -> antenna.value <> ".")
                   >> List.ofSeq
                   <| grid

    let grid = { antennas = antennas; rows = Array.length grid; columns = Array.length grid.[0] }
    grid

let solve antinodeFn = mapAntinodes antinodeFn >> List.distinct >> List.length

let solveAndParse = parse >> solve allAntinodes

let print1 =
    Console.WriteLine(solveAndParse example1)
    Console.WriteLine(solveAndParse p1)
    ()


let allAntinodes2 (map: Board) a b =
    let count = Math.Max(map.rows, map.columns)
    let antinodes a b =
        let run = a.col - b.col
        let rise = a.row - b.row
        [ for i in 0..count do
             yield { col = a.col + (i * run); row = a.row + (i * rise) }
             ]
        |> List.filter (isValidPos map)

    List.concat [ antinodes a b; antinodes b a]

let solveAndParse2 str =
    let parsed = parse str
    solve (allAntinodes2 parsed) parsed

let print2 =
    Console.WriteLine(solveAndParse2 example1)
    Console.WriteLine(solveAndParse2 p1)
    ()
module Day16
open Utils
open AoC2023.Inputs.Day16


type Direction = Left | Right | Up | Down

type Beam = {row: int; col: int; direction: Direction}

let initialBeam = {row = 0; col = 0; direction = Right}

let standardMove beam =
    match beam.direction with
    | Left -> {beam with col = beam.col - 1}
    | Right -> {beam with col = beam.col + 1}
    | Up -> {beam with row = beam.row - 1}
    | Down -> {beam with row = beam.row + 1}
    |> List.singleton

let angleBeam1 beam =
    match beam.direction with
    | Left -> {beam with direction = Down}
    | Right -> {beam with direction = Up}
    | Up -> {beam with direction = Right}
    | Down -> {beam with direction = Left}
    |> standardMove
    
let angleBeam2 beam =
    match beam.direction with
    | Left -> {beam with direction = Up}
    | Right -> {beam with direction = Down}
    | Up -> {beam with direction = Left}
    | Down -> {beam with direction = Right}
    |> standardMove
    
let splitUpDownBeam beam =
    match beam.direction with
    | Left | Right ->
        [
            { beam with direction = Up } |> standardMove;
            { beam with direction = Down } |> standardMove;
        ] |> List.collect id
    | Up | Down -> standardMove beam

let splitLeftRightBeam beam =
    match beam.direction with
    | Up | Down ->
        [
            { beam with direction = Left } |> standardMove;
            { beam with direction = Right } |> standardMove;
        ] |> List.collect id
    | Left | Right -> standardMove beam

let moveBeam (input: string array array) beam =
    let r =
        match input[beam.row][beam.col] with
        | "." -> standardMove beam
        | "/" -> angleBeam1 beam
        | @"\" -> angleBeam2 beam
        | "|" -> splitUpDownBeam beam
        | "-" -> splitLeftRightBeam beam
        | _ -> failwith "??"
    r

let isValidBeam input beam =
    beam.row >= 0 && beam.row < Array.length input &&
    beam.col >= 0 && beam.col < Array.length input[0]

let printVisited input visited =
    let visitedPositions = visited |> Set.map (fun beam -> (beam.row, beam.col))
    Array.iteri (fun i row ->
        Array.iteri (fun j col ->
            if Set.contains (i,j) visitedPositions then
                printf "#"
            else
                printf "."
        ) row
        printf "\n"
    ) input
    
let projectBeams print (input: string array array) start =
    let rec loop beams visited =
        let newBeams =
            beams
            |> List.collect (moveBeam input)
            |> List.filter (isValidBeam input)
            // omit any already processed beams to avoid cycles
            |> List.filter (flip Set.contains visited >> not)
            
        let allVisited = beams |> Set.ofList |> Set.union visited
        
        if List.isEmpty newBeams then
            allVisited
        else
            loop newBeams allVisited
    let r = loop [start] (Set.singleton start)
    if print then printVisited input r |> ignore else ()
    r

let countBeams beams =
    beams
    |> Set.map (fun beam -> (beam.row, beam.col))
    |> Set.count 


let parse = charactersAs2dArray

let solve1 = charactersAs2dArray >> flip (projectBeams false) initialBeam >> countBeams


let findBestStart (input: string array array) =
    let starts =
        [
            [for r in 0..(Array.length input) - 1 do {row = r; col = 0; direction = Right}];
            [for r in 0..(Array.length input) - 1 do {row = r; col = Array.length input[0] - 1; direction = Left}]
            [for c in 0..(Array.length input[0]) - 1 do {row = 0; col = c; direction = Down}];
            [for c in 0..(Array.length input[0]) - 1 do {row = Array.length input - 1; col = c; direction = Up}];
        ]
        |> List.collect id
    
    let best = starts
               //|> List.map (fun start -> projectBeams input start |> TupleExtras.from2 start)
               |> List.maxBy (projectBeams false input >> countBeams)
    projectBeams true input best |> countBeams  
    

let solve2 = charactersAs2dArray >> findBestStart
    

let printAnswer = printAnswersWithSameInputs2 solve1 solve2 example1 p1
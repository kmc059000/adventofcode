module Day06

open System
open Utils
open AoC2023.Inputs.Day06

type Position = {
    x: int
    y: int
}

type Direction =
    | Up
    | Right
    | Down
    | Left

type BoardState = {
    pos: Position
    dir: Direction
    obstacles: Set<Position>
    width: int
    height: int
    visited: Set<Position>
    finished: bool
}

type Cell = Empty | Obstacle | Guard

type MoveResult = CanMove | OutOfBounds | HitObstacle

let parseCell c =
    match c with
    | "." -> Empty
    | "#" -> Obstacle
    | "^" -> Guard
    | _ -> failwith "Invalid cell"

let mapWithPosition f arr =
    arr
    |> Seq.mapi (fun y row ->
        row
        |> Seq.mapi (fun x c -> { x = x; y = y }, f c))

let parseCellMap =
    charactersAs2dArray
    >> mapWithPosition parseCell
    >> Seq.collect id
    >> Map.ofSeq

let parse (input: string) =
    let parsed = parseCellMap input
    let guard = parsed |> Map.filter (fun _ v -> v = Guard) |> Map.keys |> Seq.head
    let obstacles = parsed |> Map.filter (fun _ v -> v = Obstacle) |> Map.keys |> Set.ofSeq
    let width = input |> (splitInputByNewLines >> Seq.head >> Seq.length)
    let height = input |> (splitInputByNewLines >> Seq.length)
    { pos = guard; dir = Up; obstacles = obstacles; width = width; height = height; visited = Set.ofList [guard]; finished = false }

let nextCell (state: BoardState) =
    let pos = state.pos
    match state.dir with
    | Up -> { pos with y = pos.y - 1 }
    | Right -> { pos with x = pos.x + 1 }
    | Down -> { pos with y = pos.y + 1 }
    | Left -> { pos with x = pos.x - 1 }

let isValidPos (state: BoardState) pos =
    if pos.x < 0 || pos.x >= state.width then OutOfBounds
    elif pos.y < 0 || pos.y >= state.height then OutOfBounds
    elif Set.contains pos state.obstacles then HitObstacle
    else CanMove

let rotate (dir: Direction) =
    match dir with
    | Up -> Right
    | Right -> Down
    | Down -> Left
    | Left -> Up

let move (state: BoardState) : BoardState =
    let nextPos = nextCell state
    let canMove = isValidPos state nextPos
    match canMove with
    | CanMove -> { state with pos = nextPos; visited = Set.add nextPos state.visited }
    | HitObstacle -> { state with dir = rotate state.dir }
    | OutOfBounds -> { state with finished = true }

let rec run (state: BoardState) =
    if state.finished then state
    else run (move state)

let solve = parse >> run >> _.visited >> Set.count

let print1 =
    Console.WriteLine(solve example1)
    Console.WriteLine(solve p1)
    ()
   
let print2 =
    Console.WriteLine("")
    Console.WriteLine("")
    ()
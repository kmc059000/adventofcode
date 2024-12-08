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

type Guard = {
    pos: Position
    dir: Direction
}

type FinalState = OutOfBounds | LoopDetected

type BoardState = {
    guard: Guard
    obstacles: Set<Position>
    width: int
    height: int
    visited: Set<Position>
    paths: Set<Guard>
    finished: FinalState option
}

type Cell = Empty | Obstacle | Guard

type MoveResult = CanMove | OutOfBounds | HitObstacle | LoopDetected

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
    let guardPos = parsed |> Map.filter (fun _ v -> v = Guard) |> Map.keys |> Seq.head
    let obstacles = parsed |> Map.filter (fun _ v -> v = Obstacle) |> Map.keys |> Set.ofSeq
    let width = input |> (splitInputByNewLines >> Seq.head >> Seq.length)
    let height = input |> (splitInputByNewLines >> Seq.length)
    let guard = { pos = guardPos; dir = Up }
    { guard = guard; obstacles = obstacles; width = width; height = height; visited = Set.ofList [guardPos]; finished = None; paths = Set.ofList [guard] }

let nextCell (state: BoardState) =
    let pos = state.guard.pos
    match state.guard.dir with
    | Up -> { pos with y = pos.y - 1 }
    | Right -> { pos with x = pos.x + 1 }
    | Down -> { pos with y = pos.y + 1 }
    | Left -> { pos with x = pos.x - 1 }

let isValidPos (state: BoardState) pos =
    if pos.x < 0 || pos.x >= state.width then OutOfBounds
    elif pos.y < 0 || pos.y >= state.height then OutOfBounds
    elif Set.contains { state.guard with pos = pos } state.paths then LoopDetected
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
    | CanMove ->
        { state with
            guard.pos = nextPos;
            visited = Set.add nextPos state.visited
            paths = Set.add { pos = nextPos; dir = state.guard.dir } state.paths
        }
    | HitObstacle -> { state with guard.dir = rotate state.guard.dir }
    | OutOfBounds -> { state with finished = Some FinalState.OutOfBounds }
    | LoopDetected -> { state with finished = Some FinalState.LoopDetected }

let rec run (state: BoardState) =
    match state.finished with
    | Some _ -> state
    | None -> run (move state)

let solve = parse >> run >> _.visited >> Set.count

let print1 =
    Console.WriteLine(solve example1)
    Console.WriteLine(solve p1)
    ()

// obstacles have to be on one of the visited cells, of which there are 5531.
// we have to detect a loop too. A loop exists if the next position+direction is equal to already walked position+direction

let testNewObstacle state obstacle =
    let startingState = { state with obstacles = Set.add obstacle state.obstacles }
    let testedState = run startingState
    match testedState.finished with
    | Some FinalState.LoopDetected -> true
    | Some FinalState.OutOfBounds -> false
    | None -> failwith "huh"

let solve2 input =
    let parsedState = parse input
    let solvedState = run parsedState
    let potentialObstacles = solvedState.visited
    potentialObstacles
    |> Seq.filter (testNewObstacle parsedState)
    |> Seq.length


let print2 =
    Console.WriteLine(solve2 example1)
    Console.WriteLine(solve2 p1)
    ()
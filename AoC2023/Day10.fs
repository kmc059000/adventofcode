module Day10
open Utils
open AoC2023.Inputs.Day10

let parse = charactersAs2dArray

let rec startingPosition (grid : string array array) row col =
    match grid[row][col] with
    | "S" -> (row,col)
    | _ ->
        let columns = grid[0].Length
        let idx = (row * columns + col) + 1
        let nextRow = idx / columns
        let nextCol = idx % columns
        startingPosition grid nextRow nextCol

let findConnectingCells (grid : string array array) (row,col) =
    let rows = grid.Length
    let columns = grid[0].Length
    let up = ((row - 1), col)
    let down = ((row + 1), col)
    let left = row, col - 1
    let right = row, col + 1
    let tryIdx (row,col) = Array.tryItem row grid |> Option.map (Array.tryItem col) |> Option.flatten
    let paths = [
        match tryIdx up with
        | Some "|" | Some "7" | Some "F" -> Some up
        | _ -> None;
        match tryIdx down with
        | Some "|" | Some "L" | Some "J" -> Some down
        | _ -> None
        match tryIdx left with
        | Some "-" | Some "L" | Some "F" -> Some left
        | _ -> None
        match tryIdx right with
        | Some "-" | Some "7" | Some "J" -> Some right
        | _ -> None;
    ]
    let isValid (r,c) = r >=0 && r < rows && c >= 0 && c < columns 
    paths |> List.choose id |> List.filter isValid

let solve grid =
    let start = startingPosition grid 0 0
    
    let rec bfsLoop currCells explored depth =
        let next = currCells
                   |> Seq.map (findConnectingCells grid)
                   |> Seq.collect id
                   |> Seq.filter (flip Set.contains explored >> not)
                   |> List.ofSeq
        if List.isEmpty next then
            depth
        else
            let newExplored = Set.ofList currCells |> Set.union explored
            bfsLoop next newExplored (depth + 1)
            
    bfsLoop [start] Set.empty 0

let solve1 = parse >> solve
let solve2 = id

let printAnswer = printAnswersWithSameInputs1 solve1 solve2 example1 p1
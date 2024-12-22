module Day12

open System
open Utils
open AoC2023.Inputs.Day12

type Coord = { row: int; col: int }
type Cell = { pos: Coord; value: string }
type Region = { value: string; cells: Set<Cell> }
type Board = {
    allCells: Map<Coord, Cell>
    visitedCells: Set<Cell>
    remainingCells: Cell list
    regions: Region list
}

let neighbors (coord: Coord) : Coord list = 
    [
        { row = coord.row - 1; col = coord.col }
        { row = coord.row + 1; col = coord.col }
        { row = coord.row; col = coord.col - 1 }
        { row = coord.row; col = coord.col + 1 }
    ]
    
let isValidCell board cell = Map.containsKey cell.pos board.allCells

let area = _.cells >> Set.toList >> List.length

let perimeterCoords board region =
    region.cells
    |> Set.toList
    |> List.collect (fun cell -> neighbors cell.pos)
    //|> List.distinct
    |> List.filter (fun coord -> 
        match Map.tryFind coord board.allCells with
        | Some cell -> cell.value <> region.value
        | None -> true
    )

// find distinct neighbors of cells that are not the same value 
let rec perimeter board region =
    perimeterCoords board region
    |> List.length
    
let price board region =
    let area = (area region)
    let perimeter = (perimeter board region)
    area * perimeter, area, perimeter, region.value
    
let findRegions board =
    let rec findRegion' (region: Region) (nextCells: Cell list) (visitedCells: Set<Cell>) (board: Board) : Region =
        match nextCells with
        | [] -> region
        | cell::rest ->
            let nextNeighbors =
                neighbors cell.pos
                |> List.choose ((flip Map.tryFind) board.allCells)
                |> List.filter (((flip Set.contains) visitedCells) >> not)
                |> List.filter (_.value >> ((=) cell.value))
            let region = { region with cells = Set.add cell region.cells }
            let nextCells = rest @ nextNeighbors |> List.except region.cells
            let visitedCells = Set.add cell visitedCells
            findRegion' region nextCells visitedCells board
            
    let rec findRegions' board =
        match board.remainingCells with
        | [] -> board
        | cell :: rest ->
            let region = findRegion' { value = cell.value; cells = Set.empty } [cell] Set.empty board
            let board = { board with
                            regions = region :: board.regions
                            visitedCells = Set.union board.visitedCells region.cells
                            remainingCells = List.except region.cells board.remainingCells
                        }
            findRegions' board
    findRegions' board
    
    
let cellsToBoard cells =
    let allCells = cells |> List.map (fun cell -> cell.pos, cell) |> Map.ofList
    {
        allCells = allCells
        visitedCells = Set.empty
        remainingCells = cells
        regions = []
    }
    
let parse =
    charactersAs2dArray
    >> mapChar2dArray
           (fun row col value -> { pos = { row = row;  col = col }; value = value })
    >> cellsToBoard
    
let solve priceFn board = 
    let board = findRegions board
    let prices = Seq.map (priceFn board) board.regions
    prices
    |> tapValues
    |> Seq.sumBy (fun (p,_,_,_) -> p)

let parseAndSolve = parse >> solve price

let e1 = parse example1a
let print1 =
    // Console.WriteLine(parseAndSolve example1a)
    // Console.WriteLine(parseAndSolve example1b)
    // Console.WriteLine(parseAndSolve example1c)
    // Console.WriteLine(parseAndSolve p1)
    Console.WriteLine("")
    ()
  
  
// perimeter cells are those that have a neighbor that doesnt match the value
// get perimeter cells
// walk each cell in series, counting how many "turns" occur


type Direction = Up | Left | Down | Right

let walkDirection (cell: Cell) (direction: Direction) : Coord =
    match direction with
    | Up -> { row = cell.pos.row - 1; col = cell.pos.col }
    | Left -> { row = cell.pos.row; col = cell.pos.col - 1 }
    | Down -> { row = cell.pos.row + 1; col = cell.pos.col }
    | Right -> { row = cell.pos.row; col = cell.pos.col + 1 }

//clockwise
let clockwiseFrom direction =
    match direction with
    | Up -> [Up; Right; Down; Left]
    | Right -> [Right; Down; Left; Up]
    | Down -> [Down; Left; Up; Right]
    | Left -> [Left; Up; Right; Down]

let cellsOnPerimeter board region =
    region.cells
    |> Set.toList
    |> List.filter (fun cell ->
        neighbors cell.pos
        |> List.exists (fun coord -> 
            match Map.tryFind coord board.allCells with
            | Some neighbor -> neighbor.value <> region.value
            | None -> true
        ))
    
//
// let sides board region =
//     let perimeterCells = cellsOnPerimeter board region |> Set.ofList
//     let startingCell = perimeterCells |> Set.toSeq |> Seq.head 
//     
//     let rec sides' turns (startingDir: Direction option) (cell: Cell) (directionsToTry: Direction list) (visited: Set<Cell>) : int =
//         let unvisitedCells = Set.difference visited perimeterCells |> Set.toList
//         match startingCell = cell, unvisitedCells, startingDir, directionsToTry with
//         // we arrived back to the starting cell and are pointing the same direction we started in
//         | true, [], Some startingDir, currDir::_ when startingDir = currDir -> turns - 1
//         // we never moved but also never found a neighbor. Edge case for single cell regions
//         | true, [], None, [] -> turns
//         | _,_,_, [] -> failwith "somethings wrong, we rotated and didnt find the starting cell"
//         | _, _, _, currDir::remainingDirs ->
//             let inPerimeter = (flip Set.contains) perimeterCells
//             let nextNeighborCell =
//                 walkDirection cell currDir
//                 |> (flip Map.tryFind board.allCells) 
//                 // limit to only perimeter cells to avoid walking inside the region
//                 |> Option.filter inPerimeter
//                 
//             match nextNeighborCell with
//             | None ->
//                 // didnt find a neighbor, so turn and look again
//                 sides' (turns + 1) startingDir cell remainingDirs visited
//             | Some nextCell ->
//                 // found a neighbor, so move to the neighbor and recurse going in the same direction
//                 let startingDir = Some (Option.defaultValue currDir startingDir) 
//                 sides' turns startingDir nextCell (clockwiseFrom currDir) (Set.add nextCell visited)
//                 
//            
//     sides' 0 None startingCell (clockwiseFrom Up) (Set.ofList [startingCell])
    

//AAB
//ACC


// ..
// .#

let isCorner aDir bDir board region cell =
    let checkIsDifferentValue = walkDirection cell
                                >> (flip Map.tryFind) board.allCells
                                >> Option.filter (_.value >> ((=) region.value))
                                >> Option.isNone
    
    [aDir; bDir] |> List.filter checkIsDifferentValue |> List.isEmpty

let isTopLeftCorner = isCorner Up Left
let isTopRightCorner = isCorner Up Right
let isBottomLeftCorner = isCorner Down Left
let isBottomRightCorner = isCorner Down Right

let countCellCorners board region cell =
    [isTopLeftCorner; isTopRightCorner; isBottomLeftCorner; isBottomRightCorner]
    |> List.filter (fun f -> f board region cell)
    |> List.length 
    
    
let corners board region =
    region.cells
    |> Set.toList
    |> List.sumBy (countCellCorners board region)
  
let price2 board region =
    let area = (area region)
    let corners = (corners board region)
    area * corners, area, corners, region.value
  
let parseAndSolve2 = parse >> solve price2

let e1a = parseAndSolve2 "AAB\nACC"
  

   
let print2 =
    // Console.WriteLine(parseAndSolve2 example1a)
    // Console.WriteLine(parseAndSolve2 example1b)
    // Console.WriteLine(parseAndSolve2 example1c)
    // Console.WriteLine(parseAndSolve2 example2a)
    // Console.WriteLine(parseAndSolve2 example2b)
    // Console.WriteLine(parseAndSolve2 p1)
    ()
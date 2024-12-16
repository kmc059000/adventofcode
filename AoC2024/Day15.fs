module Day15

open System
open Utils
open AoC2023.Inputs.Day15

type Coord = { row: int; col: int; }
type Direction = Left | Right | Up | Down
type Cell = Wall | Empty | Block | Fish | BlockLeft | BlockRight

type BoardState = {
    map: Cell array array
    currPos: Coord
    directions: Direction list
}

type MovingBoard = {
    board: BoardState
    moving: Coord list
}

let nextPos pos dir =
    match dir with
    | Left -> { row = pos.row; col = pos.col - 1 }
    | Right -> { row = pos.row; col = pos.col + 1 }
    | Up -> { row = pos.row - 1; col = pos.col }
    | Down -> { row = pos.row + 1; col = pos.col }

module P1 =
    let rec move pos dir board =
        let cell = board.map[pos.row].[pos.col]
        let nextPos = nextPos pos dir
        let nextCell = board.map[nextPos.row].[nextPos.col]
        match nextCell with
        // cant move
        | Wall -> board
        // can move
        | Empty ->
            board.map[nextPos.row].[nextPos.col] <- cell
            board.map[pos.row].[pos.col] <- Empty
            if cell = Fish
            then
                // if we moved a fish, we are ready for the next instruction
                { board with currPos = nextPos }
            else
                // otherwise we swap the positions and carry on and unroll the recursion
                board
        // recurse
        | Block ->
            let board = move nextPos dir board
            // detect if the board changed and recurse again if so. Should only happen once.
            if Empty = board.map[nextPos.row].[nextPos.col]
            then move pos dir board
            else board
        | Fish -> failwith "shouldn't happen"
        | BlockLeft -> failwith "shouldn't happen"
        | BlockRight -> failwith "shouldn't happen"
        
        

    let printBoard doPrint board =
        if doPrint then
            board.map |> Array.iter (fun row ->
                row |> Array.iter (fun cell ->
                    match cell with
                    | Wall -> Console.Write("#")
                    | Empty -> Console.Write(".")
                    | Block -> Console.Write("O")
                    | Fish -> Console.Write("@")
                    | BlockLeft -> failwith "todo"
                    | BlockRight -> failwith "todo"
                )
                Console.WriteLine("")
            )
            Console.WriteLine("")
            Console.WriteLine(board.directions)
            
            Console.WriteLine("")
            Console.WriteLine("")
        board

    let rec processMoves board =
        match board.directions with
        | [] -> board
        | dir :: rest ->
            let movedBoard = (move board.currPos dir board)
            printBoard false movedBoard |> ignore
            processMoves { movedBoard with directions = rest }

    let parseDirection c =
        match c with
        | '<' -> Some Left
        | '>' -> Some Right
        | '^' -> Some Up
        | 'v' -> Some Down
        | _ -> None
        
    let parseCell c =
        match c with
        | '#' -> Wall
        | '.' -> Empty
        | 'O' -> Block
        | '@' -> Fish
        | _ -> failwith "invalid cell"
        
    let parse str =
        let parts = splitInputByDoubleNewLines str
        let directions = parts[1].Replace("\n", "").ToCharArray() |> List.ofArray |> List.map parseDirection
        let map = parts[0].Split("\n") |> Array.map (fun s -> s.ToCharArray() |> Array.map parseCell)
        let currPos =
            let rec findPos row col =
                if map[row].[col] = Fish then { row = row; col = col }
                else
                    if col = Array.length map[0] - 1
                    then findPos (row + 1) 0
                    else findPos row (col + 1)
            findPos 0 0
        {
            map = map
            currPos = currPos
            directions = directions |> List.choose id
        }
     
    let score board =
        board.map |> Array.mapi (fun i row ->
            row |> Array.mapi (fun j cell ->
                match cell with
                | Block -> (i  * 100) + j
                | _ -> 0
            ) |> Array.sum
        ) |> Array.sum
        
    let solve = parse >> processMoves >> printBoard true >> score
    

let smallExample = "########
#..O.O.#
##@.O..#
#...O..#
#.#.O..#
#...O..#
#......#
########

<^^>>>vv<v>>v<<"

let print1 =
    //Console.WriteLine(P1.solve smallExample)
    //Console.WriteLine(P1.solve example1)
    //Console.WriteLine(P1.solve p1)
    ()
   
module P2 =
    let nextPositions pos dir board =
        let nextPos = nextPos pos dir
        let nextCell = board.map[nextPos.row].[nextPos.col]
        match nextCell with
        | Wall | Empty | Fish -> [pos]
        | BlockLeft -> [nextPos; { row = nextPos.row; col = nextPos.col + 1 }]
        | BlockRight -> [{ row = nextPos.row; col = nextPos.col - 1 }; nextPos]
        | Block -> failwith "?"

    type Block = {
        left: Coord
        right: Coord
    }
    
    let nextBlocks block dir board =
        let left = nextPos block.left dir
        let leftCell = board.map[left.row].[left.col]
        let right = nextPos block.right dir
        let rightCell = board.map[right.row].[right.col]
        match leftCell, rightCell with
        | BlockLeft, BlockRight ->
            // single block aligned
            [{left = left; right = right}]
        | BlockRight, BlockLeft ->
            // 2 blocks that are offset by 1
            [{left = { left with col = left.col - 1 }; right = left}
             {left = right; right = { right with col = right.col + 1 }}]
        | BlockRight, _ ->
            // 1 block offset on left
            [{left = { left with col = left.col - 1 }; right = left}]
        | _, BlockLeft ->
            // 1 block offset on right
            [{left = right; right = { right with col = right.col + 1 }}]
        | _ -> []
        //| _ -> failwith "??"
        
    let nextPosForBlock block dir board =
        [block.left; block.right]
        |> List.collect (fun pos -> nextPositions pos dir board)
        
    let cell board block =
        [board.map[block.left.row].[block.left.col]
         board.map[block.right.row].[block.right.col]]
    
    let nextCells dir board block =
        [
            nextPos block.left dir;
            nextPos block.right dir;
        ]
        |> List.map (fun pos -> board.map[pos.row].[pos.col])
    
    
    let nextLineOfBlocks blocks dir board =
        let nextBlocks =
                blocks
                |> List.collect (fun block -> nextBlocks block dir board)
                
        let nextCells = blocks |>
                        List.collect (nextCells dir board) |>
                        List.distinct
        
        nextBlocks, nextCells
    
    let rec moveBlockUpDown (currBlocks: Block list) dir board =
        match dir with
        | Left | Right -> failwith "todo"
        | Up | Down ->
            let nextBlocks, nextCells = nextLineOfBlocks currBlocks dir board        
            match nextCells with
            | [Empty] ->
                // move all blocks to the next position
                currBlocks |> List.iter (fun block ->
                    let nextLeft = nextPos block.left dir
                    let nextRight = nextPos block.right dir
                    
                    board.map[nextLeft.row].[nextLeft.col] <- BlockLeft
                    board.map[nextRight.row].[nextRight.col] <- BlockRight
                    board.map[block.left.row].[block.left.col] <- Empty
                    board.map[block.right.row].[block.right.col] <- Empty
                )
                
                board 
            | _ when nextCells |> List.exists (fun c -> c = Wall) ->
                // cant move, so halt and return the board as is.
                board
            | _ ->
                if List.tryHead nextBlocks |> Option.isNone then board
                else                   
                    let board = moveBlockUpDown nextBlocks dir board
                    
                    let _, newNextCells = nextLineOfBlocks currBlocks dir board
                    
                    // detect if the board changed and recurse again if so. Should only happen once.
                    if not (List.exists (fun c -> c <> Empty) newNextCells)
                    then
                        moveBlockUpDown currBlocks dir board
                    else board
                    
            
            // collect all next positions
            // if _all_ are empty, proceed on moving all collected blocks to the next position
            // if any are walls, halt
            // if any are blocks, add them to the list and recurse
            
    
    let rec move pos dir board =
        let cell = board.map[pos.row].[pos.col]
        let nextPos = nextPos pos dir
        let nextCell = board.map[nextPos.row].[nextPos.col]
        match nextCell with
        // cant move
        | Wall -> board
        // can move
        | Empty ->
            board.map[nextPos.row].[nextPos.col] <- cell
            board.map[pos.row].[pos.col] <- Empty
            if cell = Fish
            then
                // if we moved a fish, we are ready for the next instruction
                { board with currPos = nextPos }
            else
                // otherwise we swap the positions and carry on and unroll the recursion
                board
        // recurse
        | BlockLeft | BlockRight ->
            if dir = Left || dir = Right then
                // blocks only apply up/down. If left/right, do what P1 does.
                let board = move nextPos dir board
                // detect if the board changed and recurse again if so. Should only happen once.
                if Empty = board.map[nextPos.row].[nextPos.col]
                then move pos dir board
                else board
            else
                let posLeft, posRight =
                    match nextCell with
                    | BlockLeft -> (nextPos, { row = nextPos.row; col = nextPos.col + 1 })
                    | BlockRight -> ({ row = nextPos.row; col = nextPos.col - 1 }, nextPos)
                    | _ -> failwith "shouldn't happen"
                let board = moveBlockUpDown [{ left = posLeft; right = posRight }] dir board
                if Empty = board.map[nextPos.row].[nextPos.col]
                then
                    // move the fish, then udate pos
                    let board = move pos dir board
                    { board with currPos = nextPos }
                else board
        | Fish -> failwith "shouldn't happen"
        | Block -> failwith "shouldn't happen"
        
        

    let shouldPrint i =
        i % 10000 = 0 && i <= 18000 
    
    let printBoard doPrint board =
        if doPrint && (shouldPrint (List.length board.directions)) then
            Console.Clear()
            board.map |> Array.iter (fun row ->
                row |> Array.iter (fun cell ->
                    match cell with
                    | Wall -> Console.Write("#")
                    | Empty -> Console.Write(".")
                    | Block -> Console.Write("O")
                    | Fish ->
                        Console.BackgroundColor <- ConsoleColor.DarkRed
                        Console.Write("@")
                        Console.ResetColor()
                    | BlockLeft -> Console.Write("[")
                    | BlockRight -> Console.Write("]")
                )
                Console.WriteLine("")
            )
            Console.WriteLine("")
            Console.WriteLine(board.directions)
            Console.WriteLine(List.length board.directions)
            
            Console.WriteLine("")
            Console.WriteLine("")
            System.Threading.Thread.Sleep(750)
        board

    let rec processMoves board =
        match board.directions with
        | [] -> board
        | dir :: rest ->
            let movedBoard = (move board.currPos dir board)
            printBoard true movedBoard |> ignore
            processMoves { movedBoard with directions = rest }

    let parseDirection c =
        match c with
        | '<' -> Some Left
        | '>' -> Some Right
        | '^' -> Some Up
        | 'v' -> Some Down
        | _ -> None
        
    let parseCell c =
        match c with
        | '#' -> [Wall; Wall]
        | '.' -> [Empty; Empty]
        | 'O' -> [BlockLeft; BlockRight]
        | '@' -> [Fish; Empty]
        | _ -> failwith "invalid cell"
        |> List.toArray
        
    let parse str =
        let parts = splitInputByDoubleNewLines str
        let directions = parts[1].Replace("\n", "").ToCharArray() |> List.ofArray |> List.map parseDirection
        let map = parts[0].Split("\n") |> Array.map (fun s -> s.ToCharArray() |> Array.collect parseCell)
        let currPos =
            let rec findPos row col =
                if map[row].[col] = Fish then { row = row; col = col }
                else
                    if col = Array.length map[0] - 1
                    then findPos (row + 1) 0
                    else findPos row (col + 1)
            findPos 0 0
        {
            map = map
            currPos = currPos
            directions = directions |> List.choose id
        }
     
    let score board =
        board.map |> Array.mapi (fun i row ->
            row |> Array.mapi (fun j cell ->
                match cell with
                | BlockLeft -> (i  * 100) + j
                | _ -> 0
            ) |> Array.sum
        ) |> Array.sum
        
    let solve = parse >> processMoves >> printBoard true >> score
    
let smallExample2 = "#######
#...#.#
#.....#
#..OO@#
#..O..#
#.....#
#######

<vv<<^^<<^^"
   
let print2 =
    //Console.WriteLine(P2.solve smallExample2)
    //Console.WriteLine(P2.solve example1)
    Console.WriteLine(P2.solve p1)
    ()
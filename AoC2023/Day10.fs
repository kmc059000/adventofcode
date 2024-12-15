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
        let newExplored = Set.ofList currCells |> Set.union explored
        if List.isEmpty next then
            depth, newExplored
        else
            bfsLoop next newExplored (depth + 1)
            
    bfsLoop [start] (Set.ofList [start]) 0




let solve1 = parse >> solve >> fst


//if items on path lef/right or top/bottom are unbalanced, then outside.

let itemsInRow (grid : string array array) (row,col) =
    let cols = grid[0].Length
    [for c in 0..cols do
         yield (row, c)]
    |> List.except [(row,col)]
    |> List.partition (fun (r,c) -> c < col)

let itemsInCol (grid : string array array) (row,col) =
    let rows = grid.Length
    [for r in 0..rows do
         yield (r, col)]
    |> List.except [(row,col)]
    |> List.partition (fun (r,c) -> r < row)
    
let isLeftRightSymbol char =
    match char with
    | "|" | "L" | "J" | "7" | "F" | "S"  -> true
    | _ -> false

let isUpDownSymbol char =
    match char with
    | "-" | "L" | "J" | "7" | "F" | "S"  -> true
    | _ -> false
    
let isInside (grid : string array array) (inPath : Set<int*int>) (row,col) =
    let removeItemsInEdge = TupleExtras.mapBoth (List.filter (flip Set.contains inPath))
    let mapToChars = TupleExtras.mapBoth (List.map (fun (row,col) -> grid[row][col]))
    
    let rowBefore, rowAfter = itemsInRow grid (row,col) |> removeItemsInEdge |> mapToChars
    let colBefore, colAfter = itemsInCol grid (row,col) |> removeItemsInEdge |> mapToChars
    
    let rec countCrossesForRowRight chars isInside =
        match chars with
        | [] -> isInside
        | c::rest ->
            match c with
            | "|" -> countCrossesForRowRight rest (not isInside)
            | "-" -> countCrossesForRowRight rest isInside
            | "L" -> countCrossesForRowRight rest isInside
            | "7" -> countCrossesForRowRight rest (not isInside)
            | "F" -> countCrossesForRowRight rest isInside
            | "J" -> countCrossesForRowRight rest (not isInside)
            | _ -> failwith "bad char"
    
    let rec countCrossesForRowLeft chars isInside =
        match chars with
        | [] -> isInside
        | c::rest ->
            match c with
            | "|" -> countCrossesForRowLeft rest (not isInside)
            | "-" -> countCrossesForRowLeft rest isInside
            | "L" -> countCrossesForRowLeft rest (not isInside)
            | "7" -> countCrossesForRowLeft rest isInside
            | "F" -> countCrossesForRowLeft rest (not isInside)
            | "J" -> countCrossesForRowLeft rest isInside
            | _ -> failwith "bad char"
    
    
    
    
    
    
    let reduceLeftRightAngles chars =
        chars
        |> String.concat ""
        |> StringExtras.replace "L7" "|"
        //|> StringExtras.replace "7L" "|"
        |> StringExtras.replace "FJ" "|"
        //|> StringExtras.replace "JF" "|"
        |> StringExtras.characters
        |> List.ofArray
    
    let reduceUpDownAngles chars =
        chars
        |> String.concat ""
        |> StringExtras.replace "FL" "-"
        //|> StringExtras.replace "7L" "|"
        |> StringExtras.replace "7J" "-"
        //|> StringExtras.replace "JF" "|"
        |> StringExtras.characters
        |> List.ofArray
    
    let countCrosses detectCross = List.map reduceLeftRightAngles >> detectCross grid
    let crossesUpDown = countCrosses isUpDownSymbol
    let crossesLeftRight = countCrosses isLeftRightSymbol
    
    let isOdd x = x % 2 = 1
    
    let counts =
        [
            crossesLeftRight rowBefore
            crossesLeftRight rowAfter
            crossesUpDown colBefore
            crossesUpDown colAfter
        ]
        
    counts
    |> List.map List.length
    |> List.map isOdd
    |> List.reduce (||)
    
let findAllPossible (grid : string array array) inPath =
    let rows = grid.Length
    let cols = grid[0].Length
    
    let allCandidates =
         [for r in 0..(rows-1) do
             [for c in 0..(cols-1) do
                  if Set.contains (r,c) inPath then None else Some (r,c)
                  ]]
        |> List.collect id
        |> List.choose id
    
    let allInside =
        allCandidates
        |> List.filter (isInside grid inPath)
        
    let c = allInside |> List.length
    c
       


        
let solve2 input =
    let parsed = parse input
    let _,inPath = solve parsed
    findAllPossible parsed inPath


let parsed = parse example2_1
let _,inPath = solve parsed
let x = isInside parsed inPath (5, 5)

let printAnswer = printAnswersWithSameInputs2Example solve1 solve2 example2_2 p1
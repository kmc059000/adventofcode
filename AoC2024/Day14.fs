module Day14

open System
open System.Text.RegularExpressions
open Utils
open AoC2023.Inputs.Day14

let ints str =
    let r = Regex(@"-?\d+")
    r.Matches(str) |> Seq.map (_.Value >> int) |> Seq.toList

let parseRobot line =
    let vals = ints line
    (vals.[0], vals.[1]),(vals.[2], vals.[3])
    
let parse input = input |> splitInputByNewLinesList |> List.map parseRobot
    
let stepRobot maxRow maxCol steps robot =
    let (col, row), (dCol, dRow) = robot
    let newRow = row + dRow * steps
    let newCol = col + dCol * steps
    let modRow = newRow % maxRow
    let modCol = newCol % maxCol
    let modRow = if modRow < 0 then modRow + maxRow else modRow
    let modCol = if modCol < 0 then modCol + maxCol else modCol
    (modCol, modRow)
    
let stepRobots maxRow maxCol steps = List.map (stepRobot maxRow maxCol steps)

let quadrant maxRow maxCol (col, row) =
    let midRow = maxRow / 2
    let midCol = maxCol / 2
    
    if col = midCol || row = midRow then
        (col, row), -1
    else
        match col < midCol, row < midRow with
        | false, false -> (col, row), 0
        | false, true -> (col, row), 1
        | true, false -> (col, row), 2
        | true, true -> (col, row), 3
        
let solve maxRow maxCol steps robots =
    robots
    |> stepRobots  maxRow maxCol steps
    |> List.map (quadrant maxRow maxCol)
    //|> tapValues |> List.ofSeq
    |> List.groupBy snd
    |> List.map (fun (k, v) -> k, List.length v)
    |> tapValues |> List.ofSeq
    |> List.filter (fun (k, v) -> k <> -1)
    |> List.map snd
    |> List.fold (*) 1
        
let parseAndSolveSample sample =
    sample |> parse |> solve 7 11 100
    
let parseAndSolve input =
    input |> parse |> solve 103 101 100
        
let print1 =
    Console.WriteLine(parseAndSolveSample example1)
    Console.WriteLine(parseAndSolve p1)
    ()
   
let printRobots maxRow maxCol (robots: (int*int) list) =
    let map = robots |> List.groupBy id |> List.map (fun (k, v) -> k, List.length v) |> Map.ofList
    for row in 0..maxRow-1 do
        for col in 0..maxCol-1 do
            let cell =
                match Map.tryFind (col, row) map with
                | None -> "."
                | Some l -> string l 
            Console.Write(cell)
        Console.WriteLine("")
    robots
   
let solve2 maxRow maxCol steps robots =
    robots
    |> stepRobots  maxRow maxCol steps
    //|> printRobots maxRow maxCol
   
let scan maxRow maxCol steps robots =
    let mutable minDistance = Int32.MaxValue
    let mutable avgDistance = 0
    let mutable minStep = -1
    for i in 1..steps do
        let solved = solve2 maxRow maxCol i robots
        let (centerCol, centerRow) = (solved |> List.averageBy (fst >> double)), solved |> List.averageBy (snd >> double)
        let distance = solved |> List.map (fun (col, row) -> manhattanDistanceDouble ((double col), (double row)) (double centerCol, double centerRow)) |> List.sum |> int
        
        avgDistance <- (avgDistance * i + distance) / (i)
        
        if distance < minDistance && i > 150 then
            Console.WriteLine("Step : " + i.ToString() + " Distance: " + distance.ToString() + " Avg: " + avgDistance.ToString())
            minStep <- i
            minDistance <- distance
            printRobots maxRow maxCol solved |> ignore
        |> ignore
            
    minStep
    
    
        
    
   
let parseAndSolveSample2 input =
    input |> parse |> solve2  7 11 100
   
let parseAndSolve2 input steps =
    input |> parse |> solve2 103 101 steps
    
let parseAndScan2 input steps =
    input |> parse |> scan 103 101 steps
   
let test step total =
    let s = seq { for i in 0..total -> i * step }
    for i in s do
        Console.WriteLine($"Step: {i}")
        parseAndSolve2 p1 i
        Console.WriteLine()
        Console.WriteLine()
        Console.WriteLine()
        Console.WriteLine()
        Console.WriteLine()
   
let print2 =    
    Console.WriteLine(parseAndScan2 p1 100000)
    ()
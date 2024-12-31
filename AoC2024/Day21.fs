module Day21

open System
open Utils
open AoC2023.Inputs.Day21

let numPadShortestPaths =
    [
        ('A', 'A'), ""
        ('A', '0'), "<"
        ('A', '1'), "^<<"
        ('A', '2'), "^<"
        ('A', '3'), "^"
        ('A', '4'), "^^<<"
        ('A', '5'), "^^<"
        ('A', '6'), "^^"
        ('A', '7'), "^^^<<"
        ('A', '8'), "^^^<"
        ('A', '9'), "^^^"
        
        ('3', 'A'), "v"
        ('3', '0'), "v<"
        ('3', '1'), "<<"
        ('3', '2'), "<"
        ('3', '3'), ""
        ('3', '4'), "^<<"
        ('3', '5'), "^<"
        ('3', '6'), "^"
        ('3', '7'), "^^<<"
        ('3', '8'), "^^<"
        ('3', '9'), "^^"
        
        ('6', 'A'), "vv"
        ('6', '0'), "vv<"
        ('6', '1'), "v<<"
        ('6', '2'), "v<"
        ('6', '3'), "v"
        ('6', '4'), "<<"
        ('6', '5'), "<"
        ('6', '6'), ""
        ('6', '7'), "^<<"
        ('6', '8'), "^<"
        ('6', '9'), "^"
        
        ('9', 'A'), "vvv"
        ('9', '0'), "vvv<"
        ('9', '1'), "vv<<"
        ('9', '2'), "vv<"
        ('9', '3'), "vv"
        ('9', '4'), "v<<"
        ('9', '5'), "v<"
        ('9', '6'), "v"
        ('9', '7'), "<<"
        ('9', '8'), "<"
        ('9', '9'), ""
        
        ('0', '0'), ""
        ('0', 'A'), ">"
        ('0', '1'), "^<"
        ('0', '2'), "^"
        ('0', '3'), "^>"
        ('0', '4'), "^^<"
        ('0', '5'), "^^"
        ('0', '6'), "^^>"
        ('0', '7'), "^^^<"
        ('0', '8'), "^^^"
        ('0', '9'), "^^^>"
        
        ('2', 'A'), "v>"
        ('2', '0'), "v"
        ('2', '1'), "<"
        ('2', '2'), ""
        ('2', '3'), ">"
        ('2', '4'), "^<"
        ('2', '5'), "^"
        ('2', '6'), "^>"
        ('2', '7'), "^^<"
        ('2', '8'), "^^"
        ('2', '9'), "^^>"
        
        ('5', 'A'), "vv>"
        ('5', '0'), "vv"
        ('5', '1'), "v<"
        ('5', '2'), "v"
        ('5', '3'), "v>"
        ('5', '4'), "<"
        ('5', '5'), ""
        ('5', '6'), ">"
        ('5', '7'), "^<"
        ('5', '8'), "^"
        ('5', '9'), "^>"
        
        ('8', 'A'), "vvv>"
        ('8', '0'), "vvv"
        ('8', '1'), "vv<"
        ('8', '2'), "vv"
        ('8', '3'), "vv>"
        ('8', '5'), "v"
        ('8', '4'), "v<"
        ('8', '6'), "v>"
        ('8', '7'), "<"
        ('8', '8'), ""
        ('8', '9'), ">"
        
        ('1', 'A'), ">>v"
        ('1', '0'), ">v"
        ('1', '1'), ""
        ('1', '2'), ">"
        ('1', '3'), ">>"
        ('1', '4'), "^"
        ('1', '5'), "^>"
        ('1', '6'), "^>>"
        ('1', '7'), "^^"
        ('1', '8'), "^^>"
        ('1', '9'), "^^>>"
        
        ('4', 'A'), ">>vv"
        ('4', '0'), ">vv"
        ('4', '1'), "v"
        ('4', '2'), "v>"
        ('4', '3'), "v>>"
        ('4', '4'), ""
        ('4', '5'), ">"
        ('4', '6'), ">>"
        ('4', '7'), "^"
        ('4', '8'), "^>"
        ('4', '9'), "^>>"
        
        ('7', 'A'), ">>vvv"
        ('7', '0'), ">vvv"
        ('7', '1'), "vv"
        ('7', '2'), "vv>"
        ('7', '3'), "vv>>"
        ('7', '4'), "v"
        ('7', '5'), "v>"
        ('7', '6'), "v>>"
        ('7', '7'), ""
        ('7', '8'), ">"
        ('7', '9'), ">>"        
    ]
    |> Map.ofList
    
let arrowShortestPaths =
    [
        ('^', 'A'), ">"
        ('^', '<'), "v<"
        ('^', 'v'), "v"
        ('^', '>'), "v>"
        ('^', '^'), ""
        
        ('A', '^'), "<"
        ('A', '<'), "v<<"
        ('A', 'v'), "v<"
        ('A', '>'), "v"
        ('A', 'A'), ""
        
        ('v', 'A'), "^>"
        ('v', '<'), "<"
        ('v', '^'), "^"
        ('v', '>'), ">"
        ('v', 'v'), ""
        
        ('<', 'A'), ">>^"
        ('<', 'v'), ">"
        ('<', '^'), ">^"
        ('<', '>'), ">>"
        ('<', '<'), ""
        
        ('>', 'A'), "^"
        ('>', 'v'), "<"
        ('>', '^'), "^<"
        ('>', '<'), "<<"
        ('>', '>'), ""
    ] |> Map.ofList
    
    
let codeToArrows code =
    printfn ""
    ("A" + code).ToCharArray()
    |> Seq.ofArray
    |> Seq.pairwise
    |> Seq.map (flip Map.find numPadShortestPaths)
    |> Seq.map (fun x -> x + "A")
    |> tapValues
    |> String.concat ""
    |> tapValue 

let arrowsToArrows code =
    printfn ""
    ("A" + code).ToCharArray()
    |> Seq.ofArray
    |> Seq.pairwise
    |> Seq.map (flip Map.find arrowShortestPaths)
    |> Seq.map (fun x -> x + "A")
    |> tapValues
    |> String.concat ""
    |> tapValue

let path = codeToArrows >> arrowsToArrows >> arrowsToArrows

//path "379A" |> printfn "%s"

let totalShortestPath = path >> _.Length

let score code =
    let num = readInts code |> List.head
    let score = totalShortestPath code
    score, num
    
let parseAndSolve = splitInputByNewLinesList >> List.map score >> tapValues >> List.ofSeq >> List.map (fun (a,b) -> a * b) >> List.sum

totalShortestPath "379A" |> printfn "%i"
printfn "<v<A>>^AvA^A<vA<AA>>^AAvA<^A>AAvA^A<vA>^AA<A>A<v<A>A>^AAAvA<^A>A"

let print1 =
    //Console.WriteLine(parseAndSolve example1)
    Console.WriteLine("")
    ()
   
let print2 =
    Console.WriteLine("")
    Console.WriteLine("")
    ()
    
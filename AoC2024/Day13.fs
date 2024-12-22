module Day13

open System
open System.Text.RegularExpressions
open FSharp.Stats.Interpolation.CubicSpline
open Utils
open AoC2023.Inputs.Day13

let simplee1= "Button A: X+38, Y+79
Button B: X+28, Y+13
Prize: X=3648, Y=4148"


let regex = Regex("\d+")

let parseLine (line: string) =
    let value = regex.Matches(line)
    [|int64 value[0].Value; int64 value[1].Value|]
    
let solveGroup inc (group: string list) =
    let parsedLines = group |> List.map parseLine |> Array.ofList
    
    parsedLines[2][0] <- parsedLines[2][0] + inc
    parsedLines[2][1] <- parsedLines[2][1] + inc
    
    let aMatr = [|
        [|decimal (parsedLines[0][0]); decimal (parsedLines[1][0])|]
        [|decimal (parsedLines[0][1]); decimal (parsedLines[1][1])|]
    |]
    
    let bMatr = [|
        decimal (parsedLines[2][0]);
        decimal (parsedLines[2][1])
    |]
    
    let detA = (aMatr[0][0]*aMatr[1][1] - aMatr[0][1]*aMatr[1][0])
    let aInv = [
        [aMatr[1][1]; -aMatr[0][1]];
        [-aMatr[1][0]; aMatr[0][0]]
    ]
    
    let solvedX = Seq.zip aInv[0] bMatr |> Seq.sumBy (fun (a, b) -> a * b)
    let solvedY = Seq.zip aInv[1] bMatr |> Seq.sumBy (fun (a, b) -> a * b)
    
    let solvedX = solvedX / detA |> int64
    let solvedY = solvedY / detA |> int64
    
    let compX = ((int64 parsedLines.[0].[0]) * solvedX + (int64 parsedLines.[1].[0]) * solvedY)
    let testX = compX = int64 parsedLines.[2].[0]
    let compY = ((int64 parsedLines.[0].[1]) * solvedX + (int64 parsedLines.[1].[1]) * solvedY)
    let testY = compY = int64 parsedLines.[2].[1]
    
    solvedX, solvedY, testX && testY
    
let solve mul =
    splitInputByDoubleNewLines
    >> List.ofArray
    >> List.map (splitInputByNewLinesList >> solveGroup mul)
    >> List.mapi (fun i (x, y, test) -> i, test, x, y, (x * 3L) + y)
    >> tapValues >> List.ofSeq
    >> List.filter (fun (_, test, _, _, _) -> test)
    >> List.sumBy (fun (_, _, x, y, score) -> score)
    


let print1 =
    Console.WriteLine(solve 0 example1)
    Console.WriteLine(solve 0 p1)
    ()
   
let print2 =
    Console.WriteLine(solve 10000000000000L example1)
    Console.WriteLine(solve 10000000000000L p1)
    ()
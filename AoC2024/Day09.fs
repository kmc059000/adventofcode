module Day09

open System
open Microsoft.FSharp.Core
open Utils
open AoC2023.Inputs.Day09


type Instruction = Empty of int | Filled of int
type Cell = Empty | Filled of int

type Disk = Map<int, Cell>

let parse (str: string) : Disk =
    let rawInstructions = Seq.map string str |> List.ofSeq
    let instructions =
        List.mapi (fun i x ->
            if i % 2 = 0 then
                Instruction.Filled (int x)
            else
                Instruction.Empty (int x)
            ) rawInstructions

    let fillWith id idx length disk fn  =
        let rec fillWith' idx length disk =
            if length = 0 then
                disk, idx
            else
                fillWith' (idx + 1) (length - 1) (Map.add idx (fn id) disk)
        fillWith' idx length disk

    let rec fillDisk (idNumber: int) (idx: int) (disk: Disk) (instructions: Instruction list) =
        match instructions with
        | [] -> disk
        | Instruction.Filled len::tail ->
            let filledDisk, newIdx = fillWith idNumber idx len disk Filled
            fillDisk (idNumber + 1) newIdx filledDisk tail
        | Instruction.Empty len::tail ->
            let filledDisk, newIdx = fillWith idNumber idx len disk (fun _ -> Empty)
            fillDisk idNumber newIdx filledDisk tail

    fillDisk 0 0 Map.empty instructions


let rec compact (disk: Disk) =
    let rec compact' leftIdx rightIdx disk =
        if leftIdx >= rightIdx then
            disk
        else
            let left = Map.find leftIdx disk
            let right = Map.find rightIdx disk

            match left, right with
            // end is empty, so iterate from the end
            | _, Empty -> compact' leftIdx (rightIdx - 1) disk
            // both spots are filled, cant fill so move from the left in
            | Filled _, Filled _ -> compact' (leftIdx + 1) rightIdx disk
            | Empty, Filled value ->
                // move from right idx to left id, and then recurse
                let newDisk =
                    disk
                    |> Map.add leftIdx (Filled value)
                    |> Map.add rightIdx Empty
                compact' (leftIdx + 1) (rightIdx - 1) newDisk



    compact' 0 ((Map.count disk ) - 1) disk


let checksum (disk: Disk) =
    [ for i in 0..Map.count disk - 1 -> Map.find i disk ]
    |> List.mapi (fun i cell ->
        match cell with
        | Empty -> 0
        | Filled x -> i * x)
    |> List.map uint64
    |> List.sum


let printDisk disk =
    let str =
        [ for i in 0..Map.count disk - 1 -> Map.find i disk ]
        |> List.map (function
            | Empty -> "."
            | Filled x -> string x)
        |> String.concat ""
    Console.WriteLine(str)
    disk

example1 |> parse |> compact |> printDisk |> checksum |> tapValue |> ignore

let solve = parse >> compact >> checksum

let print1 =
    Console.WriteLine(solve example1)
    Console.WriteLine(solve p1)
    ()
   
let print2 =
    Console.WriteLine("")
    Console.WriteLine("")
    ()
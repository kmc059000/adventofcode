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

//example1 |> parse |> compact |> printDisk |> checksum |> tapValue |> ignore

let solve = parse >> compact >> checksum

let print1 =
    //Console.WriteLine(solve example1)
    //Console.WriteLine(solve p1)
    ()

let rec compact2 (disk: Disk) =
    let spanLengths =
        disk
        |> Map.values
        |> List.ofSeq
        |> List.filter (function Filled _ -> true | _ -> false)
        |> List.map (function Filled value -> value | _ -> failwith "impossible")
        |> List.groupBy id
        |> List.map (fun (value, group) -> value, group.Length)
        |> Map.ofList

    let rec compact' leftIdx rightIdx (disk: Disk) =
        if rightIdx <= 0 then
            disk
        elif leftIdx >= rightIdx then
            let right = Map.find rightIdx disk
            tapValue ("giving up on " + string right)
            compact' 0 (rightIdx - 1) disk
        else
            let left = Map.find leftIdx disk
            let right = Map.find rightIdx disk

            match left, right with
            // end is empty, so iterate from the end
            | _, Empty -> compact' leftIdx (rightIdx - 1) disk
            // both spots are filled, cant fill so move from the left in
            | Filled _, Filled _ -> compact' (leftIdx + 1) rightIdx disk
            | Empty, Filled value ->
                //determine size of filled span
                // determine size of empty span
                // if size is enough, move, otherwise, move leftIdx to end of empty span
                let filledSize = spanLengths.[value]
                let emptySize =
                    seq { for i in leftIdx..Map.count disk - 1 -> Map.find i disk }
                    |> Seq.takeWhile (function
                        | Empty -> true
                        // if the empty overlaps the span that were moving, that is okay
                        | Filled _ -> false)
                     |> Seq.length
                if emptySize < filledSize then
                    // not big enough, so move leftIdx to the end of the empty sequence, and move rightIdx to the next file
                    compact' (leftIdx + emptySize) (rightIdx) disk
                else
                    //empty is big enough, so move all the filled items to the empty
                    // and then recurse
                    let spacesToFill = [ for i in leftIdx..leftIdx + filledSize - 1 -> i ]
                    let spacesToEmpty = [ for i in (rightIdx - filledSize + 1)..rightIdx -> i ]

                    let emptied = List.fold (fun acc i -> Map.add i Empty acc) disk spacesToEmpty
                    let newDisk = List.fold (fun acc i -> Map.add i (Filled value) acc) emptied spacesToFill

                    tapValue ("moved " + string value)

                    //printDisk newDisk

                    // start left from the beginning since there may be empty spaces that we can put things in
                    compact' 0 (rightIdx - filledSize) newDisk

    compact' 0 ((Map.count disk ) - 1) disk

//example1 |> parse |> printDisk |> compact2 |> printDisk |> checksum |> tapValue |> ignore

let solve2 = parse >> compact2 >> checksum

let print2 =
    //Console.WriteLine(solve2 example1)
    Console.WriteLine(solve2 p1)
    ()
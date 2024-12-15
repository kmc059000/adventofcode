module Day05

open System
open Utils
open AoC2023.Inputs.Day05


type Rule = { Left: int; Right: int; }

type Puzzle = {
    Rules : Rule list;
    Updates : int list list;
}

let parseOrder lst =
    { Left = List.item 0 lst; Right = List.item 1 lst }

let parse str =
    let sections = splitInputByDoubleNewLines str
    let orderings = sections[0] |> splitInputByNewLinesMapList (splitByMapList "|" int) |> List.map parseOrder
    let updates = sections[1] |> splitInputByNewLinesMapList (splitByMapList "," int)

    { Rules = orderings; Updates = updates }

let satisfiesRule ((updateLeft, updateRight) : int * int) (rule : Rule) =
    if updateLeft = rule.Left && updateRight = rule.Right then true
    else if updateLeft = rule.Right && updateRight = rule.Left then false
    else true

let satisfiesRules (rules : Rule list) (update : int * int) = List.forall (satisfiesRule update) rules

let isSorted input updateLine =
    updateLine
    |> List.pairwise
    |> List.forall (satisfiesRules input.Rules)

let middle (update : int list) = update.[(List.length update) / 2]

let solveAll input =
     input.Updates
     |> List.map (fun update -> (isSorted input update, middle update, update))
     //|> List.map tapValue
     |> List.filter (fun (isSorted, _, _) -> isSorted)
     |> List.sumBy (fun (_, num, _) -> num)


let parseAndSolve = parse >> solveAll


let swapIdx idx otherIdx lst =
    let mutable arr = lst |> Array.ofList

    if(otherIdx >= List.length lst) then
        lst
    else

    let temp = arr.[idx]
    arr.[idx] <- arr.[otherIdx]
    arr.[otherIdx] <- temp
    let newList = arr |> List.ofArray

    //Console.WriteLine("Swapping " + idx.ToString() + " with " + otherIdx.ToString() + " in " + String.Join(",", lst |> List.map string) + " -> " + String.Join(",", newList |> List.map string))

    newList

let applyRule update (rule : Rule) =
    let leftIdx = List.tryFindIndex ((=) rule.Left) update
    let rightIdx = List.tryFindIndex ((=) rule.Right) update
    match leftIdx, rightIdx with
    | None, _ -> update
    | _, None -> update
    | Some leftIdx, Some rightIdx when leftIdx = rightIdx + 1 -> swapIdx leftIdx rightIdx update
    | _ -> update



let resort input update =
    // do pairs, and repeatedly apply rules until sorted
    let mutable sortedLine = update
    while not (isSorted input sortedLine) do
        // loop over all rules, and apply them to the line one by one
        List.iter (fun rule -> sortedLine <- applyRule sortedLine rule) input.Rules

    sortedLine

let solveAll2 input =
     input.Updates
     |> List.map (fun update -> (isSorted input update, middle update, update))
     //|> List.map tapValue
     |> List.filter (fun (isSorted, _, _) -> not isSorted)
     |> List.map (fun (_, _, update) -> resort input update)
     |> List.map (fun update -> (isSorted input update, middle update, update))
     |> List.map tapValue
     |> List.filter (fun (isSorted, _, _) -> isSorted)
     |> List.sumBy (fun (_, num, _) -> num)

let parseAndSolve2 = parse >> solveAll2


let print1 =
    Console.WriteLine(parseAndSolve example1)
    Console.WriteLine(parseAndSolve p1)
    ()
   
let print2 =
    Console.WriteLine(parseAndSolve2 example1)
    Console.WriteLine(parseAndSolve2 p1)
    ()
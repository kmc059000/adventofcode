module Day11

open System
open Utils
open AoC2023.Inputs.Day11

// let memoize f =
//     let cache = System.Collections.Concurrent.ConcurrentDictionary()
//     fun x -> cache.GetOrAdd(x, lazy f x).Value
//
// let memoizedIToString = memoize (fun i -> i.ToString().Length)

module P1 =
    let calcStone (i: uint64) =    
        if i = 0UL then [ 1UL ]
        elif i = 1UL then [2024UL]
        elif i.ToString().Length % 2 = 0 then
            let str = i.ToString()
            [ str.Substring(0, str.Length / 2 );  str.Substring(str.Length / 2 ) ] |> List.map uint64
        else [ i * 2024UL ]
        
    let blink = List.collect calcStone

    let rec blinkTimes times (stones: uint64 list) =
        //tapValue (joinInts " " stones) |> ignore
        tapValue times |> ignore
        tapValue (List.length stones) |> ignore
        if times = 0 then stones
        else blinkTimes (times - 1) (blink stones)
        


    let parse = splitBySpaces >> List.ofArray >> List.map uint64

    let solve times = parse >> blinkTimes times >> List.length

let print1 =
    //Console.WriteLine(solve 7 example1)
    // Console.WriteLine(P1.solve 6 "125 17")
    // Console.WriteLine(P1.solve 25 "125 17")
    // Console.WriteLine(P1.solve 25 p1)
    ()
    
module P2 =
    type StoneGroup = { Value: uint64; StoneCount: uint64 }
    
    let calcStoneGroup group =
        group.Value
        |> P1.calcStone
        |> List.map (fun newVal -> { Value = newVal; StoneCount = group.StoneCount })
    
    let blink (stoneGroups: StoneGroup list) =
        stoneGroups
        |> List.collect calcStoneGroup
        |> List.groupBy _.Value
        |> List.map (fun (value, lst) -> { Value = value; StoneCount = List.sumBy _.StoneCount lst } )

    let rec blinkTimes times (stoneGroups: StoneGroup list) =
        tapValue times |> ignore
        tapValue (List.sumBy _.StoneCount stoneGroups) |> ignore
        if times = 0 then (List.sumBy _.StoneCount stoneGroups)
        else blinkTimes (times - 1) (blink stoneGroups)
        
    let parseToGroups uints =
      uints
      |> List.groupBy id
      |> List.map (fun (value, lst) -> { Value = value; StoneCount = List.length lst |> uint64 } )
        
    let solve times = P1.parse >> parseToGroups >> blinkTimes times
   
let print2 =
    Console.WriteLine(P2.solve 75 p1)
    Console.WriteLine("")
    ()
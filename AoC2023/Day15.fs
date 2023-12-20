module Day15
open Utils
open AoC2023.Inputs.Day15


// let rec hashStr (chars : char list) (currValue : int) =
//     match chars with
//     | [] -> currValue
//     | char::cs ->
//         let asciiCode = int char
//         let nextValue1 = (currValue + asciiCode)
//         let nextValue2 = nextValue1 * 17
//         let nextValue3 = nextValue2 % 256
//         hashStr cs nextValue3

let hashStr (str : string) =
    str
    |> StringExtras.characters
    |> List.ofSeq
    |> Seq.fold (fun (acc : int) (curr : string) ->
        let asciiCode = int curr[0]
        let nextValue1 = (acc + asciiCode)
        let nextValue2 = nextValue1 * 17
        let nextValue3 = nextValue2 % 256
        nextValue3
        ) 0

printfn $"""%i{(hashStr "HASH")}""" 

let solve1 = splitBy "," >> (Array.sumBy hashStr)

let solve2 = id
    

let printAnswer = printAnswersWithSameInputs1 solve1 solve2 example1 p1
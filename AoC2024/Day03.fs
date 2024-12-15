module Day03

open System
open System.Linq
open System.Text.RegularExpressions
open Utils
open AoC2023.Inputs.Day03

let example1 = "xmul(2,4)%&mul[3,7]!@^do_not_mul(5,5)+mul(32,64]then(mul(11,8)mul(8,5))"

let regex = Regex("mul\((\d+),(\d+)\)", RegexOptions.Compiled ||| RegexOptions.IgnoreCase)

let compute = regex.Matches >> Seq.map (fun m -> int m.Groups.[1].Value * int m.Groups.[2].Value) >> Seq.sum

let print1 =
    Console.WriteLine(compute example1)
    Console.WriteLine(compute p1)
    ()

let example2 = "xmul(2,4)&mul[3,7]!^don't()_mul(5,5)+mul(32,64](mul(11,8)undo()?mul(8,5))"

let regex2 = Regex("(mul\((\d+),(\d+)\))|(do\(\))|(don't\(\))", RegexOptions.Compiled ||| RegexOptions.IgnoreCase)

type Result = { Enabled: bool; Acc: int; }

let compute2 str =
    regex2.Matches(str).Cast<Match>()
    |> Seq.fold (fun acc curr ->
        match curr.Groups[0].Value, acc.Enabled with
        | "do()", _ -> { acc with Enabled = true }
        | "don't()", _ -> { acc with Enabled = false }
        | _, false -> acc
        | _, true -> { acc with Acc = acc.Acc + int curr.Groups.[2].Value * int curr.Groups.[3].Value }
    ) { Enabled = true; Acc = 0; }
    |> _.Acc

let print2 =
    Console.WriteLine(compute2 example2)
    Console.WriteLine(compute2 p1)
    ()
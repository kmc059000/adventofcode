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

//printfn $"""%i{(hashStr "HASH")}""" 

let solve1 = splitBy "," >> (Array.sumBy hashStr)



type Lens = {
    label : string
    focalLength : int;
}

type BoxNum = int

type Box = {
    num : BoxNum
    lenses : Lens list
}

type Command = Remove of BoxNum*string | Insert of BoxNum*string * int

let printState boxes command =
    printfn $"For Command {command}"
    boxes
    |> List.filter (_.lenses >> List.tryHead >> Option.isSome)
    |> List.sortBy _.num
    |> List.iter (fun box ->
        printfn ""
        printfn $"\tBox {box.num}:"
        box.lenses |> List.iter (fun lens ->
            printf $"\t[{lens.label} {lens.focalLength}]"
        )
    )


let parseCommand (str : string) : Command  =
    if str.Contains("=") then
        let label = str.Substring(0, str.IndexOf("="))
        let focalLength = int (str.Substring(str.IndexOf("=")  + 1))
        let box : BoxNum = hashStr label
        Insert (box,label, focalLength)
    else
        let label = str.Substring(0, str.IndexOf("-"))
        let box : BoxNum = hashStr label
        Remove (box,label)

let parseCommands input = input |> splitByComma |> Seq.map parseCommand |> List.ofSeq


let initialBoxes = [0..256] |> List.map (fun n -> {num = n; lenses = []})

let processCommand boxes command =
    let newState =
        match command with
        | Insert (box,label,focalLength) ->
            let relevantBox = boxes |> List.find (fun b -> b.num = box)
            let existingLens = relevantBox.lenses |> List.tryFind (fun l -> l.label = label)
            let newLens = {label = label; focalLength = focalLength}
            let newLenses =
                match existingLens with
                | Some lens ->
                    relevantBox.lenses |> ListExtras.replaceElement lens newLens
                | None ->
                    // so inefficient!
                    relevantBox.lenses |> List.rev |> List.append [newLens] |> List.rev
            let newBox = {relevantBox with lenses = newLenses}
            boxes |> ListExtras.replaceElement relevantBox newBox
        | Remove (box,label) ->
            let relevantBox = boxes |> List.find (fun b -> b.num = box)
            let newLenses = relevantBox.lenses |> List.filter (fun l -> l.label <> label)
            let newBox = {relevantBox with lenses = newLenses}
            boxes |> ListExtras.replaceElement relevantBox newBox
    //printState newState command
    newState

let processCommands commands = Seq.fold processCommand initialBoxes commands

let sumPower boxes =
    boxes
    |> Seq.map (fun box ->
        let lenspower = box.lenses |> List.mapi (fun lensIdx lens -> (lensIdx + 1) * lens.focalLength) |> List.sum
        (box.num + 1) * lenspower
    )
    |> Seq.sum

let solve2 = parseCommands >> processCommands >> sumPower
    

let printAnswer = printAnswersWithSameInputs2 solve1 solve2 example1 p1
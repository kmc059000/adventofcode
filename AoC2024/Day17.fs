module Day17

open System
open Microsoft.FSharp.Core
open Utils
open AoC2023.Inputs.Day17


type Execution = {
    registerA: int64
    registerB: int64
    registerC: int64
    program: Map<int, int64>
    instructionPointer: int
    halted: bool
    output: string
    expectedOutput: string
}

type OpCode =
    | Adv
    | Bxl
    | Bst
    | Jnz
    | Bxc
    | Out
    | Bdv
    | Cdv

let parseOpCode (i: int64) =
    [|Adv; Bxl; Bst; Jnz; Bxc; Out; Bdv; Cdv;|][int i]
  
let getComboOperand (operand: int64) exe =
    match operand with
    | 0L | 1L | 2L | 3L -> int64 operand
    | 4L -> exe.registerA
    | 5L -> exe.registerB
    | 6L -> exe.registerC
    | _ -> failwith ("got bad combo operand - " + operand.ToString())

let movePointer num exe = { exe with instructionPointer = exe.instructionPointer + num }
let move2 = movePointer 2
    
let step exe =
    let opCode = Map.tryFind exe.instructionPointer exe.program |> Option.map parseOpCode
    let operand = Map.tryFind (exe.instructionPointer + 1) exe.program |> Option.defaultValue 0 |> int64
    
    let adv () =
        let numerator = exe.registerA
        let result = numerator >>> (getComboOperand operand exe |> int)
        result
    
    match opCode with
    | None -> {exe with halted = true}
    | Some opcode ->
        match opcode with
        | Adv ->
            { exe with registerA = adv() } |> move2
        | Bxl ->
            { exe with registerB = exe.registerB ^^^ operand } |> move2
        | Bst ->
            { exe with registerB = (getComboOperand operand exe) % 8L } |> move2
        | Jnz ->
            if exe.registerA = 0
            then exe |> move2
            else
                { exe with instructionPointer = int operand }
        | Bxc ->
            { exe with registerB = exe.registerB ^^^ exe.registerC } |> move2
        | Out ->
            let output = (getComboOperand operand exe) % 8L
            let newOutput =
                if exe.output = "" then output.ToString()
                else exe.output + "," + output.ToString()
            { exe with output = newOutput } |> move2
        | Bdv ->
            { exe with registerB = adv() } |> move2
        | Cdv ->
            { exe with registerC = adv() } |> move2

[<TailCall>]
let rec solve exe =
    if exe.halted then
        exe
    else
        solve (step exe) 

let parse input =
    let lines = splitInputByNewLines input
    {
        registerA = readInt lines[0]
        registerB = readInt lines[1]
        registerC = readInt lines[2]
        instructionPointer = 0
        halted = false
        program = readInts lines[3] |> Seq.map int64 |> Seq.indexed |> Map.ofSeq
        output = ""
        expectedOutput = readInts lines[3] |> List.map string |> String.concat ","
    }
 
let solve1 = solve >> _.output
let parseAndSolve = parse >> solve1

let print1 =
    // Console.WriteLine(parseAndSolve example1)
    // Console.WriteLine(parseAndSolve p1)
    ()
 
let stepOptimized exe =
    let bInterm = (exe.registerA &&& 7L) ^^^ 5L
    let c = exe.registerA >>> (int bInterm)
    let output = ((bInterm ^^^ c) ^^^ 6L) &&& 7L
    let newOutput =
        if exe.output = "" then output.ToString()
        else exe.output + "," + output.ToString()
    let newA = exe.registerA >>> 3
    { exe with
        registerA = newA
        output = newOutput
        halted = newA = 0L }
 
[<TailCall>]
let rec solveOptimized exe =
    if exe.halted then
        exe
    else
        solveOptimized (stepOptimized exe)  
    
    
let solve2 input start debug =
    let parsed = parse input
    let mutable solved = false
    let mutable i = start
    while not solved do
        if debug && i % 100000L = 0L then 
            Console.WriteLine(i.ToString())
            Console.WriteLine(solve { parsed with registerA = i } |> _.output)
        
        let result = solveOptimized { parsed with registerA = i }
        if result.output = result.expectedOutput then
            solved <- true
            Console.WriteLine(i.ToString())
        i <- i + 1L

// After reverse engineering the formula, the most significant octet of A prints the right most digit of the output.
// Therefore going right to left in the output, find an all A values (shifted the left enough bits for that digit)
// and filter to those that print that digit in the expected output. Then recurse with those A values to find the next digit.
// Changing bits on the right doesnt affect any digits printed on the left side of the output, so this allows us to find
// all A values from the MSB to LSB, which prints right to left.

// digit to find is the digit counting from the left
let rec fastp2 exe digitToFind aValue =
    let digitFromRight = (exe.program.Count - digitToFind - 1)
    if digitFromRight < 0 then
        [aValue]
    else
        let octetLocation = digitFromRight * 3
        let allOctets = [for i in 0..7 -> ((int64 i) <<< octetLocation) ||| aValue]
        let expectedDigit = exe.expectedOutput.Split(',') |> Array.rev |> (flip Array.get digitToFind) |> int |> int64
        
        let solveForA a = solveOptimized { exe with registerA = a }
        
        let matchesExpectedDigit = fun (exe, octetVal) -> 
            let outputDigits = exe.output.Split(',') |> Array.rev
            if (outputDigits.Length - 1) < digitToFind
            then false
            else (outputDigits[digitToFind] |> int |> int64) = expectedDigit
        
        let solvedForDigit =
            allOctets
            |> List.mapi (fun i a -> solveForA a, a)
            
        let matchingDigits =
            solvedForDigit
            |> List.filter matchesExpectedDigit
        
        if List.isEmpty matchingDigits then
            // we found no valid A values for this digit, so abort this branch
            []
        else
            // add the digit to the current A value, and recurse
            let newAs = matchingDigits |> List.map (snd >> ((|||) aValue))
            
            newAs
            |> List.collect (fun a -> fastp2 exe (digitToFind + 1) a)
 
let test2 input a =
    let parsed = parse input
    let result = solveOptimized { parsed with registerA = a }
    Console.WriteLine(a.ToString() + ": " + result.output) 
    
let solveFast2 () =
    let parsed = parse p1
    let lowestA =
        fastp2 parsed 0 0
        |> List.min
        
    test2 p1 lowestA
        
    let len = fastp2 parsed 0 0 |> List.length
    Console.WriteLine(lowestA.ToString() + " - " + len.ToString())
    ()


        
let parseAndSolveOptimized1 = parse >> solveOptimized >> _.output    
   
let print2 =
    solveFast2()
    //Console.WriteLine(parseAndSolveOptimized1 p1)
    // Console.WriteLine(solve2 example2 1 true)
    //Console.WriteLine(solve2 p1 140737488355331L true)
    // Console.WriteLine(test2 p1)
    ()
module Utils

open System
open System.Collections.Generic
open System.Text.RegularExpressions

let reverse (input:string) = input |> Seq.rev |> System.String.Concat
let joinInts (separator : string) (ints : seq<int>) =
    ints
    |> Seq.map (fun x -> x.ToString())
    |> String.concat separator
    
let splitBy (separator : string) (str: string) = str.Split(separator, StringSplitOptions.RemoveEmptyEntries)
let splitByList separator = splitBy separator >> List.ofArray


let splitBy2 (separator : string) (str: string) =
    let tokens = splitBy separator str
    tokens[0], tokens[1]


let splitBySpaces = splitBy " "
let splitByComma = splitBy ","

let splitIntsBySpaces = splitBySpaces >> Seq.map int

let splitInputByNewLines (str : string) = str.Split("\n", StringSplitOptions.RemoveEmptyEntries)

let splitInputByDoubleNewLines (str : string) = str.Split("\n\n", StringSplitOptions.RemoveEmptyEntries)

let parseInt (str : string) = Int32.Parse(str)


let splitByAll (separators : string) (str: string) = str.Split(separators.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
    
let chars (str : string) = str.ToCharArray() |> Array.map string

let charactersAs2dArray = splitInputByNewLines >> Array.map chars

let replace (find: string) (replacement: string) (string : string) = string.Replace(find, replacement)

let toString x = x.ToString()

let indexOf (find: string) (str : string) = str.IndexOf(find)
let lastIndexOf (find: string) (str : string) = str.LastIndexOf(find)

let tapValue value =
    Console.WriteLine(value.ToString())
    value
    
let tapValues<'a> : 'a seq -> 'a seq = Seq.map tapValue
    
let tapValue2 v1 value =
    Console.WriteLine(v1.ToString() + " " + value.ToString())
    value
    
let tapValues2<'a> v : 'a seq -> 'a seq = Seq.map (tapValue2 v)
    
let flip f x y = f y x

let isPositive x = x >= 0

let joinMaps (p:Map<'a,'b>) (q:Map<'a,'b>) = 
    Map(Seq.concat [ (Map.toSeq p) ; (Map.toSeq q) ])
    
let toTuple xFn yFn value = (xFn value, yFn value)


let parseGrid parseLine combineLines (input : string) =
    input
    |> splitInputByNewLines
    |> Seq.mapi parseLine
    |> combineLines
    
let fst3 (x,_,_) = x

let default0 = Option.defaultValue 0
let default1 = Option.defaultValue 1

let atLeast1 num = Math.Max(1, num)

let mergeMaps map1 map2 =
    map1
    |> Map.toList
    |> List.fold (fun acc (key, value) ->
        Map.add key value acc
    ) map2
    
let mergeManyMaps maps =
    maps
    |> Seq.fold (fun acc map -> mergeMaps acc map) Map.empty
    
let isEven i = i % 2 = 0
let isOdd i = i % 2 = 1

let splitByRegex (regex : Regex) string = regex.Split(string) |> List.ofArray
    
let stringNotEmpty = String.IsNullOrEmpty >> not


let regexWhitespace = Regex("\s+", RegexOptions.Compiled)

let splitByWhitespace = splitByRegex regexWhitespace


let inline (||>) (x1,x2) f = f x1 x2

module SeqExtras =
    let compactStrings : (string seq -> string seq) = Seq.filter stringNotEmpty
    
    let pluckFirst2ToTuple seq = 
        let e1 = Seq.head seq
        let e2 = Seq.head (Seq.skip 1 seq)
        e1, e2
        
    let inline defaultIfEmpty<'a> (def : 'a) (s : 'a seq) = if Seq.isEmpty s then [def] |> Seq.ofList else s
    
    let mapFirst mapFn = Seq.mapi (fun i x -> if i = 0 then mapFn x else x)
   
   
let printAnswers1 solvePart1 e1 p1 solvePart2 e2 p2 =
    printfn "********* Part 1 *********"
    printfn "********* Example *********"
    printfn $"%A{solvePart1 e1}"
    printfn ""
    printfn "********* Actual *********"
    printfn $"%A{solvePart1 p1}"
    printfn ""
    
let printAnswers2 solvePart1 e1 p1 solvePart2 e2 p2 =
    printfn "********* Part 2 *********"
    printfn "********* Example *********"
    printfn $"%A{solvePart2 e2}"
    printfn ""
    printfn "********* Actual *********"
    printfn $"%A{solvePart2 p2}"
    printfn ""
    
let printAnswers solvePart1 e1 p1 solvePart2 e2 p2 =
    printAnswers1 solvePart1 e1 p1 solvePart2 e2 p2
    printAnswers2 solvePart1 e1 p1 solvePart2 e2 p2
    
    
let printAnswersWithSameInputs solve1 solve2 e1 p1 = printAnswers solve1 e1 p1 solve2 e1 p1
let printAnswersWithSameInputs1 solve1 solve2 e1 p1 = printAnswers1 solve1 e1 p1 solve2 e1 p1
let printAnswersWithSameInputs1Example solve1 solve2 e1 p1 = printAnswers1 solve1 e1 e1 solve2 e1 p1
let printAnswersWithSameInputs2 solve1 solve2 e1 p1 = printAnswers2 solve1 e1 p1 solve2 e1 p1
let printAnswersWithSameInputs2Example solve1 solve2 e1 p1 = printAnswers2 solve1 e1 e1 solve2 e1 e1

let inline defaultIfEmpty d l = if List.isEmpty l then [d] else l


module TupleExtras =
    
    let from2 a b = a,b
    
    let replicate x = x, x

    let curry f x y = f (x, y)

    let uncurry f (x, y) = f x y

    let swap (x, y) = (y, x)

    let mapFst f (x, y) = f x, y

    let mapSnd f (x, y) = x, f y

    let extendFst f (x,y) = f (x,y), y

    let extendSnd f (x,y) = x, f(x,y)

    let optionOfFst f (x, y) =
        match f x with
        | Some x' -> Some (x', y)
        | None    -> None

    let optionOfSnd f (x, y) =
        match f y with
        | Some y' -> Some (x, y')
        | None    -> None
        
    let applyBack f (x,y) = f y x
        
        
module StringExtras =
    let characters (str : string) = str.ToCharArray() |> Array.map _.ToString()
    
    let matches (regex : Regex) (str : string) =
        regex.Matches(str).AsReadOnly() |> Seq.map _.Value |> Seq.toArray
        
    let characterSquare = splitInputByNewLines >> Array.map characters
        
        
let rec gcdU64 (a : uint64) (b : uint64) =
    if b = 0UL then a
    else gcdU64 b (a % b)

let lcmU64 (a : uint64) (b : uint64) = (a * b) / (gcdU64 a b)


module ListExtras =
    let every fn = List.exists (fn >> not) >> not
    
    let replaceElement findElement replaceElement list =
        list |> List.map (fun x -> if x = findElement then replaceElement else x)
    
    
let manhattanDistance (x1, y1) (x2, y2) = abs((int64 x1) - (int64 x2)) + abs((int64 y1) - (int64 y2))
module Day11
open Utils
open AoC2023.Inputs.Day11

type Galaxy = {
    row: int
    col: int;
}

type Universe = {
    galaxies : Galaxy list;
    maxRow : int;
    maxCol : int;
}

let printUniverse universe =
    [for row in 1..universe.maxRow + 1 do
        yield [for col in 1..universe.maxCol + 1 do
               yield if universe.galaxies |> List.exists (fun galaxy -> galaxy.row = row && galaxy.col = col) then "#" else "."
        ] |> String.concat ""
    ] |> String.concat "\n" |> printfn "%s"
    universe

let makeUniverse galaxies =
    let maxRow = galaxies |> List.maxBy _.row
    let maxCol = galaxies |> List.maxBy _.col
    { galaxies = galaxies; maxRow = maxRow.row; maxCol = maxCol.col }

let parse input =
    let arr = charactersAs2dArray input
    let pickGalaxy rowNum colNum char = if char = "#" then Some { row = rowNum + 1; col = colNum + 1 } else None
    let pickGalaxiesFromRow rowNum row = row |> Array.mapi (pickGalaxy rowNum) |> Array.choose id
    let galaxies = arr |> Array.mapi pickGalaxiesFromRow |> Array.collect id |> List.ofArray
    makeUniverse galaxies
    

let expandUniverse universe =
    let knownRows = universe.galaxies |> List.map _.row |> Set.ofList
    let knownCols = universe.galaxies |> List.map _.col |> Set.ofList    
    let emptyRows = [1..universe.maxRow] |> List.filter (flip Set.contains knownRows >> not)
    let emptyCols = [1..universe.maxCol] |> List.filter (flip Set.contains knownCols >> not)
    
    printfn $"knownRows: %A{knownRows}"
    printfn $"knownCols: %A{knownCols}"
    
    printfn $"emptyRows: %A{emptyRows}"
    printfn $"emptyCols: %A{emptyCols}"
    
    let galaxies =
        universe.galaxies
        |> List.map (fun galaxy ->
            let rowsToAdd = emptyRows |> List.sumBy (fun emptyRow -> if emptyRow < galaxy.row then 1 else 0)
            let colsToAdd = emptyCols |> List.sumBy (fun emptyCol -> if emptyCol < galaxy.col then 1 else 0)
            { row = galaxy.row + rowsToAdd; col = galaxy.col + colsToAdd }
        )
    
    { galaxies = galaxies
      maxRow = universe.maxRow + emptyRows.Length
      maxCol = universe.maxCol + emptyCols.Length; 
    }


let measureDistance g1 g2 = manhattanDistance (g1.row, g1.col) (g2.row, g2.col)

let testDistances =
    printfn $"%i{(measureDistance { row = 7; col = 2 } { row = 12; col = 6 })}"
    printfn $"%i{(measureDistance { row = 1; col = 5 } { row = 8; col = 13 })}"
    printfn $"%i{(measureDistance { row = 3; col = 1 } { row = 8; col = 13 })}"
    printfn $"%i{(measureDistance { row = 9; col = 1 } { row = 9; col = 6 })}"
    

let allCombos universe =
    let rec loop galaxies =
        match galaxies with
        | [] -> []
        | galaxy :: rest -> (galaxy, rest) :: loop rest
    let pairs = loop universe.galaxies
    pairs |> List.collect (fun (galaxy, rest) -> rest |> List.map (fun g2 -> (galaxy, g2)))



let allDistances universe =    
    universe |> allCombos |> List.sumBy (fun (g1, g2) -> measureDistance g1 g2)

let solve1 = parse >> expandUniverse >> printUniverse >> allDistances

//let solve1 _ = testDistances

let solve2 = id
    

let printAnswer = printAnswersWithSameInputs1 solve1 solve2 example1 p1
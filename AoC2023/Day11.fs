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
    

let expandUniverse dist universe =
    // this is how many additional rows/cols to _add_ for the expansion.
    // If we are adding 1 empty row represents 10 expanded rows, `dist` would be 10, but we only need
    // to add 9 rows in this logic to get the full 10 since the original row remains.
    let expansionDist = dist - 1
    let knownRows = universe.galaxies |> List.map _.row |> Set.ofList
    let knownCols = universe.galaxies |> List.map _.col |> Set.ofList    
    let emptyRows = [1..universe.maxRow] |> List.filter (flip Set.contains knownRows >> not)
    let emptyCols = [1..universe.maxCol] |> List.filter (flip Set.contains knownCols >> not)
    
    // printfn $"knownRows: %A{knownRows}"
    // printfn $"knownCols: %A{knownCols}"
    //
    // printfn $"emptyRows: %A{emptyRows}"
    // printfn $"emptyCols: %A{emptyCols}"
    
    let galaxies =
        universe.galaxies
        |> List.map (fun galaxy ->
            let rowsToAdd = expansionDist * (emptyRows |> List.sumBy (fun emptyRow -> if emptyRow < galaxy.row then 1 else 0))
            let colsToAdd = expansionDist * (emptyCols |> List.sumBy (fun emptyCol -> if emptyCol < galaxy.col then 1 else 0))
            { row = galaxy.row + rowsToAdd; col = galaxy.col + colsToAdd }
        )
    
    { galaxies = galaxies
      maxRow = universe.maxRow + (emptyRows.Length * expansionDist)
      maxCol = universe.maxCol + (emptyCols.Length * expansionDist); 
    }


let measureDistance g1 g2 = manhattanDistance (g1.row, g1.col) (g2.row, g2.col)

// let testDistances =
//     printfn $"%i{(measureDistance { row = 7; col = 2 } { row = 12; col = 6 })}"
//     printfn $"%i{(measureDistance { row = 1; col = 5 } { row = 8; col = 13 })}"
//     printfn $"%i{(measureDistance { row = 3; col = 1 } { row = 8; col = 13 })}"
//     printfn $"%i{(measureDistance { row = 9; col = 1 } { row = 9; col = 6 })}"
    

let allCombos universe =
    let rec loop galaxies =
        match galaxies with
        | [] -> []
        | galaxy :: rest -> (galaxy, rest) :: loop rest
    let pairs = loop universe.galaxies
    pairs |> List.collect (fun (galaxy, rest) -> rest |> List.map (fun g2 -> (galaxy, g2)))



let allDistances universe =    
    universe |> allCombos |> List.sumBy (fun (g1, g2) -> measureDistance g1 g2)

let solve1 = parse >> expandUniverse 2 >> printUniverse >> allDistances

let solve2 = parse >> expandUniverse 1_000_000 >> allDistances
    

let printAnswer = printAnswersWithSameInputs2 solve1 solve2 example1 p1
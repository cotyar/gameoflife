// Implements Conway's Game Of Life badly
// https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life on a torus
namespace GameOfLife

open System
open System.IO

module World =
    type [<Struct>] Cell = Dead | Live
    type World = Cell[,]

    let parseCell = function
                    | '_' -> Dead
                    | '*' -> Live
                    | c   -> failwithf "Unexpected character '%c' in the input" c // TODO: add characted coordinates

    let toCell = function
                    | Live, 2 | Live, 3
                    | Dead, 3           -> Live
                    | _                 -> Dead

    let evolve (world: World) =
        let width  = world |> Array2D.length1
        let height = world |> Array2D.length2

        let cellNeighbours i j =
            let rows = seq { Math.Max(0, i - 1) .. Math.Min(i + 1, height - 1) }
            let cols = seq { Math.Max(0, j - 1) .. Math.Min(j + 1, width - 1) }
 
            let mutable count = 0
            for wi in rows do
                for wj in cols do
                    if not (wi = i && wj = j) && (world.[wi,wj] = Live) then count <- count + 1

            (world.[i,j], count) |> toCell

        seq { for i in 0 .. height - 1 ->
                seq { for j in 0 .. width - 1 -> cellNeighbours i j } } 
        |> array2D

    let printCell = function
                    | Live -> '*'
                    | Dead -> '_'

    let printWorld (world: World) =
        let width  = world |> Array2D.length1
        let height = world |> Array2D.length2

        for i in 0 .. height - 1 do
            for j in 0 .. width - 1 do 
                world.[i,j] |> printCell |> printf "%c"
            printfn ""

    let readWorld fileName =
        let lines = File.ReadAllLines fileName
        let world = lines |> Array.map(Seq.map parseCell >> Seq.toArray)

        if world.Length < 3 then failwithf "Expected at least 3 lines, but found %d" world.Length
        if world.[0].Length < 3 then failwithf "Expected at least 3 column, but found %d" world.[0].Length

        let melformedLines = world |> Array.mapi (fun i line -> i, line.Length) |> Array.filter (fun (i, length) -> world.[0].Length <> length)
        if melformedLines.Length > 0 then failwithf "Incorrect length for lines: %A, expected: %d" melformedLines world.[0].Length

        world |> array2D


    [<EntryPoint>]
    let main argv =
        readWorld (if argv.Length > 0 then argv.[0] else "data/sample_input.txt")
        |> evolve 
        |> printWorld
        
        0


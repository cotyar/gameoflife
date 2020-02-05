#load "Program.fs"

open System
open System.IO

open GameOfLife.World

let areEqual (world1: World) (world2: World) =
    if (world1 |> Array2D.length1) <> (world2 |> Array2D.length1)
        || (world1 |> Array2D.length2) <> (world2 |> Array2D.length2) then false
    else 
        let height = world1 |> Array2D.length1
        let width  = world1 |> Array2D.length2

        let rec compare i j =
            world1.[i,j] = world2.[i,j]
            &&  if j = width - 1 then
                    if i = height - 1 then true
                    else compare (i + 1) 0
                else compare i (j + 1)

        compare 0 0

let test () =
    let world = readWorld (Path.Combine(__SOURCE_DIRECTORY__, "data", "sample2_input.txt"))

    printfn "Original world:"
    world |> printWorld

    printfn "Gen 1:"
    let gen1 = world |> evolve
    gen1 |> printWorld
    let expected1 = readWorld (Path.Combine(__SOURCE_DIRECTORY__, "data", "sample2_output_gen1.txt"))
    if areEqual expected1 gen1 |> not then failwithf "Mismatch in Gen1. Expected: '%A', got: '%A'" expected1 gen1

    printfn "Gen 2:"
    let gen2 = gen1 |> evolve
    gen2 |> printWorld
    let expected2 = readWorld (Path.Combine(__SOURCE_DIRECTORY__, "data", "sample2_output_gen2.txt"))
    if areEqual expected2 gen2 |> not then failwithf "Mismatch in Gen1. Expected: '%A', got: '%A'" expected2 gen2

    printfn "Gen 3:"
    let gen3 = gen2 |> evolve
    gen3 |> printWorld
    let expected3 = readWorld (Path.Combine(__SOURCE_DIRECTORY__, "data", "sample2_output_gen3.txt"))
    if areEqual expected3 gen3 |> not then failwithf "Mismatch in Gen1. Expected: '%A', got: '%A'" expected3 gen3

    printfn "Completed successfully"

test ()


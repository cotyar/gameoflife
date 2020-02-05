# Game of Life

## Implements Conway's [Game Of Life](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life)

It's a coding test translating solution in the *badlife.fsharp* solution into one in the *goodlife.fsharp* solution.

The solution is intentially kept minimalistic with no external dependencies (even to unit test frameworks). 

## Changes compared to BadLife
- New module *World* was created to hold all world manipulation logic 
- ```fsharp    
    let input = System.IO.StreamReader("sample_input.txt")
    let all_text = input.ReadToEnd()
    let lines = all_text.Split([|'\r'; '\n'|])

    let world : bool array array = Array.zeroCreate lines.Length 
    for x in [0 .. lines.Length-1] do
        world.[x] <- Array.zeroCreate lines.[x].Length
        let mutable c = 0
        while c < lines.[x].Length do
            if lines.[x].[c] = '_' then
                world.[x].[c] <- false
            elif lines.[x].[c] = '*' then
                world.[x].[c] <- true
            c <- c + 1
    ```
    - data file loading was extracted into a separate function *readWorld*
    - bool array *world* was replaced with explicit type *World* of *Cell* ***struct*** discriminated union. Using ***struct DU*** allows better code readability while keeping allocations to minimum but with the cost of slightly more memory used per cell
    - Arrays2D is used instead of jagged arrays as mor appropriate 
    - explicit *parseCell* function was added in order to increase modularity and code readability
    - *parseCell* also allows avoiding preallocation of arrays and doing file parsing in just two lines of ideomatic functional code while eliminating usage of mutable variables 
    - basic data validations were added to both *readWorld* and *parseCell* ensuring predicted behaviour in case of broken input (the task assumes it out of scope but adding them was a too easy win to miss)
- ```fsharp    
    let evolve (world : bool array array) : unit = 
        let mutable neighbours = 0
        let rows = world.Length
        let cols = world.[0].Length

        for g in [0 .. world.Length-1] do
            for k in [0 .. world.Length-1] do
                if (world.[if g - 1 < 0 then rows - 1 else g - 1].[if k - 1 < 0 then cols - 1 else k - 1]) then neighbours <- neighbours+1
                if (world.[if g - 1 < 0 then rows - 1 else g - 1].[k]) then neighbours <- neighbours+1
                if (world.[if g - 1 < 0 then rows - 1 else g - 1].[if k + 1 = cols then 0 else k + 1]) then neighbours <- neighbours+1

                if (world.[g].[if k - 1 < 0 then cols - 1 else k - 1]) then neighbours <- neighbours+1
                if (world.[g].[if k + 1 = cols then 0 else k + 1]) then neighbours <- neighbours+1
                if (world.[if g + 1 = rows then 0 else g + 1].[if k - 1 < 0 then cols - 1 else k - 1]) then neighbours <- neighbours+1
                if (world.[if g + 1 = rows then 0 else g + 1].[k]) then neighbours <- neighbours+1
                if (world.[if g + 1 = rows then 0 else g + 1].[if k + 1 = cols then 0 else k + 1]) then neighbours <- neighbours+1

                if (world.[g].[k]&& neighbours < 2) then world.[g].[k] <- false
                if (world.[g].[k] && neighbours = 2 || neighbours = 3) then world.[g].[k] <- true
                if (world.[g].[k] && neighbours > 3) then world.[g].[k] <- false
                if (not world.[g].[k] && neighbours = 3) then world.[g].[k] <- true 
    ```
    - *evolve* function was completely rewritten as considered unreadable, dangerous and simply wrong - it updates in place state of a cell which may affect calculation of up to four following cells (next one and three under) during the same iteration. 
    - (minor) *seq {}* could be used instead of lists in for loops
    - the new solution keeps all iterations immutable and separate
    - doesn't overallocate 
    - a mutable *counter* was (arguably) considered more readable comparing to equivalent linq-like query 
    - amount of *if-statements* was massively reduced
- ```fsharp    
    for a in [0 .. world.Length] do
        let mutable line = ""
        for b in [0 .. world.[0].Length-1] do
            if world.[a].[b] then line <- line + "*" else line <- line + " "
        printfn "%s" line
    ```
    - *printWorld* and *printCell* functions were introduced in order to increase code readability, avoid mutables and extra string allocations
- ```fsharp    
    42 // return an integer exit code
    ```
    - replaced with 0 as non-zero application exit codes are usually considered as program abnormal termination 



## Notes

Also Test.fsx demonstrates programmatic usage with an alternative asymetric input data set and three iterations.

Program.main can be called programatically for an input file from the same script file with: 
`let ret = Program.main [| Path.Combine(__SOURCE_DIRECTORY__, "sample_input2.txt") |]`

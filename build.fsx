#r "nuget: Fable.PublishUtils, 2.4.0"

open PublishUtils

let args =
    fsi.CommandLineArgs
    |> Array.skip 1
    |> List.ofArray

match args with
| IgnoreCase "publish"::_ ->
    pushFableNuget "src/Fable.Solid/Fable.Solid.fsproj" [] doNothing
    pushFableNuget "src/Elmish.Solid/Elmish.Solid.fsproj" [] doNothing
| _ -> ()

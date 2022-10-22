[<AutoOpen>]
module Resource

open Fable.Core
open Feliz.JSX.Solid
open Fetch

printfn $"Loading {__SOURCE_FILE__}..."

let fetchUser id =
    tryFetch $"https://swapi.dev/api/people/{id}/" []
    |> Promise.bind (function
        | Ok res -> res.json ()
        | Error er -> Promise.lift {| error = er.Message |})

[<Emit("Object.keys($0 || {}).map(k => [k, $0[k]])")>]
let keyValues (o: obj) : (string * obj)[] = nativeOnly

type Components with

    [<JSX.Component>]
    static member Resource() =
        let userId, setUserId = Solid.createSignal (None)
        let user, _ = Solid.createResource (userId, fetchUser)

        Html.fragment
            [
                Html.label $"""UserID: """
                Html.input
                    [
                        Attr.typeNumber
                        Attr.min 1
                        Attr.placeholder "Enter Numeric Id"
                        Ev.onTextInput (fun (value: string) -> value |> Some |> setUserId)
                    ]
                Html.ul
                    [
                        // Use parenthesis to prevent F# from interpreting
                        // this as a list builder
                        (if user.loading then
                             Html.li "Loading..."
                         else
                             Solid.For(
                                 keyValues user.current,
                                 fun (k, v) _ -> Html.li [ Html.children [ Html.strong k; Html.text $": {string v}" ] ]
                             ))
                    ]
            ]

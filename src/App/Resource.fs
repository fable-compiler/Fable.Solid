[<AutoOpen>]
module Resource

open Fable.Core
open Fable.Core.JS
open Feliz.JSX.Solid
open Fetch

printfn $"Loading {__SOURCE_FILE__}..."

// const fetchUser = async (id) =>
//   (await fetch(`https://swapi.dev/api/people/${id}/`)).json();

let fetchUser id =
    promise {
        let! res = fetch $"https://swapi.dev/api/people/{id}/" []
        let! json = res.json()
        return JSON.stringify json
    }

type Components with
    [<JSX.Component>]
    static member Resource() =
        let userId, setUserId = Solid.createSignal(None)
        let user, _ = Solid.createResource (userId, fetchUser)


        Html.fragment [
            Html.label $"UserID : {userId()}"
            Html.br []
            Html.input [
                Attr.typeNumber
                Attr.min 1
                Attr.placeholder "Enter Numeric Id"
                Ev.onTextInput (fun (value: string) -> value |> Some |> setUserId)
            ]
            if user.loading then Html.span "Loading" else Html.none
            Html.div [
                Html.pre $"{JSON.stringify(user())}"
            ]
        ]



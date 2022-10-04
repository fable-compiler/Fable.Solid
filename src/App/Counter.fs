[<AutoOpen>]
module Counter

open Fable.Core
open Feliz.JSX.Solid

printfn $"Loading {__SOURCE_FILE__}..."

type Components with
    [<JSX.Component>]
    static member Counter() =
        let count, setCount = Solid.createSignal(0)
        let doubled() = count() * 2
        let quadrupled() = doubled() * 2

        Html.fragment [
            Html.p $"{count()} * 2 = {doubled()}"
            Html.p $"{doubled()} * 2 = {quadrupled()}"
            Html.br []
            Html.button [
                Attr.className "button"
                Ev.onClick(fun _ -> count() + 1 |> setCount)
                Html.children [
                    Html.text $"Click me!"
                ]
            ]
        ]

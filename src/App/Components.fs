namespace global

open Fable.Core
open Feliz.JSX.Solid

type Components =
    [<JSX.Component>]
    static member Div (classes: string seq) children =
        Html.div [
            Attr.classList classes
            Html.children children
        ]

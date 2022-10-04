module App

open Browser
open Feliz.JSX.Solid
open Fable.Core
open Solid
open type Components

type Lazy =
    static member Counter() = Solid.lazyImport Counter
    static member Svg() = Solid.lazyImport Svg
    static member Sketch() = Solid.lazyImport Sketch
    static member TodoElmish() = Solid.lazyImport TodoElmish
    static member Shoelace() = Solid.lazyImport Shoelace

[<JSX.Component>]
let Tab(href: string, txt: string) =
    Html.li (Router.Link(href, Html.text txt))

[<JSX.Component>]
let Tabs() =
    Html.fragment [
        Html.div [
            Attr.className "tabs"
            Attr.style [
                Css.marginBottom (length.rem 0.5)
            ]
            Html.children [
                Html.ul [
                    Html.children [
                        Tab("/counter", "Counter")
                        Tab("/svg", "Svg")
                        Tab("/sketch", "Sketch")
                        Tab("/elmish", "Todo Elmish")
                        Tab("/shoelace", "Shoelace Web Components")
                    ]
                ]
            ]
        ]
        Html.div [
            Attr.style [
                Css.margin (length.zero, length.auto)
                Css.maxWidth 800
                Css.padding 20
            ]
            Html.children [
                Router.Routes [
                    Router.Route("/counter", ``component`` = Lazy.Counter)
                    Router.Route("/svg", ``component`` = Lazy.Svg)
                    Router.Route("/sketch", ``component`` = Lazy.Sketch)
                    Router.Route("/elmish", ``component`` = Lazy.TodoElmish)
                    Router.Route("/shoelace", ``component`` = Lazy.Shoelace)
                ]
            ]
        ]
    ]

Solid.render((fun () -> Router.Router(Tabs())), document.getElementById("app-container"))

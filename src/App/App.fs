module App

open Browser
open Feliz.JSX.Solid
open Fable.Core
open Solid
open type Components

type Lazy =
    static member Counter() = Solid.lazyImport Counter
    static member Resource() = Solid.lazyImport Resource
    static member Svg() = Solid.lazyImport Svg
    static member Sketch() = Solid.lazyImport Sketch
    static member TodoElmish() = Solid.lazyImport TodoElmish
    static member Shoelace() = Solid.lazyImport Shoelace

[<JSX.Component>]
let Tab(href: string, txt: string) =
    Html.li (Router.Link(href, Html.text txt))

let LoadingWrapper(inner: unit -> JSX.Element) () =
    Solid.Suspense(
        Html.text "Loading...",
        Solid.ErrorBoundary(
            (fun (e: exn) (_) -> JSX.text $"{e}"),
            inner()
        )
    )

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
                        Tab("/resource", "Resource")
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
                    Html.fragment [
                        Router.Route("/",         LoadingWrapper(Lazy.Counter))
                        Router.Route("/counter",  LoadingWrapper(Lazy.Counter))
                        Router.Route("/resource", LoadingWrapper(Lazy.Resource))
                        Router.Route("/svg",      LoadingWrapper(Lazy.Svg))
                        Router.Route("/sketch",   LoadingWrapper(Lazy.Sketch))
                        Router.Route("/elmish",   LoadingWrapper(Lazy.TodoElmish))
                        Router.Route("/shoelace", LoadingWrapper(Lazy.Shoelace))
                    ]
                ]
            ]
        ]
    ]

[<JSX.Component>]
let App() =
    Router.Router(
        ColorProvider("green", Tabs())
    )

Solid.render((fun () -> App()), document.getElementById("app-container"))

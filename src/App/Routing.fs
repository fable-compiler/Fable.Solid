module Routing

open Browser
open Feliz.JSX.Solid
open Fable.Core
open Solid


type Lazy =
    static member Counter() = Solid.lazyImport Components.Counter
    static member Resource() = Solid.lazyImport Components.Resource
    static member Svg() = Solid.lazyImport Components.Svg
    static member Sketch() = Solid.lazyImport Components.Sketch
    static member TodoElmish() = Solid.lazyImport Components.TodoElmish
    static member Shoelace() = Solid.lazyImport Components.Shoelace

[<JSX.Component>]
let Tabs (children) =
    Html.fragment [
        Html.div [
            Attr.className "tabs"
            Attr.style [ Css.marginBottom (length.rem 0.5) ]
            Html.children [
                Html.ul [
                    Html.li (Router.A("/", Html.text "home"))
                    Html.li (Router.A("/counter",Html.text  "Counter"))
                    Html.li (Router.A("/resource", Html.text "Resource"))
                    Html.li (Router.A("/svg", Html.text "Svg"))
                    Html.li (Router.A("/sketch", Html.text "Sketch"))
                    Html.li (Router.A("/elmish", Html.text "Todo Elmish"))
                    Html.li (Router.A("/shoelace", Html.text "Shoelace Web Components"))
                ]
            ]
        ]
        children
    ]

[<JSX.Component>]
let Home () =
    JSX.jsx $"""<h1>Welcome home!</h1>"""


[<JSX.Component>]
let ProvideColor (color,children) = // a Context.Provider used in Counter.fs
    Components.ColorProvider(color, children)

[<JSX.Component>]
let Routing () =
    // below, in <Router root={{Tabs}}>, use double curly braces to pass 'Tab' on as string in jsx and not insert a function via fable
    JSX.jsx $"""
        import {{ Route, Router}} from "@solidjs/router";
        <div style = { {| margin = "0 auto"; ``max-width`` = "800px"; padding = "20px" |} }>
            <ProvideColor color="green">
                <Router root={{Tabs}} >
                    <Route path="/"         component={Home} />
                    <Route path="/counter"  component={Lazy.Counter} />
                    <Route path="/resource" component={Lazy.Resource} />
                    <Route path="/svg"      component={Lazy.Svg} />
                    <Route path="/sketch"   component={Lazy.Sketch} />
                    <Route path="/elmish"   component={Lazy.TodoElmish} />
                    <Route path="/shoelace" component={Lazy.Shoelace} />
                </Router>
            </ProvideColor>
        </div>
        """

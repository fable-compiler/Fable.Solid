module Routing

open Browser
open Feliz.JSX.Solid
open Fable.Core
open Solid
// open type Components

type Lazy =
    static member Counter() = Solid.lazyImport Components.Counter
    static member Resource() = Solid.lazyImport Components.Resource
    static member Svg() = Solid.lazyImport Components.Svg
    static member Sketch() = Solid.lazyImport Components.Sketch
    static member TodoElmish() = Solid.lazyImport Components.TodoElmish
    static member Shoelace() = Solid.lazyImport Components.Shoelace

[<JSX.Component>]
let Tabs (children) =
    Html.fragment
        [ Html.div
              [ Attr.className "tabs"
                Attr.style [ Css.marginBottom (length.rem 0.5) ]
                Html.children
                    [ Html.ul
                          [ Html.children
                                [ Html.li (Router.A("/", "home"))
                                  Html.li (Router.A("/counter", "Counter"))
                                  Html.li (Router.A("/resource", "Resource"))
                                  Html.li (Router.A("/svg", "Svg"))
                                  Html.li (Router.A("/sketch", "Sketch"))
                                  Html.li (Router.A("/elmish", "Todo Elmish"))
                                  Html.li (Router.A("/shoelace", "Shoelace Web Components")) ] ] ] ]
          children ]

[<JSX.Component>]
let Home () =
    console.log ("loading Home!")
    JSX.jsx $"""<h1>Welcome home!</h1>"""


[<JSX.Component>]
let Routing () =
    // below, in <Router root={{Tabs}}>, use double curly braces to pass 'Tab' on as string in jsx and not insert a function via fable
    JSX.jsx
        $"""
    import {{ Route, Router}} from "@solidjs/router";
    <div style = {{| margin = "0 auto"
                     ``max-width`` = "800px"
                     padding = "20px" |}}>
        <Router root={{Tabs}}  >
            <Route path="/" component={Home} />
            <Route path="/counter" component={Lazy.Counter} />
            <Route path="/resource" component={Lazy.Resource} />
            <Route path="/svg" component={Lazy.Svg} />
            <Route path="/sketch" component={Lazy.Sketch} />
            <Route path="/elmish" component={Lazy.TodoElmish} />
            <Route path="/shoelace" component={Lazy.Shoelace} />
        </Router>
    </div>
    """

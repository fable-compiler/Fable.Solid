namespace Solid

open Fable.Core

/// Bindings (partial) for @solidjs/router
type Router =
    [<ImportMember("@solidjs/router"); JSX.Component>]
    static member Router(children: JSX.Element): JSX.Element = jsNative

    [<ImportMember("@solidjs/router"); JSX.Component>]
    static member Routes(children: JSX.Element seq): JSX.Element = jsNative

    [<ImportMember("@solidjs/router"); JSX.Component>]
    static member Route(path: string, ``component``: JSX.ElementType): JSX.Element = jsNative

    [<ImportMember("@solidjs/router"); JSX.Component>]
    static member Route(path: string, element: JSX.Element): JSX.Element = jsNative

    [<ImportMember("@solidjs/router"); JSX.Component>]
    static member Link(href: string, children: JSX.Element): JSX.Element = jsNative

    [<ImportMember("@solidjs/router")>]
    static member useIsRouting(): unit -> bool = jsNative

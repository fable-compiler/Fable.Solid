namespace Solid

open Fable.Core

/// Bindings (partial) for @solidjs/router
type Router =
    [<ImportMember("@solidjs/router"); JSX.Component>]
    static member Router(children: JSX.Element): JSX.Element = jsNative

    [<ImportMember("@solidjs/router"); JSX.Component>]
    static member Router(children: JSX.Element seq): JSX.Element = jsNative

    [<ImportMember("@solidjs/router"); JSX.Component>]
    static member Router(root:JSX.ElementType, children: JSX.Element seq): JSX.Element = jsNative

    /// The HashRouter is a top level component that manages the routing of your application.
    /// It is a client side router that uses hash-values in the URL - providing a single-page application
    /// a way to replicate the experience of a multi-page application.
    /// Since hash-routing provides a way for an application to run from a single HTML file,
    /// it can be used when hosting on a static file server.
    /// Compared to a browser-router, such as Router, is that this approach is not SEO friendly.
    /// Because most search engines do not index the hash portion of a URL,
    /// they are only able to see the index page of your application when using this approach.
    /// The root property is used for components that wrap a matched route and require access to the router context,
    /// relevant with navigation components that use &lt;A&gt; links.
    [<ImportMember("@solidjs/router"); JSX.Component>]
    static member HashRouter(root:JSX.ElementType, children: JSX.Element seq): JSX.Element = jsNative

    [<ImportMember("@solidjs/router"); JSX.Component>]
    static member Route(path: string, ``component``: JSX.ElementType): JSX.Element = jsNative

    [<ImportMember("@solidjs/router"); JSX.Component>]
    static member Route(path: string, element: JSX.Element): JSX.Element = jsNative


    /// Solid-Router exposes an &lt;A /&gt; component as a wrapper around the native &lt;a /&gt; tag.
    /// &lt;A /&gt; supports relative and base paths. &lt;a /&gt; doesn't. But &lt;a /&gt; gets augmented when JS is present via
    /// a top-level listener to the DOM, so you get the soft-navigation experience nonetheless.
    /// The &lt;A /&gt; supports the &lt;Router /&gt; base property (&lt;Router base="/subpath"&gt;) and prepend it
    /// to the received href automatically and the &lt;a /&gt;does not. The same happens with relative paths passed to &lt;A /&gt;.
    /// The &lt;A&gt; tag has an active class if its href matches the current location, and inactive otherwise.
    /// By default matching includes locations that are descendants (e.g.: href /users matches locations /users and /users/123).
    [<ImportMember("@solidjs/router"); JSX.Component>]
    static member A(href: string, children: JSX.Element): JSX.Element = jsNative

    /// Solid-Router exposes an &lt;A /&gt; component as a wrapper around the native &lt;a /&gt; tag.
    /// &lt;A /&gt; supports relative and base paths. &lt;a /&gt; doesn't. But &lt;a /&gt; gets augmented when JS is present via
    /// a top-level listener to the DOM, so you get the soft-navigation experience nonetheless.
    /// The &lt;A /&gt; supports the &lt;Router /&gt; base property (&lt;Router base="/subpath"&gt;) and prepend it
    /// to the received href automatically and the &lt;a /&gt;does not. The same happens with relative paths passed to &lt;A /&gt;.
    /// The &lt;A&gt; tag has an active class if its href matches the current location, and inactive otherwise.
    /// By default matching includes locations that are descendants (e.g.: href /users matches locations /users and /users/123).
    [<ImportMember("@solidjs/router"); JSX.Component>]
    static member A(href: string, ``base``:string , children: JSX.Element): JSX.Element = jsNative

    /// Solid Router provides a Navigate component that works similarly to &lt;A&gt;,
    /// but it will immediately navigate to the provided path as soon as the component is rendered.
    /// It also uses the href prop, but with the additional option of passing a
    /// function to href that returns a path to navigate to:
    [<ImportMember("@solidjs/router"); JSX.Component>]
    static member Navigate(href: string, children: JSX.Element): JSX.Element = jsNative

    /// Solid Router provides a Navigate component that works similarly to &lt;A&gt;,
    /// but it will immediately navigate to the provided path as soon as the component is rendered.
    /// It also uses the href prop, but with the additional option of passing a
    /// function to href that returns a path to navigate to:
    [<ImportMember("@solidjs/router"); JSX.Component>]
    static member Navigate(href: unit-> string, children: JSX.Element): JSX.Element = jsNative
namespace global

open System
open Fable.Core
open Fable.Core.JsInterop

type SolidStore<'T> =
    [<Emit("$0")>]
    abstract Value: 'T

type SolidStoreSetter<'T> =
    [<Emit("$0($1)")>]
    abstract Update: 'T -> unit
    [<Emit("$0($1)")>]
    abstract Update: ('T -> 'T) -> unit
    [<Emit("$0(...$1)")>]
    abstract UpdatePath: obj[] -> unit

type SolidStorePath<'T, 'Value>(setter: SolidStoreSetter<'T>, path: obj[]) =
    member _.Setter = setter
    member _.Path = path
    member inline this.Map(map: 'Value -> 'Value2) =
        SolidStorePath<'T, 'Value2>(this.Setter, Experimental.namesofLambda map |> Array.map box |> Array.append this.Path)
    member this.Update(value: 'Value): unit = this.Setter.UpdatePath(Array.append this.Path [|value|])
    member this.Update(updater: 'Value -> 'Value): unit = this.Setter.UpdatePath(Array.append this.Path [|updater|])

[<AutoOpen>]
module SolidExtensions =
    type SolidStoreSetter<'T> with
        member this.Path = SolidStorePath<'T, 'T>(this, [||])

[<Runtime.CompilerServices.Extension>]
type SolidStorePathExtensions =
    [<Runtime.CompilerServices.Extension>]
    static member inline Item(this: SolidStorePath<'T, 'Value array>, index: int) =
        SolidStorePath<'T, 'Value>(this.Setter, Array.append this.Path [|index|])

    [<Runtime.CompilerServices.Extension>]
    static member inline Find(this: SolidStorePath<'T, 'Value array>, predicate: 'Value -> bool) =
        SolidStorePath<'T, 'Value>(this.Setter, Array.append this.Path [|predicate|])

type Solid =
    [<ImportMember("solid-js/web")>]
    static member render(f: unit -> JSX.Element, el: Browser.Types.Element): unit = jsNative

    static member renderDisposable(f: unit -> JSX.Element, el: Browser.Types.Element): IDisposable =
        // HACK Solid.render actually returns a disposable function
        let disp: unit->unit = !!Solid.render(f, el)
        { new IDisposable with
            member _.Dispose() = disp() }

    [<ImportMember("solid-js")>]
    static member createSignal(value: 'T): (unit -> 'T) * ('T -> unit) = jsNative

    [<ImportMember("solid-js")>]
    static member createMemo(value: unit -> 'T): (unit -> 'T) = jsNative

    [<ImportMember("solid-js")>]
    static member createEffect(effect: unit -> unit): unit = jsNative

    [<ImportMember("solid-js")>]
    static member createEffect(effect: 'T -> 'T, initialValue: 'T): unit = jsNative

    static member createRef<'El when 'El :> Browser.Types.Element>(): 'El ref = ref Unchecked.defaultof<'El>

    static member inline ref<'El when 'El :> Browser.Types.Element>(f: 'El -> unit): JSX.Prop = "ref", f

    static member inline ref<'El when 'El :> Browser.Types.Element>(r: 'El ref): JSX.Prop = "ref", box(fun el -> r.Value <- el)

    [<ImportMember("solid-js")>]
    static member onMount(value: unit -> unit): unit = jsNative

    [<ImportMember("solid-js")>]
    static member onCleanup(value: unit -> unit): unit = jsNative

    [<ImportMember("solid-js"); JSX.Component>]
    static member Index(each: 'T[], children: (unit -> 'T) -> int -> JSX.Element, ?fallback: JSX.Element): JSX.Element = jsNative

    [<ImportMember("solid-js"); JSX.Component>]
    static member For(each: 'T[], children: 'T -> (unit -> int) -> JSX.Element, ?fallback: JSX.Element): JSX.Element = jsNative

    [<ImportMember("solid-js"); JSX.Component>]
    static member Show(``when``: bool, children: JSX.Element): JSX.Element = jsNative

    [<ImportMember("solid-js"); JSX.Component>]
    static member Show(``when``: 'T option, children: 'T -> JSX.Element, ?fallback: JSX.Element): JSX.Element = jsNative

    [<ImportMember("solid-js"); JSX.Component>]
    static member Switch(children: JSX.Element list, ?fallback: JSX.Element): JSX.Element = jsNative

    [<ImportMember("solid-js"); JSX.Component>]
    static member Match(``when``: bool, children: JSX.Element): JSX.Element = jsNative

    [<ImportMember("solid-js"); JSX.Component>]
    static member Match(``when``: 'T option, children: 'T -> JSX.Element): JSX.Element = jsNative

    [<ImportMember("solid-js/store")>]
    static member createStore(store: 'T): SolidStore<'T> * SolidStoreSetter<'T> = jsNative

    [<ImportMember("solid-js/store"); NamedParams(fromIndex=1)>]
    static member reconcile(store: 'T, ?merge: bool, ?key: string): 'T = jsNative

    [<ImportMember("solid-js/store")>]
    static member unwrap(store: 'T): 'T = jsNative

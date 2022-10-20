namespace global

open System
open Fable.Core
open Fable.Core.JsInterop

[<RequireQualifiedAccess; StringEnum>]
type SolidResourceState =
    /// Hasn't started loading, no value yet
    | Unresolved
    /// It's loading, no value yet
    | Pending
    /// Finished loading, has value
    | Ready
    /// It's re-loading, `latest` has value
    | Refreshing
    /// Finished loading with an error, no value
    | Errored

type SolidResource<'T> =
    /// Attention, will be undefined while loading
    [<Emit("$0()")>]
    abstract current: 'T
    abstract state: SolidResourceState
    abstract loading: bool
    abstract error: exn option
    /// Unlike `current`, it keeps the latest value while re-loading
    /// Attention, will be undefined until first value has been loaded
    abstract latest: 'T

type SolidResourceManager<'T> =
    abstract mutate: 'T -> 'T
    abstract refetch: unit -> JS.Promise<'T>

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

    /// Fetcher will be called immediately
    [<ImportMember("solid-js"); ParamObject(fromIndex=1)>]
    static member createResource(fetcher: unit -> 'T, ?initialValue: 'T): SolidResource<'T> * SolidResourceManager<'T>  = jsNative

    /// Fetcher will be called only when source signal returns `Some('U)`
    [<ImportMember("solid-js"); ParamObject(fromIndex=2)>]
    static member createResource(source: unit -> 'U option, fetcher: 'U -> 'T, ?initialValue: 'T): SolidResource<'T> * SolidResourceManager<'T>  = jsNative

    static member createRef<'El when 'El :> Browser.Types.Element>(): 'El ref = ref Unchecked.defaultof<'El>

    static member inline ref<'El when 'El :> Browser.Types.Element>(f: 'El -> unit): JSX.Prop = "ref", f

    static member inline ref<'El when 'El :> Browser.Types.Element>(r: 'El ref): JSX.Prop = "ref", box(fun el -> r.Value <- el)

    /// Exposes a JSX.Component through a lazy import, `lazy` must wrap the full body of the component.
    [<Import("lazy", "solid-js"); JS.RemoveSurroundingArgs>]
    static member ``lazy``(import: unit -> JS.Promise<'T>): JSX.Element = jsNative

    // Solid (like React) expects a default export, that's why we wrap the returned result

    /// Automatically creates a lazy component with a dynamic import to the referenced value
    static member inline lazyImport(com: JSX.ElementType): JSX.Element =
        Solid.``lazy``(fun () -> (importValueDynamic com).``then``(fun x -> createObj ["default" ==> x]))

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

module Elmish.Solid

open Elmish
open Fable.Core
open Fable.Core.JsInterop
open System.Runtime.CompilerServices
open System.Runtime.InteropServices

module Util =
    let isPlainObj (obj: obj): bool =
        emitJsExpr obj "$0 != null && typeof $0 === 'object' && Object.getPrototypeOf($0) === Object.prototype"

    let deepClone (o: 'T): 'T =
        let rec deepClone (o: obj) =
            if JS.Constructors.Array.isArray(o) then
                o :?> obj[] |> Array.map deepClone |> box
            elif Reflection.isRecord o || isPlainObj o then
                let newObj = obj()
                JS.Constructors.Object.keys(o) |> Seq.iter (fun k ->
                    newObj?(k) <- deepClone(o?(k)))
                newObj
            else o

        deepClone o :?> 'T

type Solid with
    /// Initialize a SolidJS store using an Elmish program: https://www.solidjs.com/tutorial/stores_nested_reactivity
    /// Attention: use arrays instead of lists for better performance
    static member
#if DEBUG
        inline
#endif
        createElmishStore(
            program: Program<unit, 'Model, 'Msg, unit>
#if DEBUG
            , [<CallerFilePath; Optional; DefaultParameterValue("")>] callerPath: string
#endif
        ): 'Model * ('Msg -> unit) =

        let mutable disposed = false
        let mutable dispatch = Unchecked.defaultof<_>
        let mutable model, cmd = Program.init program ()
#if DEBUG
        emitJsStatement () $"""
        if (import.meta.hot) {{
            import.meta.hot.accept();
            import.meta.hot.dispose(data => {{
                data.elmish = data.elmish || {{}};
                data.elmish[{callerPath}] = {model};
            }});
            if (import.meta.hot.data.elmish && import.meta.hot.data.elmish[{callerPath}]) {{
                {cmd <- Cmd.none};
                {model} = import.meta.hot.data.elmish[{callerPath}];
            }}
        }}
        """
#endif
        // We need to deep clone the model before creating the store to avoid conflicts during the updates
        let store, setStore = model |> Util.deepClone |> Solid.createStore
        Solid.onCleanup(fun () -> disposed <- true)

        program
        |> Program.map
            (fun _ _ -> model, cmd) id id id id
            (fun (predicate, terminate) ->
                (fun msg -> predicate msg || disposed),
                (fun model -> terminate model))
        |> Program.withSetState (fun model' dispatch' ->
            dispatch <- dispatch'
            // Skip update if model hasn't changed
            if not(obj.ReferenceEquals(model, model')) then
                model <- model'
                model' |> Util.deepClone |> Solid.reconcile |> setStore.Update)
        |> Program.run

        store.Value, dispatch

    /// Initialize a SolidJS store using an Elmish program: https://www.solidjs.com/tutorial/stores_nested_reactivity
    /// Attention: use arrays instead of lists for better performance
    static member inline createElmishStore(init: unit -> 'Model * Cmd<'Msg>, update: 'Msg -> 'Model -> 'Model * Cmd<'Msg>
#if DEBUG
            , [<CallerFilePath; Optional; DefaultParameterValue("")>] callerPath: string
#endif
    ) =
        Solid.createElmishStore(Program.mkProgram init update (fun _ _ -> ())
#if DEBUG
        , callerPath
#endif
        )
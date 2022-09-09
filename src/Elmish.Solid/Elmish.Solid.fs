module Elmish.Solid

open Elmish

module private Util =
    open Fable.Core
    open Fable.Core.JsInterop

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
    /// SolidJS can optimize updates for plain JS objects and arrays in the store, so you can provide a projection
    /// to do this transformation (e.g. convert records into anonymous records and lists into arrays)
    static member createElmishStore(program: Program<unit, 'Model, 'Msg, unit>, projection: 'Model -> 'ViewModel): 'ViewModel * ('Msg -> unit) =
        let mutable dispatch' = Unchecked.defaultof<_>
        let model, cmd = Program.init program ()

        // We need to deep clone the model before creating the store to avoid conflicts during the updates
        let store, setStore = model |> projection |> Util.deepClone |> Solid.createStore

        program
        |> Program.map (fun _ _ -> model, cmd) id id id id id
        |> Program.withSetState (fun model dispatch ->
            dispatch' <- dispatch
            let projected = projection model
            projected |> Util.deepClone |> Solid.reconcile |> setStore.Update)
        |> Program.run

        store.Value, dispatch'

    /// Initialize a SolidJS store using an Elmish program: https://www.solidjs.com/tutorial/stores_nested_reactivity
    /// SolidJS can optimize updates for plain JS objects and arrays in the store, so you can provide a projection
    /// to do this transformation (e.g. convert records into anonymous records and lists into arrays)
    static member createElmishStore(init: unit -> 'Model * Cmd<'Msg>, update: 'Msg -> 'Model -> 'Model * Cmd<'Msg>, projection: 'Model -> 'ViewModel) =
        Solid.createElmishStore(Program.mkProgram init update (fun _ _ -> ()), projection)

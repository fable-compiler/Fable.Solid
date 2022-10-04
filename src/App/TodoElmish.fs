[<AutoOpen>]
module TodoElmish

open Fable.Core
open Browser.Types
open Feliz.JSX.Solid
open Elmish.Solid
open type Components

printfn $"Loading {__SOURCE_FILE__}..."

module private App =
    open System
    open Elmish

    type Todo = {
        Id: Guid
        Description: string
        Editing: string option
        Completed: bool
    }

    type State =
        { Todos: Todo list }

    type Msg =
        | AddNewTodo of string
        | DeleteTodo of Guid
        | ToggleCompleted of Guid
        | CancelEdit
        | ApplyEdit of string
        | StartEditingTodo of Guid

    let newTodo txt =
        {
            Id = Guid.NewGuid()
            Description = txt
            Completed = false
            Editing = None
        }

    let initTodos() = [
        newTodo "Learn F#"
        { newTodo "Learn Elmish" with Completed = true }
    ]

    let init () =
        { Todos = initTodos() }, Cmd.none

    let update (msg: Msg) (state: State) =
        match msg with
        | AddNewTodo txt ->
            { state with Todos = (newTodo txt)::state.Todos }, Cmd.none

        | DeleteTodo todoId ->
            state.Todos
            |> List.filter (fun todo -> todo.Id <> todoId)
            |> fun todos -> { state with Todos = todos }, Cmd.none

        | ToggleCompleted todoId ->
            state.Todos
            |> List.map
                (fun todo ->
                    if todo.Id = todoId then
                        let completed = not todo.Completed
                        { todo with
                            Completed = completed }
                    else
                        todo)
            |> fun todos -> { state with Todos = todos }, Cmd.none

        | StartEditingTodo todoId ->
            state.Todos |> List.map (fun t ->
                match t.Editing with
                | _ when t.Id = todoId -> { t with Editing = Some t.Description }
                | Some _ -> { t with Editing = None }
                | _ -> t)
            |> fun todos -> { state with Todos = todos }, Cmd.none

        | CancelEdit ->
            state.Todos |> List.map (fun t ->
                if Option.isSome t.Editing
                then { t with Editing = None }
                else t)
            |> fun todos -> { state with Todos = todos }, Cmd.none

        | ApplyEdit txt ->
            state.Todos |> List.map (fun t ->
                match t.Editing with
                | Some _ ->
                    { t with Description = txt; Editing = None }
                | None -> t)
            |> fun todos -> { state with Todos = todos }, Cmd.none


module private Util =
    open App

    let onEnterOrEscape dispatchOnEnter dispatchOnEscape (ev: KeyboardEvent) =
        let el = ev.target :?> HTMLInputElement

        match ev.key with
        | "Enter" ->
            dispatchOnEnter el.value
            el.value <- ""
        | "Escape" ->
            dispatchOnEscape()
            el.value <- ""
            el.blur ()
        | _ -> ()

    [<JSX.Component>]
    let InputField (dispatch: Msg -> unit) =
        let inputRef = Solid.createRef<HTMLInputElement>()
        Div [ "field"; "has-addons" ] [
           Div [ "control"; "is-expanded" ] [
                Html.input [
                    Solid.ref inputRef
                    Attr.classList [ "input"; "is-medium" ]
                    Attr.autoFocus true
                    Ev.onKeyUp (onEnterOrEscape (AddNewTodo >> dispatch) ignore)
                ]
            ]

           Div [ "control" ] [
                Html.button [
                    Attr.classList [
                        "button"
                        "is-primary"
                        "is-medium"
                    ]
                    Ev.onClick (fun _ ->
                        let txt = inputRef.Value.value
                        inputRef.Value.value <- ""
                        txt |> AddNewTodo |> dispatch)
                    Html.children [
                        Html.i [
                            Attr.classList [ "fa"; "fa-plus" ]
                        ]
                    ]
                ]
            ]
        ]

    [<JSX.Component>]
    let Button isVisible dispatch classes (iconClasses: string list) =
        Html.button [
            Attr.typeButton
            Attr.classList [
                "button", true
                "is-invisible", not(isVisible())
                yield! classes
            ]
            Attr.style [
                Css.marginRight(length.px 4)
            ]
            Ev.onClick (fun _ -> dispatch())
            Html.children [
                Html.i [
                    Attr.classList iconClasses
                ]
            ]
        ]

    [<JSX.Component>]
    let TodoView (todo: Todo) dispatch =
        let inputRef = Solid.createRef<HTMLInputElement>()
        let isEditing() = Option.isSome todo.Editing
        let isNotEditing() = Option.isNone todo.Editing

        Solid.createEffect(fun () ->
            if isEditing() then
                inputRef.Value.select()
                inputRef.Value.focus())

        Html.li [
            Attr.className "box"

            Html.children [
                Div [ "columns" ] [
                    Div [ "column"; "is-7" ] [
                        Solid.Show(todo.Editing, (fun editing ->
                            Html.input [
                                Solid.ref inputRef
                                Attr.classList [ "input"; "is-medium" ]
                                Attr.value editing
                                Ev.onKeyUp (onEnterOrEscape (ApplyEdit >> dispatch) (fun _ -> dispatch CancelEdit))
                                Ev.onBlur (fun _ -> dispatch CancelEdit)
                            ]),
                            fallback = Html.p [
                                Attr.className "subtitle"
                                Ev.onDblClick (fun _ -> dispatch (StartEditingTodo todo.Id))
                                Attr.style [
                                    Css.userSelectNone
                                    Css.cursorPointer
                                ]
                                Html.children [
                                    Html.text todo.Description
                                ]
                            ])
                    ]

                    Div [ "column"; "is-4" ] [
                        Button isEditing (fun () -> ApplyEdit inputRef.Value.value |> dispatch)
                            [ "is-primary", true ]
                            [ "fa"; "fa-save" ]

                        Button isNotEditing (fun () -> ToggleCompleted todo.Id |> dispatch)
                            [ "is-success", todo.Completed ]
                            [ "fa"; "fa-check" ]

                        Button isNotEditing (fun () -> StartEditingTodo todo.Id |> dispatch)
                            [ "is-primary", true ]
                            [ "fa"; "fa-edit" ]

                        Button isNotEditing (fun () -> DeleteTodo todo.Id |> dispatch)
                            [ "is-danger", true ]
                            [ "fa"; "fa-times" ]
                    ]
                ]
            ]
        ]

open App
open Util

type Components with
    [<JSX.Component>]
    static member TodoElmish() =
        // Solid can optimize updates better if we only use plain objects and arrays
        // so we create an according transformation to be used in the view
        let todos, dispatch = Solid.createElmishStore(init, update, fun (m: State) -> List.toArray m.Todos)

        Html.fragment [
            Html.p [
                Attr.className "title"
                Html.children [
                    Html.text "Elmish.Solid To-Do List"
                ]
            ]

            InputField dispatch

            Html.ul [
                Html.children [
                    Solid.For(todos, fun todo _ ->
                        TodoView todo dispatch)
                ]
            ]
        ]

(*
module TodoNonElmish =
    // Reuse model, msg and view from the Elmish version
    open TodoElmish

    type Model = Todo[]

    let update (store: SolidStoreSetter<Model>) (msg: Msg) =
        match msg with
        | AddNewTodo txt ->
            store.Update(Array.append [|newTodo txt|])

        | DeleteTodo todoId ->
            store.Update(Array.filter (fun (t: Todo) -> t.Id <> todoId))

        | ToggleCompleted todoId ->
            store.Path
                .Find(fun (t: Todo) -> t.Id = todoId)
                .Map(fun t -> t.Completed)
                .Update(not)

        | StartEditingTodo todoId ->
            // Update all todos because we need to make sure there is not more than one todo being edited at the same time
            store.Update(Array.map (fun (t: Todo) ->
                match t.Editing with
                | _ when t.Id = todoId -> { t with Editing = Some t.Description }
                | Some _ -> { t with Editing = None }
                | _ -> t))

        | CancelEdit ->
            store.Update(Array.map (fun (t: Todo) ->
                if Option.isSome t.Editing
                then { t with Editing = None }
                else t))

        | ApplyEdit txt ->
            store.Path.Find(fun (t: Todo) -> Option.isSome t.Editing).Update(fun t ->
                { t with Description = txt; Editing = None })

    [<JSX.Component>]
    let App() =
        let store, setStore = Solid.createStore(initTodos() |> List.toArray)
        let dispatch = update setStore
        let todos = store.Value

        Html.fragment [
            Html.p [
                Attr.className "title"
                Html.children [
                    Html.text "Solid Store To-Do List"
                ]
            ]

            InputField dispatch

            Html.ul [
                Html.children [
                    Solid.For(todos, fun todo _ ->
                        TodoView todo dispatch)
                ]
            ]
        ]
*)

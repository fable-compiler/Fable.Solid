module Components

open Browser
open Browser.Types
open Feliz.JSX.Solid
open Fable.Core
open Fable.Core.JsInterop

[<JSX.Component>]
let Div (classes: string seq) children =
    Html.div [
        Attr.classList classes
        Html.children children
    ]

[<JSX.Component>]
let Counter() =
    let count, setCount = Solid.createSignal(0)
    let doubled() = count() * 2
    let quadrupled() = doubled() * 2

    Html.fragment [
        Html.p $"{count()} * 2 = {doubled()}"
        Html.p $"{doubled()} * 2 = {quadrupled()}"
        Html.br []
        Html.button [
            Attr.className "button"
            Ev.onClick(fun _ -> count() + 1 |> setCount)
            Html.children [
                Html.text $"Click me!"
            ]
        ]
    ]

[<JSX.Component>]
let Svg() =
    let gradient, setGradient = Solid.createSignal(5)

    // let num, setNum = Solid.createSignal(0)
    // let _ = JS.setInterval (fun () -> num() + 1 % 255 |> setNum) 30

    Html.fragment [
        Html.div [
            Html.input [
                Attr.typeRange
                Attr.min 1
                Attr.max 100
                Attr.value $"{gradient()}"
                Ev.onTextInput (fun (value: string) -> value |> int |> setGradient)
            ]
        ]

        Html.p "This is created using Feliz API"

        Svg.svg [
            Attr.height 150
            Attr.width 300
            Svg.children [
                Svg.defs [
                    Svg.linearGradient [
                        Attr.id "gr1"
                        Attr.x1 (length.perc 0)
                        Attr.y1 (length.perc 60)
                        Attr.x2 (length.perc 100)
                        Attr.y2 (length.perc 0)
                        Svg.children [
                            Svg.stop [
                                Attr.offset (length.perc (gradient()))
                                Attr.style [
                                    "stop-color", "rgb(255,255,3)"
                                    "stop-opacity", "1"
                                ]
                            ]
                            Svg.stop [
                                Attr.offset (length.perc 100)
                                Attr.style [
                                    "stop-color", "rgb(255,0,0)"
                                    "stop-opacity", "1"
                                ]
                            ]
                        ]
                    ]
                ]
                Svg.ellipse [
                    Attr.cx 125
                    Attr.cy 75
                    Attr.rx 100
                    Attr.ry 60
                    Attr.fill "url(#gr1)"
                ]
            ]
        ]

        Html.p "This is created using HTML template"
        // Note the interpolation hole must replace the whole attribute value (as in standard JSX)
        // We cannot interpolate only part of the attribute, e.g. `offset="{gradient()}%%"

        JSX.html $"""
        <svg height="150" width="300">
            <defs>
                <linearGradient id="gr2" x1="0%%" y1="60%%" x2="100%%" y2="0%%">
                <stop offset={ length.perc (gradient()) } style="stop-color:rgb(52, 235, 82);stop-opacity:1" />
                <stop offset="100%%" style="stop-color:rgb(52, 150, 235);stop-opacity:1" />
                </linearGradient>
            </defs>
            <ellipse cx="125" cy="75" rx="100" ry="60" fill="url(#gr2)" />
            Sorry but this browser does not support inline SVG.
        </svg>
        """

        // Html.div [
        //     Attr.style [
        //         Css.color $"rgb({num()}, 180, {num()})"
        //         Css.fontWeight 800
        //         Css.fontSize (length.px(num()))
        //     ]
        //     Html.children [
        //         Html.text $"Number is {num()}"
        //     ]
        // ]
    ]



module Sketch =
    let setStyle (el: HTMLElement) ((key, value): string * string) =
        el?style?(key) <- value

    let maxGridPixelWidth = 500.

    let randomHexColorString(): string =
        let v = JS.Math.random() * 16777215. |> int
        "#" + System.Convert.ToString(v, 16)

    let clampGridSideLength(newSideLength) =
        min (max newSideLength 0.) 100.

    [<JSX.Component>]
    let App(initialSide: float) =
        let gridSideLength, setGridSideLength = Solid.createSignal(initialSide)
        let gridTemplateString = Solid.createMemo(fun () ->
            $"repeat({gridSideLength()}, {maxGridPixelWidth / gridSideLength()}px)")

        Html.fragment [
            Html.div [
                Attr.style [ Css.marginBottom 10 ]
                Html.children [
                    Html.label "Grid Side Length: "
                    Html.input [
                        Attr.typeNumber
                        Attr.value (gridSideLength().ToString())
                        Ev.onInput(fun e ->
                            (e.currentTarget :?> HTMLInputElement).valueAsNumber
                            |> clampGridSideLength
                            |> setGridSideLength
                        )
                    ]
                ]
            ]

            Html.div [
                Attr.style [
                    Css.displayGrid
                    Css.gridTemplateRows [gridTemplateString() |> grid.ofString]
                    Css.gridTemplateColumns [gridTemplateString() |> grid.ofString]
                ]
                Html.children [
                    Solid.Index(
                        each = (Array.init (gridSideLength() ** 2 |> int) id),
                        fallback = (Html.text "Input a grid side length."),
                        children = (fun _ _ ->
                            Html.div [
                                Attr.className "cell"
                                Ev.onMouseEnter(fun ev ->
                                    let el = ev.currentTarget :?> HTMLElement
                                    Css.backgroundColor(randomHexColorString()) |> setStyle el
                                    JS.setTimeout (fun () ->
                                        Css.backgroundColor "initial" |> setStyle el) 500
                                    |> ignore
                                )
                            ]
                        ))
                ]
            ]
        ]

module TodoElmish =
    open System
    open Elmish
    open Elmish.Solid

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

    // View

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
        printfn "Render todo"
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
                            [ "is-success", (
                                printfn "Render complete"
                                todo.Completed
                            ) ]
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

    [<JSX.Component>]
    let App() =
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

module Shoelace =
    // Cherry-pick Shoelace image comparer element, see https://shoelace.style/components/image-comparer?id=importing
    importSideEffects "https://cdn.jsdelivr.net/npm/@shoelace-style/shoelace@2.0.0-beta.73/dist/components/image-comparer/image-comparer.js"
    importSideEffects "https://cdn.jsdelivr.net/npm/@shoelace-style/shoelace@2.0.0-beta.73/dist/components/qr-code/qr-code.js"

    [<JSX.Component>]
    let ImageComparer() =
        let inputRef = Solid.createRef<HTMLInputElement>()
        let position, setPosition = Solid.createSignal(25)

        Html.fragment [
            Html.div [
                Html.input [
                    Solid.ref inputRef
                    Attr.typeRange
                    Attr.min 1
                    Attr.max 100
                    Attr.value (position() |> string)
                    Ev.onTextInput (fun (value: string) -> value |> int |> setPosition)
                ]
            ]

            // We can invoke registered web components "dynamically" without bindings
            JSX.create "sl-image-comparer" [
                "position" ==> position()
                // Solid request that non-native events are prefixed with `on:`
                "on:sl-change" ==> (fun (ev: Event) ->
                    let pos: int = ev.target?position
                    // The input is an "uncontrolled" element so we need to update it manually
                    inputRef.Value.value <- string pos
                    setPosition pos
                )
                Html.children [
                    Html.img [
                        Attr.slot "before"
                        Attr.src "https://images.unsplash.com/photo-1520903074185-8eca362b3dce?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=1200&q=80"
                        Attr.alt "A person sitting on bricks wearing untied boots."
                    ]
                    Html.img [
                        Attr.slot "after"
                        Attr.src "https://images.unsplash.com/photo-1520640023173-50a135e35804?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=2250&q=80"
                        Attr.alt "A person sitting on a yellow curb tying shoelaces on a boot."
                    ]
                ]
            ]
        ]

    [<JSX.Component>]
    let QrCode() =
        let value, setValue = Solid.createSignal("https://shoelace.style/")

        Html.fragment [
            Html.input [
                Attr.className "input mb-5"
                Attr.typeText
                Attr.autoFocus true
                Attr.value (value())
                Ev.onTextChange setValue
            ]
            Html.div [
                JSX.html $"""
                    <sl-qr-code value={value()} radius="0.5"></sl-qr-code>
                """
            ]
        ]

    [<JSX.Component>]
    let App() =
        Html.fragment [
            QrCode()
            Html.br []
            ImageComparer()
        ]

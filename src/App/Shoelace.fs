[<AutoOpen>]
module Shoelace

open Fable.Core
open Browser.Types
open Feliz.JSX.Solid
open Elmish.Solid
open type Components

printfn $"Loading {__SOURCE_FILE__}..."

module private Util =
    open Fable.Core.JsInterop

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

open Util

type Components with
    [<JSX.Component>]
    static member Shoelace() =
        Html.fragment [
            QrCode()
            Html.br []
            ImageComparer()
        ]

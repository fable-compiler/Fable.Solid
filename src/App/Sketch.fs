[<AutoOpen>]
module Sketch

open Fable.Core
open Browser.Types
open Feliz.JSX.Solid

printfn $"Loading {__SOURCE_FILE__}..."

module private Util =
    open Fable.Core.JsInterop

    let setStyle (el: HTMLElement) ((key, value): string * string) =
        el?style?(key) <- value

    let maxGridPixelWidth = 500.

    let randomHexColorString(): string =
        let v = JS.Math.random() * 16777215. |> int
        "#" + System.Convert.ToString(v, 16)

    let clampGridSideLength(newSideLength) =
        min (max newSideLength 0.) 100.

open Util

type Components with
    [<JSX.Component>]
    static member Sketch() =
        let gridSideLength, setGridSideLength = Solid.createSignal(10.)
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

[<AutoOpen>]
module Svg

open Fable.Core
open Feliz.JSX.Solid

printfn $"Loading {__SOURCE_FILE__}..."

type Components with

    [<JSX.Component>]
    static member Svg() =
        let gradient, setGradient = Solid.createSignal (5)

        // let num, setNum = Solid.createSignal(0)
        // let _ = JS.setInterval (fun () -> num() + 1 % 255 |> setNum) 30

        Html.fragment [
            Html.div [
                Html.input [
                    Attr.typeRange
                    Attr.min 1
                    Attr.max 100
                    Attr.value $"{gradient ()}"
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
                                    Attr.offset (length.perc (gradient ()))
                                    Attr.style [ "stop-color", "rgb(255,255,3)"; "stop-opacity", "1" ]
                                ]
                                Svg.stop [
                                    Attr.offset (length.perc 100)
                                    Attr.style [ "stop-color", "rgb(255,0,0)"; "stop-opacity", "1" ]
                                ]
                            ]
                        ]
                    ]
                    Svg.ellipse [ Attr.cx 125; Attr.cy 75; Attr.rx 100; Attr.ry 60; Attr.fill "url(#gr1)" ]
                ]
            ]

            Html.p "This is created using HTML template"
            // Note the interpolation hole must replace the whole attribute value (as in standard JSX)
            // We cannot interpolate only part of the attribute, e.g. `offset="{gradient()}%%"

            JSX.html
                $"""
            <svg height="150" width="300">
                <defs>
                    <linearGradient id="gr2" x1="0%%" y1="60%%" x2="100%%" y2="0%%">
                    <stop offset={length.perc (gradient ())} style="stop-color:rgb(52, 235, 82);stop-opacity:1" />
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

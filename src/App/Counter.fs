[<AutoOpen>]
module Counter

open System
open Fable.Core
open Feliz.JSX.Solid
open Browser.Types

open type Components

printfn $"Loading {__SOURCE_FILE__}..."

type Components with

    [<JSX.Component>]
    static member DivideBy() =
        let dividend, setDividend = Solid.createSignal (8.)
        let divisor, setDivisor = Solid.createSignal (2.)

        let fallback (err: exn) (reset: Action) =
            JSX.jsx
                $"""
            <>
                <p>Error: {err.Message}</p>
                <button class="button" onClick={fun _ ->
                                                    // Make sure the divisor is corrected before resetting
                                                    setDivisor 1.
                                                    reset.Invoke()}>Reset</button>
            </>
            """

        // JS doesn't throw when dividing by error, so throw the error
        let divide (x: float) (y: float) =
            if y = 0. then
                failwith "Cannot divide by zero"

            x / y

        Solid.ErrorBoundary(
            fallback,
            JSX.jsx
                $"""
            <p>Try dividing by 0 to trigger an error</p>
            <br />
            <p>{FloatInput(dividend (), setDividend)} divided by {FloatInput(divisor (), setDivisor, defVal = 1.)} is {divide (dividend ()) (divisor ())}</p>
        """
        )

    [<JSX.Component>]
    static member Counter() =
        let count, setCount = Solid.createSignal (0)
        let doubled () = count () * 2
        let quadrupled () = doubled () * 2

        Html.fragment
            [
                Html.p $"{count ()} * 2 = {doubled ()}"
                Html.p $"{doubled ()} * 2 = {quadrupled ()}"
                Html.br []
                Html.button
                    [
                        Attr.className "button"
                        Ev.onClick (fun _ -> count () + 1 |> setCount)
                        Html.children [ Html.text $"Click me!" ]
                    ]

                Html.hr []
                Components.DivideBy()
            ]

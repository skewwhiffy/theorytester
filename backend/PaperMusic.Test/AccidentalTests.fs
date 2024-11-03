module PaperMusic.Test.AccidentalTests

open Faqt
open PaperMusic
open Xunit

[<Fact>]
let ``can sharpen flat`` () =
    let flat = Accidental.Flat
    let expected = Accidental.Natural

    let sharpened = flat.Sharpen()

    sharpened.Should().Be(expected)

let shortStringTestData: obj[] list =
    [ [| Accidental.Natural; "" |]
      [| Accidental.Sharp; "#" |]
      [| Accidental.Sharp.Sharpen(); "x" |]
      [| Accidental.Sharp.Sharpen().Sharpen(); "x#" |]
      [| Accidental.Flat; "b" |]
      [| Accidental.Flat.Flatten(); "bb" |] ]

[<Theory>]
[<MemberData(nameof shortStringTestData)>]
let ``short string correct`` (accidental: Accidental, expected: string) =
    let actual = accidental.ShortString

    actual.Should().Be(expected)

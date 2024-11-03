module PaperMusic.Test.NoteTest

open PaperMusic
open Xunit
open Faqt

[<Fact>]
let ``can sharpen flat`` () =
    let flat = Note.C.Flat
    let expected = Note.C

    let sharpened = flat.Sharp

    sharpened.Should().Be(expected)

let shortStringTestData: obj[] list =
    [ [| Note.C; "C" |]
      [| Note.D.Sharp; "D#" |]
      [| Note.E.Flat; "Eb" |] ]

[<Theory>]
[<MemberData(nameof shortStringTestData)>]
let ``short string correct`` (note: Note, expected: string) =
    let actual = note.ShortString

    actual.Should().Be(expected)

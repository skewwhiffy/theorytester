module PaperMusic.Test.ScaleStart

open PaperMusic
open Xunit
open Faqt

[<Fact>]
let ``can construct major scale`` () =
    let expected =
        [ NoteName.C
          NoteName.D
          NoteName.E
          NoteName.F
          NoteName.G
          NoteName.A
          NoteName.B ]
        |> List.map (fun it -> Note(it, Accidental.Natural))
        |> List.map (fun it -> Pitch(it, Octave.Default))
        |> fun it -> it @ [ Pitch(Note.C, Octave.Default.Up) ]

    let actual = Scale.Major.StartingWith(Pitch.MiddleC)

    actual.Ascending.Should().Be(expected)

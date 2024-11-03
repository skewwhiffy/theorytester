module PitchTests

open PaperMusic
open Xunit
open Faqt

[<Fact>]
let ``can sharpen flat`` () =
    let flat = Pitch.MiddleC.Flat
    let expected = Pitch.MiddleC

    let sharpened = flat.Sharp

    sharpened.Should().Be(expected)

let minorSecondTestData: obj[] list =
    [ [| Pitch.MiddleC; Pitch(Note.D.Flat, Octave.Default) |]
      [| Pitch(Note.D.Sharp, Octave.Default); Pitch(Note.E, Octave.Default) |]
      [| Pitch(Note.B.Flat, Octave.Default); Pitch(Note.C.Flat, Octave.Default.Up) |] ]

[<Theory>]
[<MemberData(nameof minorSecondTestData)>]
let ``can add minor second`` (original: Pitch, expected: Pitch) =
    let actual = original + Interval.Minor.Second

    actual.Should().Be(expected)

let shortStringTestData: obj[] list =
    [
      [| Pitch.MiddleC; Pitch(Note.D.Flat, Octave.Default); Interval.Minor.Second |]
      [| Pitch.MiddleC; Pitch(Note.D, Octave.Default); Interval.Major.Second |]
      [| Pitch.MiddleC; Pitch(Note.E.Flat, Octave.Default); Interval.Minor.Third |]
      [| Pitch.MiddleC
         Pitch(Note.F.Sharp, Octave.Default)
         Interval.Augmented.Fourth |]
      [| Pitch.MiddleC
         Pitch(Note.G.Flat, Octave.Default)
         Interval.Diminished.Fifth |]
      [| Pitch.MiddleC
         Pitch(Note.A.Sharp, Octave.Default)
         Interval.Augmented.Sixth |]
      [| Pitch.MiddleC
         Pitch(Note.B.Flat.Flat, Octave.Default)
         Interval.Diminished.Seventh |]
      [| Pitch.MiddleC; Pitch(Note.C, Octave.Default.Up); Interval.Perfect.Octave |]
      [| Pitch(Note.B, Octave.Default)
         Pitch(Note.C, Octave.Default.Up)
         Interval.Minor.Second |]
      [| Pitch.MiddleC
         Pitch(Note.D.Flat, Octave.Default.Up)
         Interval.Compound.Minor.Second |] ]

[<Theory>]
[<MemberData(nameof shortStringTestData)>]
let ``can add`` (original: Pitch, expected: Pitch, interval: Interval) =
    let actual = original + interval

    actual.Should().Be(expected)

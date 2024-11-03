module IntervalTest

open PaperMusic
open Xunit
open Faqt

[<Fact>]
let ``can diminish and augment interval`` () =
    let major = Interval.Major.Third
    let expected = Interval.Minor.Third

    let actual = major.Diminish()

    actual.Should().Be(expected)

let shortStringTestData: obj[] list =
    [ [| Interval.Major.Third; "major 3rd" |]
      [| Interval.Minor.Sixth; "minor 6th" |]
      [| Interval.Compound.Major.Second; "compound major 2nd" |]
      [| Interval.Compound.Compound.Minor.Second; "double compound minor 2nd" |]
      [| Interval.Compound.Compound.Compound.Perfect.Fourth; "3x compound perfect 4th" |]
      [| Interval.Perfect.Fourth; "perfect 4th" |]
      [| Interval.Diminished.Third; "diminished 3rd" |]
      [| Interval.Diminished.Fourth; "diminished 4th" |]
      [| Interval.Augmented.Fifth; "augmented 5th" |]
      [| Interval.Diminished.Sixth.Diminish(); "doubly diminished 6th" |]
      [| Interval.Diminished.Sixth.Diminish().Diminish(); "3x diminished 6th" |]
      [| Interval.Diminished.Fifth.Diminish(); "doubly diminished 5th" |]
      [| Interval.Diminished.Fifth.Diminish().Diminish(); "3x diminished 5th" |]
      [| Interval.Augmented.Seventh.Augment(); "doubly augmented 7th" |] ]

[<Theory>]
[<MemberData(nameof shortStringTestData)>]
let ``shortString works`` (interval: Interval, expected: string) =
    let actual = interval.ShortString

    actual.Should().Be(expected)

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
      [| Interval.Perfect.Fourth; "perfect 4th" |] ]
    // TODO:
    // - Diminished (from perfect and minor)
    // - Augmented
    // - Doubly diminished (from perfect and minor)
    // - Doubly augmented
    // - 5x diminished (from perfect and minor)
    // - 5x augmented

[<Theory>]
[<MemberData(nameof shortStringTestData)>]
let ``shortString works`` (interval: Interval, expected: string) =
    let actual = interval.ShortString

    actual.Should().Be(expected)

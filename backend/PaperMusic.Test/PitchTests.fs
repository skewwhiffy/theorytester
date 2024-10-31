module PitchTests

open PaperMusic
open Xunit
open Faqt

[<Fact>]
let ``can sharpen flat``() =
    let flat = Pitch.MiddleC.Flat
    let expected = Pitch.MiddleC
    
    let sharpened = flat.Sharp
    
    sharpened.Should().Be(expected)
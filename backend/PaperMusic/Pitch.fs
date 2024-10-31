namespace PaperMusic

open System
open System.Text.Json.Serialization

type Pitch(note: Note, octave: Octave) =
    static member MiddleC = Pitch(Note.C, Octave.Default)
    member this.Note = note
    member this.Octave = octave

    [<JsonIgnore>]
    member this.Flat = Pitch(note.Flat, octave)

    [<JsonIgnore>]
    member this.Sharp = Pitch(note.Sharp, octave)

    override this.Equals other =
        match other with
        | :? Pitch as otherPitch -> otherPitch.Note.Equals(note) && otherPitch.Octave.Equals(octave)
        | _ -> false

    override this.GetHashCode() = HashCode.Combine(note, octave)

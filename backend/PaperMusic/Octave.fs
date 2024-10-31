namespace PaperMusic

open System.Text.Json.Serialization

type Octave(offset: int) =
    static member Default = Octave(1)
    member this.Offset = offset

    [<JsonIgnore>]
    member this.Up = Octave(offset + 1)

    [<JsonIgnore>]
    member this.Down = Octave(offset - 1)

    override this.Equals other =
        match other with
        | :? Octave as otherOctave -> otherOctave.Offset.Equals(offset)
        | _ -> false

    override this.GetHashCode() = offset.GetHashCode()

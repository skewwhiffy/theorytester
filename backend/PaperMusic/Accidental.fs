namespace PaperMusic

open System.Text.Json.Serialization

type Accidental(offset: int) =
    static member Flat = Accidental(-1)
    static member Natural = Accidental(0)
    static member Sharp = Accidental(1)

    member this.Offset = offset

    member this.Flatten() = Accidental(offset - 1)

    member this.Sharpen() = Accidental(offset + 1)

    [<JsonIgnore>]
    member this.ShortString =
        match offset with
        | 1 -> "#"
        | 0 -> ""
        | _ when offset > 1 -> "x" + this.Flatten().Flatten().ShortString
        | _ when offset < 0 -> "b" + this.Sharpen().ShortString
        | _ -> failwith "Maths has failed"

    override this.Equals other =
        match other with
        | :? Accidental as otherAccidental -> otherAccidental.Offset.Equals(offset)
        | _ -> false

    override this.GetHashCode() = offset.GetHashCode()

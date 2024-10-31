namespace PaperMusic

open System

type NoteName =
    | A
    | B
    | C
    | D
    | E
    | F
    | G

type Accidental(offset: int) =
    static member Flat = Accidental(-1)
    static member Natural = Accidental(0)
    static member Sharp = Accidental(1)

    member this.Offset = offset
    member this.Flatten() = Accidental(offset - 1)
    member this.Sharpen() = Accidental(offset + 1)
    member this.ShortString
        with get() =
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

type Note(name: NoteName, accidental: Accidental) =
    static member A = Note(A, Accidental.Natural)
    static member B = Note(B, Accidental.Natural)
    static member C = Note(C, Accidental.Natural)
    static member D = Note(D, Accidental.Natural)
    static member E = Note(E, Accidental.Natural)
    static member F = Note(F, Accidental.Natural)
    static member G = Note(G, Accidental.Natural)

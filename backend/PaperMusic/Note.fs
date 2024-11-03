namespace PaperMusic

open System
open System.Text.Json.Serialization

type NoteName =
    | A
    | B
    | C
    | D
    | E
    | F
    | G

type Note(name: NoteName, accidental: Accidental) =
    static member A = Note(A, Accidental.Natural)
    static member B = Note(B, Accidental.Natural)
    static member C = Note(C, Accidental.Natural)
    static member D = Note(D, Accidental.Natural)
    static member E = Note(E, Accidental.Natural)
    static member F = Note(F, Accidental.Natural)
    static member G = Note(G, Accidental.Natural)
    member this.Name = name
    member this.Accidental = accidental
    member this.ShortString = $"{name}{accidental.ShortString}"
    [<JsonIgnore>]
    member this.Flat = Note(name, accidental.Flatten())
    [<JsonIgnore>]
    member this.Sharp = Note(name, accidental.Sharpen())

    override this.Equals other =
        match other with
        | :? Note as otherNote -> otherNote.Name.Equals(name) && otherNote.Accidental.Equals(accidental)
        | _ -> false

    override this.GetHashCode() = HashCode.Combine(name, accidental)
    override this.ToString() = this.ShortString

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
    override this.ToString() = $"{this.Note}{this.Octave}"

type Pitch with
    static member (+)(pitch: Pitch, interval: Interval) =
        let defaultNewPitch =
            match interval.Size with
            | 1u -> pitch
            | 2u -> pitch.addMajorSecond ()
            | 3u -> pitch.addMajorSecond().addMajorSecond ()
            | 4u -> pitch + Interval.Major.Third + Interval.Minor.Second
            | 5u -> pitch + Interval.Perfect.Fourth + Interval.Major.Second
            | 6u -> pitch + Interval.Perfect.Fifth + Interval.Major.Second
            | 7u -> pitch + Interval.Major.Sixth + Interval.Major.Second
            | 8u -> Pitch(pitch.Note, pitch.Octave.Up)
            | _ -> pitch + Interval.Perfect.Octave + Interval(interval.Size - 7u, interval.Quality)

        if interval.Size > 8u then
            defaultNewPitch
        else
            let newOffset = defaultNewPitch.Note.Accidental.Offset + interval.Quality.Offset
            let newAccidental = Accidental(newOffset)
            let newNote = Note(defaultNewPitch.Note.Name, newAccidental)
            Pitch(newNote, defaultNewPitch.Octave)

    member private this.addMinorSecond() =
        let accidental = this.Note.Accidental

        let newAccidental =
            match this.Note.Name with
            | A
            | C
            | D
            | F
            | G -> accidental.Flatten()
            | B
            | E -> accidental

        let newNoteName =
            match this.Note.Name with
            | A -> B
            | B -> C
            | C -> D
            | D -> E
            | E -> F
            | F -> G
            | G -> A

        let newNote = Note(newNoteName, newAccidental)

        let newPitch =
            Pitch(
                newNote,
                if this.Note.Name.Equals(B) then
                    this.Octave.Up
                else
                    this.Octave
            )

        newPitch

    member private this.addMajorSecond() = this.addMinorSecond().Sharp

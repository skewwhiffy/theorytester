namespace PaperMusic

open System
open System.Text.Json.Serialization

type ScaleType =
    | Major
    | MinorHarmonic
    | MinorMelodic

type ScaleStep =
    | ToneAndSemitone
    | Tone
    | Semitone

type DiatonicScaleStep = ScaleStep list

type ScaleDefinition(ascendingSteps: DiatonicScaleStep, descendingSteps: DiatonicScaleStep) =
    private new(steps: DiatonicScaleStep) = ScaleDefinition(steps, steps)
    new([<ParamArray>] steps: ScaleStep array) = ScaleDefinition(Array.toList (steps))
    member this.AscendingSteps = ascendingSteps
    member this.DescendingSteps = descendingSteps

type ScaleTypeDefinition =
    { scaleType: string
      definition: ScaleDefinition }

    member this.StartingWith(lowestNote: Pitch) = Scale(lowestNote, this)

and Scale(start: Pitch, definition: ScaleTypeDefinition) =
    static member Major =
        ScaleDefinition(Tone, Tone, Semitone, Tone, Tone, Tone, Semitone)
        |> fun it -> { scaleType = "major"; definition = it }

    static member MinorHarmonic =
        ScaleDefinition(Tone, Semitone, Tone, Tone, Semitone, ToneAndSemitone, Semitone)
        |> fun it ->
            { scaleType = "minor harmonic"
              definition = it }

    static member MinorMelodic =
        ScaleDefinition(
            [ Tone; Semitone; Tone; Tone; Tone; Tone; Semitone ],
            [ Tone; Semitone; Tone; Tone; Semitone; Tone; Tone ]
        )

    [<JsonIgnore>]
    member this.Start = start

    member this.Ascending: Pitch list =
        definition.definition.AscendingSteps
        |> List.fold
            (fun next prev ->
                next
                @ [ match prev with
                    | Tone -> List.last (next) + Interval.Major.Second
                    | Semitone -> List.last (next) + Interval.Minor.Second
                    | ToneAndSemitone -> List.last (next) + Interval.Augmented.Second
                    | scaleSteps -> failwith "todo" ])
            [ this.Start ]

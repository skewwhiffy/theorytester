namespace PaperMusic

open System
open System.Text.Json.Serialization

type IntervalQuality(offsetFromDefault: int) =
    member this.Offset = offsetFromDefault

    [<JsonIgnore>]
    member this.Augmented = IntervalQuality(offsetFromDefault + 1)

    [<JsonIgnore>]
    member this.Diminished = IntervalQuality(offsetFromDefault - 1)

    override this.Equals other =
        match other with
        | :? IntervalQuality as otherQuality -> otherQuality.Offset.Equals(offsetFromDefault)
        | _ -> false

    override this.GetHashCode() = offsetFromDefault.GetHashCode()

type IQualityFactory =
    abstract Major: INonPerfectFactory
    abstract Minor: INonPerfectFactory
    abstract Perfect: IPerfectFactory
    abstract Diminished: IFactory
    abstract Augmented: IFactory

and IFactory =
    abstract Unison: Interval
    abstract Second: Interval
    abstract Third: Interval
    abstract Fourth: Interval
    abstract Fifth: Interval
    abstract Sixth: Interval
    abstract Seventh: Interval
    abstract Octave: Interval

and IPerfectFactory =
    abstract Unison: Interval
    abstract Fourth: Interval
    abstract Fifth: Interval
    abstract Octave: Interval

and INonPerfectFactory =
    abstract Second: Interval
    abstract Third: Interval
    abstract Sixth: Interval
    abstract Seventh: Interval

and Interval(size: int, quality: IntervalQuality) =
    static member wrap(wrapped: INonPerfectFactory, transform: Interval -> Interval) =
        { new INonPerfectFactory with
            member this.Second = transform wrapped.Second
            member this.Third = transform wrapped.Third
            member this.Sixth = transform wrapped.Sixth
            member this.Seventh = transform wrapped.Seventh }

    static member wrap(wrapped: IPerfectFactory, transform: Interval -> Interval) =
        { new IPerfectFactory with
            member this.Unison = transform wrapped.Unison
            member this.Fourth = transform wrapped.Fourth
            member this.Fifth = transform wrapped.Fifth
            member this.Octave = transform wrapped.Octave }

    static member wrap(wrapped: IFactory, transform: Interval -> Interval) =
        { new IFactory with
            member this.Unison = transform wrapped.Unison
            member this.Second = transform wrapped.Second
            member this.Third = transform wrapped.Third
            member this.Fourth = transform wrapped.Fourth
            member this.Fifth = transform wrapped.Fifth
            member this.Sixth = transform wrapped.Sixth
            member this.Seventh = transform wrapped.Seventh
            member this.Octave = transform wrapped.Octave }

    static member wrap(perfect: IPerfectFactory, nonPerfect: INonPerfectFactory) =
        { new IFactory with
            member this.Unison = perfect.Unison
            member this.Second = nonPerfect.Second
            member this.Third = nonPerfect.Third
            member this.Fourth = perfect.Fourth
            member this.Fifth = perfect.Fifth
            member this.Sixth = nonPerfect.Sixth
            member this.Seventh = nonPerfect.Seventh
            member this.Octave = perfect.Octave }

    static member Compound =
        let compound: Interval -> Interval = fun it -> Interval(it.Size + 7, it.Quality)

        { new IQualityFactory with
            member this.Perfect = Interval.wrap (Interval.Perfect, compound)

            member this.Augmented = Interval.wrap (Interval.Augmented, compound)
            member this.Major = Interval.wrap (Interval.Major, compound)
            member this.Diminished = Interval.wrap (Interval.Diminished, compound)
            member this.Minor = Interval.wrap (Interval.Minor, compound) }

    static member Perfect: IPerfectFactory =
        let perfect = IntervalQuality(0)

        { new IPerfectFactory with
            member this.Unison = Interval(1, perfect)
            member this.Fourth = Interval(4, perfect)
            member this.Fifth = Interval(5, perfect)
            member this.Octave = Interval(8, perfect) }

    static member Major: INonPerfectFactory =
        let major = IntervalQuality(0)

        { new INonPerfectFactory with
            member this.Second = Interval(2, major)
            member this.Third = Interval(3, major)
            member this.Sixth = Interval(6, major)
            member this.Seventh = Interval(7, major) }


    static member Minor: INonPerfectFactory = Interval.wrap (Interval.Major, _.Diminish())

    static member Diminished: IFactory =
        Interval.wrap (Interval.wrap (Interval.Perfect, _.Diminish()), Interval.wrap (Interval.Minor, _.Diminish()))

    static member Augmented: IFactory =
        Interval.wrap (Interval.wrap (Interval.Perfect, _.Augment()), Interval.wrap (Interval.Major, _.Augment()))

    member this.Size = size
    member this.Quality = quality
    member this.Diminish() = Interval(size, quality.Diminished)
    member this.Augment() = Interval(size, quality.Augmented)

    override this.Equals other =
        match other with
        | :? Interval as otherInterval -> otherInterval.Size.Equals(size) && otherInterval.Quality.Equals(quality)
        | _ -> false

    override this.GetHashCode() = HashCode.Combine(size, quality)

type Interval with
    [<JsonIgnore>]
    member this.ShortString =
        if (this.Size > 8) then
            $"compound {Interval(this.Size - 7, this.Quality).ShortString}"
        else
            let ordinal =
                match this.Size with
                | 1 -> string this.Size + "st"
                | 2 -> string this.Size + "nd"
                | 3 -> string this.Size + "rd"
                | _ -> string this.Size + "th"

            let positiveQuality (amount: int) =
                match amount with
                | 1 -> "augmented"
                | 2 -> "doubly augmented"
                | _ -> $"TODO {amount}"

            let negativeQuality (amount: int) =
                if amount.Equals(0) then
                    match this.Size with
                    | 1
                    | 4
                    | 5
                    | 8 -> "perfect"
                    | _ -> "major"
                else
                    let diminishedAmount =
                        match this.Size with
                        | 1
                        | 4
                        | 5
                        | 8 -> amount
                        | _ -> amount - 1

                    match diminishedAmount with
                    | 0 -> "minor"
                    | 1 -> "diminished"
                    | 2 -> "doubly diminished"
                    | _ -> $"TODO {diminishedAmount}"

            let quality =
                match this.Quality.Offset with
                | it when it > 0 -> positiveQuality it
                | it -> negativeQuality -it

            $"{quality} {ordinal}"

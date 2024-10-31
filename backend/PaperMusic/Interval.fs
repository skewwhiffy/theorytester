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

type IFactory =
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

and WrappingPerfectIntervalFactory(wrapped: IPerfectFactory, transform: Interval -> Interval) =
    interface IPerfectFactory with
        member this.Unison = transform wrapped.Unison
        member this.Fourth = transform wrapped.Fourth
        member this.Fifth = transform wrapped.Fifth
        member this.Octave = transform wrapped.Octave

and WrappingNonPerfectIntervalFactory(wrapped: INonPerfectFactory, transform: Interval -> Interval) =
    interface INonPerfectFactory with
        member this.Second = transform wrapped.Second
        member this.Third = transform wrapped.Third
        member this.Sixth = transform wrapped.Sixth
        member this.Seventh = transform wrapped.Seventh

and Interval(size: int, quality: IntervalQuality) =
    static member Perfect: IPerfectFactory = PerfectFactory()
    static member Major: INonPerfectFactory = MajorFactory()

    static member Minor: INonPerfectFactory =
        WrappingNonPerfectIntervalFactory(Interval.Major, _.Diminish())

    static member Diminished: IFactory = DiminishedFactory()
    static member Augmented: IFactory = AugmentedFactory()

    member this.Size = size
    member this.Quality = quality
    member this.Diminish() = Interval(size, quality.Diminished)
    member this.Augment() = Interval(size, quality.Augmented)


    override this.Equals other =
        match other with
        | :? Interval as otherInterval -> otherInterval.Size.Equals(size) && otherInterval.Quality.Equals(quality)
        | _ -> false

    override this.GetHashCode() = HashCode.Combine(size, quality)


and MajorFactory() =
    member private this.Major = IntervalQuality(0)

    interface INonPerfectFactory with
        member this.Second = Interval(2, this.Major)
        member this.Third = Interval(3, this.Major)
        member this.Sixth = Interval(6, this.Major)
        member this.Seventh = Interval(7, this.Major)

and PerfectFactory() =
    member private this.Perfect = IntervalQuality(0)

    interface IPerfectFactory with
        member this.Unison = Interval(1, this.Perfect)
        member this.Fourth = Interval(4, this.Perfect)
        member this.Fifth = Interval(5, this.Perfect)
        member this.Octave = Interval(8, this.Perfect)

and DiminishedFactory() =
    static member private perfect: IPerfectFactory =
        WrappingPerfectIntervalFactory(Interval.Perfect, _.Diminish())

    static member private minor: INonPerfectFactory =
        WrappingNonPerfectIntervalFactory(Interval.Minor, _.Diminish())

    interface IFactory with
        member this.Unison = DiminishedFactory.perfect.Unison
        member this.Second = DiminishedFactory.minor.Second
        member this.Third = DiminishedFactory.minor.Third
        member this.Fourth = DiminishedFactory.perfect.Fourth
        member this.Fifth = DiminishedFactory.perfect.Fifth
        member this.Sixth = DiminishedFactory.minor.Sixth
        member this.Seventh = DiminishedFactory.minor.Seventh
        member this.Octave = DiminishedFactory.perfect.Octave

and AugmentedFactory() =
    static member private perfect: IPerfectFactory =
        WrappingPerfectIntervalFactory(Interval.Perfect, _.Augment())

    static member private minor: INonPerfectFactory =
        WrappingNonPerfectIntervalFactory(Interval.Minor, _.Augment())

    interface IFactory with
        member this.Unison = AugmentedFactory.perfect.Unison
        member this.Second = AugmentedFactory.minor.Second
        member this.Third = AugmentedFactory.minor.Third
        member this.Fourth = AugmentedFactory.perfect.Fourth
        member this.Fifth = AugmentedFactory.perfect.Fifth
        member this.Sixth = AugmentedFactory.minor.Sixth
        member this.Seventh = AugmentedFactory.minor.Seventh
        member this.Octave = AugmentedFactory.perfect.Octave

type Interval with
    [<JsonIgnore>]
    member this.ShortString =
        let ordinal =
            match if this.Size > 20 then this.Size % 10 else this.Size % 20 with
            | 1 -> string this.Size + "st"
            | 2 -> string this.Size + "nd"
            | 3 -> string this.Size + "rd"
            | _ -> string this.Size + "th"

        let positiveQuality (amount: int) =
            match amount with
            | 1 -> "augmented"
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
                | _ -> $"TODO {diminishedAmount}"

        let quality =
            match this.Quality.Offset with
            | it when it > 0 -> positiveQuality (it)
            | it -> negativeQuality (-it)

        $"{quality} {ordinal}"

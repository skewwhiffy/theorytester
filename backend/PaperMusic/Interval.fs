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

type PerfectIntervalFactory =
    abstract Unison: Interval
    abstract Fourth: Interval
    abstract Fifth: Interval
    abstract Octave: Interval

and NonPerfectIntervalFactory =
    abstract Second: Interval
    abstract Third: Interval
    abstract Sixth: Interval
    abstract Seventh: Interval

and WrappingPerfectIntervalFactory(wrapped: PerfectIntervalFactory, transform: Interval -> Interval) =
    interface PerfectIntervalFactory with
        member this.Unison = transform wrapped.Unison
        member this.Fourth = transform wrapped.Fourth
        member this.Fifth = transform wrapped.Fifth
        member this.Octave = transform wrapped.Octave

and WrappingNonPerfectIntervalFactory(wrapped: NonPerfectIntervalFactory, transform: Interval -> Interval) =
    interface NonPerfectIntervalFactory with
        member this.Second = transform wrapped.Second
        member this.Third = transform wrapped.Third
        member this.Sixth = transform wrapped.Sixth
        member this.Seventh = transform wrapped.Seventh

and Interval(size: int, quality: IntervalQuality) =
    static member Perfect: PerfectIntervalFactory = PerfectFactory()
    static member Major: NonPerfectIntervalFactory = MajorFactory()

    static member Minor: NonPerfectIntervalFactory =
        WrappingNonPerfectIntervalFactory(Interval.Major, _.Diminish())

    member this.Size = size
    member this.Quality = quality
    member this.Diminish() = Interval(size, quality.Diminished)

    [<JsonIgnore>]
    member this.ShortString =
        let ordinal n =
            match if n > 20 then n % 10 else n % 20 with
            | 1 -> string n + "st"
            | 2 -> string n + "nd"
            | 3 -> string n + "rd"
            | _ -> string n + "th"

        $"major {ordinal size}"

    override this.Equals other =
        match other with
        | :? Interval as otherInterval -> otherInterval.Size.Equals(size) && otherInterval.Quality.Equals(quality)
        | _ -> false

    override this.GetHashCode() = HashCode.Combine(size, quality)


and MajorFactory() =
    member private this.Major = IntervalQuality(0)

    interface NonPerfectIntervalFactory with
        member this.Second = Interval(2, this.Major)
        member this.Third = Interval(3, this.Major)
        member this.Sixth = Interval(6, this.Major)
        member this.Seventh = Interval(7, this.Major)

and PerfectFactory() =
    member private this.Perfect = IntervalQuality(0)

    interface PerfectIntervalFactory with
        member this.Unison = Interval(1, this.Perfect)
        member this.Fourth = Interval(4, this.Perfect)
        member this.Fifth = Interval(5, this.Perfect)
        member this.Octave = Interval(8, this.Perfect)

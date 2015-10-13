module RelayFoods.Tests.TimeSlot

open System
open RelayFoods
open RelayFoods.Types
open FsCheck
open FsCheck.Xunit
open Microsoft.FSharp.Core.LanguagePrimitives

module TimeSlots =
    let baseDate = DateTime.Today

    let baseGen = 
        Arb.generate<PositiveInt * PositiveInt>
        |> Gen.suchThat (fun (days, hrs) -> days.Get <= 10 && hrs.Get >= 2 && hrs.Get <= 5)
        |> Gen.map (fun (days, hrs) -> 
            let date = baseDate.AddDays(float days.Get)
            {StartTime=date;EndTime=date.AddHours(float hrs.Get)}
        )

    let never = 
        fun ts -> ts, {ts with StartTime=ts.EndTime.AddSeconds(1.0); EndTime=ts.EndTime.AddHours(5.0)}
        <!> baseGen


type FiveHoursTimeSlots =
    static member TimeSlot() =
        let always = 
            fun ts -> ts, {ts with StartTime=ts.StartTime.AddSeconds(1.0)}
            <!> TimeSlots.baseGen

        Gen.oneof [always ; TimeSlots.never]
        |> Arb.fromGen


type OverlappingTimeSlots =
    static member TimeSlot() =
        let always = 
            fun ts -> ts, {ts with StartTime=ts.StartTime.AddMinutes(30.0); EndTime=ts.EndTime.AddSeconds(-1.0)}
            <!> TimeSlots.baseGen
        Gen.oneof [always ; TimeSlots.never]
        |> Arb.fromGen


[<Property(Arbitrary=[| typeof<FiveHoursTimeSlots> |])>]
let ``Time slot includes another when happens inside the range`` (ts1: TimeSlot, ts2: TimeSlot) =
    let inside = ts1.StartTime <= ts2.StartTime && ts1.EndTime >= ts2.EndTime
    inside ==> (ts2 |> TimeSlot.includes ts1)
    |> Prop.classify inside "Inside"
    |> Prop.classify (not inside) "Outside"


[<Property(Arbitrary=[| typeof<OverlappingTimeSlots> |])>]
let ``Time slot overlaps is starts or ends inside the range`` (ts1: TimeSlot, ts2: TimeSlot) =
    let overlaps = ts1.StartTime < ts2.EndTime && ts2.StartTime < ts1.EndTime
    overlaps ==> (ts2 |> TimeSlot.overlaps ts1)

module RelayFoods.Tests.TimeSlot

open System
open RelayFoods
open RelayFoods.Types
open FsCheck
open FsCheck.Xunit
open Microsoft.FSharp.Core.LanguagePrimitives

type TimeSlots =
    static member TimeSlot() =
        let endsAfterStarts (d1, d2) = d2 > d1
        let noMoreThanFiveHoursApart (d1:DateTime, d2:DateTime) = (d2 - d1).Hours <= 5
        let (<&>) f g = (fun x -> f x && g x)

        Arb.generate<DateTime>
        |> Gen.two
        |> Gen.suchThat (endsAfterStarts <&> noMoreThanFiveHoursApart)
        |> Gen.map (fun (d1, d2) -> {StartTime=d1;EndTime=d2})
        |> Arb.fromGen


[<Property(Arbitrary=[| typeof<TimeSlots> |])>]
let ``Time slot includes another when happens inside the range`` (ts1: TimeSlot) (ts2: TimeSlot) =
    let inside = ts1.StartTime <= ts2.StartTime && ts1.EndTime >= ts2.EndTime
    inside ==> (ts2 |> TimeSlot.includes ts1)

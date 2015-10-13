namespace RelayFoods

open System
open Types
open Microsoft.FSharp.Data.UnitSystems.SI.UnitSymbols

module TimeSlot =
    let includes (p1: TimeSlot) (p2: TimeSlot) =
        p1.StartTime <= p2.StartTime &&
        p1.EndTime   >= p2.EndTime

    let overlapsAtLeast (m:int<minutes>) (ts1: TimeSlot) (ts2: TimeSlot) =
        let max = max ts1.StartTime ts2.StartTime
        let min = min ts1.EndTime ts2.EndTime

        (ts1.StartTime < ts2.EndTime && ts2.StartTime < ts1.EndTime) &&
        (min - max).TotalMinutes >= float m


module GeoLocation =
    let private dToR (d:float<degree>) = d * System.Math.PI / 180.0<degree/radian>

    let private cos = dToR >> float >> cos
    let private sin = dToR >> float >> sin
    let private acos = acos >> (*) 1.0<radian>

    let distance (locA:GeoLocation) (locB:GeoLocation) =
        let theta = locA.Long - locB.Long
        let dist = 
            (locA.Lat |> sin) * (locB.Lat |> sin) +
            (locA.Lat |> cos) * (locB.Lat |> cos) * (theta |> cos)
            |> acos
            |> (*) 1.0<km/radian>

        dist * 6371.0 // aprox earth radius

module Delivery = 
    let closestLocation (truckStops: TruckStop list) (customers: Customer list) =
        let firstLoc (customer: Customer) = customer.PreferredPickup |> List.head
        let customer = customers |> Seq.head

        [
            customer, truckStops |> Seq.head
        ]
namespace RelayFoods

open System
open Types
open Microsoft.FSharp.Data.UnitSystems.SI.UnitSymbols

module TimeSlot =
    let includes (ts1: TimeSlot) (ts2: TimeSlot) =
        ts1.StartTime <= ts2.StartTime && ts1.EndTime >= ts2.EndTime

    let overlaps (ts1: TimeSlot) (ts2: TimeSlot) =
        ts1.StartTime < ts2.EndTime && ts2.StartTime < ts1.EndTime


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

    let closestLocation (truckStops: TruckStop list) (customer: Customer) =
        let distToCust = GeoLocation.distance customer.Address.Geo
        let overlapping truck = 
            let oo = customer.PreferredPickup |> List.exists (TimeSlot.overlaps truck.TimeSlot)
            oo
            
        truckStops 
        |> List.sortBy (fun ts -> ts.Geo |> distToCust)
        |> List.tryFind overlapping


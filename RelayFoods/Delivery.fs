namespace RelayFoods

open System
open Types
open Microsoft.FSharp.Data.UnitSystems.SI.UnitSymbols

module TimeSlot =
    let includes (p1: TimeSlot) (p2: TimeSlot) =
        p1.StartTime <= p2.StartTime && p1.EndTime   >= p2.EndTime

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
    let closestLocation (truckStops: TruckStop list) (customers: Customer list) =
        let findClosest customer =
            let distToCust = GeoLocation.distance customer.Address.Geo
            let overlapping truck = 
                customer.PreferredPickup 
                |> List.exists (TimeSlot.overlaps truck.TimeSlot)
            
            truckStops 
            |> List.sortBy (fun ts -> ts.Geo |> distToCust)
            |> List.tryFind overlapping

        customers
        |> List.map (fun c -> c, findClosest c)

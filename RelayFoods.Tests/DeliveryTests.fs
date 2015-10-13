module RelayFoods.Tests.Delivery

open System
open RelayFoods
open RelayFoods.Types
open Xunit
open Xunit.Extensions
open FsUnit
open FsUnit.Xunit

[<AutoOpen>]
module TimeSlots =
    let mkTS2 starts ends = {
        StartTime=DateTime.Today.AddHours(starts)
        EndTime=DateTime.Today.AddHours(ends)
    }
    let mkTS hr = mkTS2 hr (hr+5.0)
    let morning = mkTS 6.0
    let noon = mkTS 12.0
    let evening = mkTS 17.0
    let weeHours = mkTS 0.0

[<AutoOpen>]
module Locations =
    let park       = {Lat=20.0<degree>;Long=0.<degree>}
    let super      = {Lat=40.0<degree>;Long=0.<degree>}
    let parkingLot = {Lat=90.0<degree>;Long=0.<degree>}
    let theatre    = {Lat=5.0<degree>;Long=0.<degree>}

    let nearThe loc = {
        Street = "Some street"
        Number = "2222"
        City = "Starling City"
        State = "NY"
        Geo = {loc with Lat=loc.Lat * 1.5}
    }
        
[<AutoOpen>]
module TruckStops =
    let atThePark       = {Geo=park;TimeSlot=morning}
    let atTheTheatre    = {Geo=theatre;TimeSlot=morning}
    let atTheSuper      = {Geo=super;TimeSlot=evening}
    let atTheParkingLot = {Geo=parkingLot;TimeSlot=noon}

[<AutoOpen>]
module Customers =
    let john = {
        FirstName="John"
        LastName="Lassar"
        Address=(nearThe super)
        PreferredPickup=[noon]
    }

    let sara = {
        FirstName="Sara"
        LastName="Ew"
        Address=(nearThe park)
        PreferredPickup=[morning; evening]
    }

    let simon = {
        FirstName="Simon"
        LastName="Tenn"
        Address=(nearThe theatre)
        PreferredPickup=[morning]
    }

    let nocturn = {
        FirstName="Severus"
        LastName="Snape"
        Address=(nearThe park)
        PreferredPickup=[weeHours]
    }

    let veryBusy = {nocturn with PreferredPickup=[mkTS2 4.0 5.0]}

let locations = box [atThePark; atTheSuper; atTheParkingLot; atTheTheatre]

let customerAndTrucks () =
    seq { 
        yield [| locations; box john ; box atTheParkingLot |]
        yield [| locations; box sara ; box atThePark       |]
        yield [| locations; box simon; box atTheTheatre    |]
    }

[<Theory>]
[<MemberData("customerAndTrucks")>]
let ``The truck stop is the closest to the customer's address`` 
    (truckStops: TruckStop list) (customer: Customer) (expected: TruckStop) =
    
    (truckStops, customer)
    ||> Delivery.closestLocation 
    |> should equal (Some expected)


let notOverlappingTimeSlots() =
    seq { 
        yield [| locations; box nocturn |]
        yield [| locations; box veryBusy |]
    }

[<Theory>]
[<MemberData("notOverlappingTimeSlots")>]
let ``No truck stop is returned if there's no overlapping time slots`` 
    (truckStops: TruckStop list) (customer: Customer) =
    
    (truckStops, customer)
    ||> Delivery.closestLocation 
    |> Option.isNone
    |> should equal true

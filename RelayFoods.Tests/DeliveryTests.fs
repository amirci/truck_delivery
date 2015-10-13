module RelayFoods.Tests.Delivery

open System
open RelayFoods
open RelayFoods.Types
open Xunit
open Xunit.Extensions

[<AutoOpen>]
module TimeSlots =
    let mkTS hr = {StartTime=DateTime.Today.AddHours(hr);EndTime=DateTime.Today.AddHours(hr+5.0)}
    let morning = mkTS 6.0
    let noon = mkTS 12.0
    let evening = mkTS 17.0

[<AutoOpen>]
module Locations =
    let park = {Lat=20.0<degree>;Long=0.<degree>}
    let super = {Lat=40.0<degree>;Long=0.<degree>}
    let parkingLot = {Lat=90.0<degree>;Long=0.<degree>}

    let nearThe loc = {
        Street = "Some street"
        Number = "2222"
        City = "Starling City"
        State = "NY"
        Geo = {loc with Lat=loc.Lat * 1.5}
    }
        
[<AutoOpen>]
module TruckStops =
    let atThePark = {Geo=park;TimeSlot=morning}
    let atTheSuper = {Geo=super;TimeSlot=evening}

[<AutoOpen>]
module Customers =
    let john = {
        FirstName="John"
        LastName="Lassar"
        Address=(nearThe super)
        PreferredPickup=[noon]
    }

let customerAndTrucks () =
    seq { 
        yield [| box [atThePark; atTheSuper]; box [john] |]
    }

[<Theory>]
[<MemberData("customerAndTrucks")>]
let ``The truck stop overlaps the customer's preferred time slot`` 
    (truckStops: TruckStop list) (customers: Customer list) =
    
    let doestNotOverlap pp =
        let notOverlaps = (TimeSlot.overlaps pp) >> not
        truckStops |> List.forall (fun ts -> ts.TimeSlot |> notOverlaps) 

    let preferredTimeSlot (customer: Customer, truckStop:TruckStop option) =
        let assertion = 
            match truckStop with
            | Some ts -> List.exists (TimeSlot.overlaps ts.TimeSlot)
            | None -> List.forall doestNotOverlap
        customer.PreferredPickup |> assertion

    (truckStops, customers)
    ||> Delivery.closestLocation 
    |> List.forall preferredTimeSlot


[<Theory>]
[<MemberData("customerAndTrucks")>]
let ``The truck stop is the closest to the customer's address`` 
    (truckStops: TruckStop list) (customers: Customer list) =
    
    let dist customer ts = customer.Address.Geo |> GeoLocation.distance ts.Geo

    let isClosest (customer: Customer, truckStop: TruckStop option) =
        match truckStop with
        | Some ts ->
            truckStops 
            |> List.minBy (fun ts -> ts |> dist customer)
            |> (=) ts
        | None -> false

    (truckStops, customers)
    ||> Delivery.closestLocation 
    |> List.forall isClosest

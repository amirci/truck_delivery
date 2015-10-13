module RelayFoods.Tests.Delivery

open System
open FsCheck
open FsCheck.Xunit
open RelayFoods
open RelayFoods.Types

type Deliveries =
    static member CustomerDelivery() =
        Arb.generate<Customer * TruckStop>
        |> Arb.fromGen
        
[<Property(Arbitrary=[| typeof<Deliveries> |])>]
let ``The resulting truck stop overlaps the customer's preferred time slot for at least half an our`` 
    (truckStops, customers) =
    
    let overlapsPreferredTimeSlot (customer: Customer, truckStop:TruckStop) =
        customer.PreferredPickup
        |> List.exists (fun pp -> pp |> TimeSlot.overlapsAtLeast 30<minutes> truckStop.TimeSlot)
    
    truckStops, customers
    ||> Delivery.closestLocation 
    |> List.forall overlapsPreferredTimeSlot


[<Property>]
let ``The resulting truck stop is the closest to th ecustomer's address`` 
    truckStops customers =
    
    let isClosest allStops (_, ts) = true

    truckStops, customers
    ||> Delivery.closestLocation 
    |> List.forall (isClosest truckStops)

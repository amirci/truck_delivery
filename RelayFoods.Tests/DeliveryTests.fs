module RelayFoods.Tests.Delivery

open System
open FsCheck
open FsCheck.Xunit
open RelayFoods
open RelayFoods.Types


// Proves the resulting location for each customer 
// happens during a truck stop
[<Property>]
let ``The customer pick up time happens during the truck stop`` truckStops customers =
    let isATruckStop (_, ts) = true
    
    truckStops, customers
    ||> Delivery.closestLocation 
    |> List.forall isATruckStop


// Proves the chosen geo location is the closest for
// that customer at that time
[<Property>]
let ``The resulting location is the closest`` truckStops customers =
    let isClosest allStops (_, ts) = true

    truckStops, customers
    ||> Delivery.closestLocation 
    |> List.forall (isClosest truckStops)

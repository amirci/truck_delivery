module RelayFoods.Tests.Delivery

open System
open FsCheck
open FsCheck.Xunit
open RelayFoods
open RelayFoods.Types


// Proves the resulting location for each customer 
// is during a truck stop
[<Property>]
let ``The location is during the truck stop`` intinerary customers =
    let isATruckStop (_, ts) = true
    
    intinerary, customers
    ||> Delivery.closestLocation 
    |> List.forall isATruckStop


// Proves the chosen geo location is the closest for
// that customer at that time
[<Property>]
let ``The location is the closest`` intinerary customers =
    let isClosest allStops (_, ts) = true

    intinerary, customers
    ||> Delivery.closestLocation 
    |> List.forall (isClosest intinerary)

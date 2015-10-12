module RelayFoods.Tests.Geolocation

open System
open RelayFoods
open RelayFoods.Types
open FsUnit
open NUnit.Framework


// 49.870161, -97.180149
// 6.42 kms
// 49.817468, -97.213108

[<Test>]
let ``The distance is xxxx`` () =
    let locA = {Lat= 49.870161<degree>; Long= -97.180149<degree>}
    let locB = {Lat= 49.817468<degree>; Long= -97.213108<degree>}

    locB 
    |> GeoLocation.distance locA 
    |> should (equalWithin 0.15<km>) 6.42<km>


module RelayFoods.Types

open System
open Microsoft.FSharp.Data.UnitSystems.SI.UnitSymbols

[<Measure>] type degree
[<Measure>] type radian
[<Measure>] type km
    
let mPerKm = 1000.0<m/km>

type GeoLocation = {
    Lat: float<degree>
    Long: float<degree>
}

type Address = {
    Number: string
    Street: string
    City: string
    State: string
    Geo: GeoLocation
}

type TimeSlot = {
    StartTime: DateTime
    EndTime: DateTime
}

type Customer = {
    FirstName: string
    LastName: string
    Address: Address
    PreferredPickup: TimeSlot list
}

type TruckStop = {
    Geo: GeoLocation
    TimeSlot: TimeSlot
}


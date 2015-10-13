module RelayFoods.Types

open System
open Microsoft.FSharp.Data.UnitSystems.SI.UnitSymbols

[<Measure>] type degree
[<Measure>] type radian
[<Measure>] type km
[<Measure>] type minutes
    
let mPerKm = 1000.0<m/km>

type GeoLocation = {
    Lat: float<degree>
    Long: float<degree>
} with
    override this.ToString() =
        sprintf "(%f, %f)" this.Lat this.Long

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
} with
    override this.ToString() = 
        sprintf "from %s to %s" (this.StartTime.ToString("%H:%m")) (this.EndTime.ToString("%H:%m"))

type Customer = {
    FirstName: string
    LastName: string
    Address: Address
    PreferredPickup: TimeSlot list
}

type TruckStop = {
    Geo: GeoLocation
    TimeSlot: TimeSlot
} with
    override this.ToString() =
        sprintf "(Truck at %O -- %O)" this.Geo this.TimeSlot


module RelayFoods.Tests.Geolocation

open System
open RelayFoods
open RelayFoods.Types
open FsCheck
open FsCheck.Xunit
open Microsoft.FSharp.Core.LanguagePrimitives

type GeoLocations =
    static member GeoLocation() =
        let min = -90.0<degree>, -180.0<degree>
        let max = 90.0<degree>, 180.0<degree>
        let round (digits:int) (f:float) = System.Math.Round(f, digits)
        let toGeo (nf: NonNegativeInt) = 
            nf.Get |> float |> FloatWithMeasure<degree>

        let genLatAndLong = 
            Arb.generate<NonNegativeInt> 
            |> Gen.two
            |> Gen.map (fun (f, s) -> (toGeo f), (toGeo s))
            |> Gen.suchThat (fun d -> d >= min && d <= max)

        fun (lat, long) -> {Lat= lat; Long=long}
        <!> genLatAndLong
        |> Arb.fromGen


[<Property(Arbitrary=[| typeof<GeoLocations>|])>]
let ``Latitude and longitude are in range`` (loc: GeoLocation) =
    loc.Lat >= -90.0<degree> && loc.Lat <= 90.0<degree> &&
    loc.Long >= -180.0<degree> && loc.Long <= 180.0<degree>


open FsUnit.Xunit
[<Xunit.Theory>]
[<Xunit.InlineData(49.870161<degree>, -97.180149<degree>, 49.817468<degree>,-97.213108<degree>, 6.32<km>)>]
[<Xunit.InlineData(38.898556<degree>, -77.037852<degree>, 38.897147<degree>,-77.043934<degree>, 0.549<km>)>]
// Pentagon to the whitehouse
[<Xunit.InlineData(38.873353<degree>, -77.056135<degree>, 38.897927<degree>,-77.036551<degree>, 3.217<km>)>]
[<Xunit.InlineData(38.049472<degree>, -78.541887<degree>, 38.054714<degree>,-78.514557<degree>, 2.464<km>)>]
// Puerta del sol al palacio real
[<Xunit.InlineData(40.417797<degree>, -3.703614<degree>, 40.418515<degree>, -3.714421<degree>, 0.919<km>)>]
let ``Expected distance of locations`` latA longA latB longB distance =
  let actual = 
    {Lat=latB; Long=longB} 
    |> GeoLocation.distance {Lat=latA;Long=longA}
  
  actual |> should (equalWithin 0.05<km>) distance

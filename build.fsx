// include Fake lib
#r @"packages/FAKE/tools/FakeLib.dll"

open System.IO
open Fake
open Fake.Testing

RestorePackages()

[<AutoOpen>]
module Config =
    let testDir     = "./"
    let srcDir      = "./"
    let prjName     = "RelayFoods"
    let mainSln     = prjName + ".sln"
    let mainTestPrj = prjName + ".Tests"
    let mainPrj     = prjName
    let mainConfig  = mainPrj @@ "app.config"
    let testConfig  = mainTestPrj @@ "app.config"

    let environments = ["Debug"; "Release"; "CI"]
    let buildMode () = getBuildParamOrDefault "buildMode" "Release"
    let version      = "1.0.0.0"
    let targetWithEnv target env = sprintf "%s:%s" target env

    let setBuildMode = setEnvironVar "buildMode"

    let debugMode   () = setBuildMode "Debug"
    let releaseMode () = setBuildMode "Release"
    let ciMode () = setBuildMode "CI"

    let setParams defaults =
        { defaults with
            Targets = ["Build"]
            Properties =
                [
                    "Optimize", "True"
                    "Platform", "Any CPU"
                    "Configuration", buildMode()
                ]
        }


Target "Help" (fun _ ->
    PrintTargets()
    printf "\nDefault Build:Debug\n\n"
)

Target "Test" (fun _ ->
  !! "**/bin/Debug/*Tests.dll"
  |> xUnit id
)

let addBuildTarget name env sln =
    let rebuild config = {(setParams config) with Targets = ["Build"]}
    Target (targetWithEnv name env) (fun _ ->
        setBuildMode env
        build rebuild sln
    )

environments |> Seq.iter (fun env -> addBuildTarget "Build" env mainSln)

"Build:Debug"
    ==> "Test"

// start build
RunTargetOrDefault (targetWithEnv "Build" "Debug")
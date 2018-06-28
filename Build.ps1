# Make so the script will stop when it hits an error.
$ErrorActionPreference = "Stop"

# Get the executing directory and set it to the current directory.
$scriptBin = ""
Try { $scriptBin = "$(Split-Path -Parent $MyInvocation.MyCommand.Definition)" } Catch {}
If ([string]::IsNullOrEmpty($scriptBin)) { $scriptBin = $pwd }
Set-Location $scriptBin

$version = & git describe --tags;
mkdir "$scriptBin/artifacts"
$outputDir = Resolve-Path "$scriptBin/artifacts";

dotnet --info

dotnet build -c Release $sln;

dotnet test -c Release $sln;

dotnet pack -c Release /p:Version="$version" -o $outputDir


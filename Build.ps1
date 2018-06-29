Param(
  [string]$version
)


# Make so the script will stop when it hits an error.
$ErrorActionPreference = "Stop"

# Get the executing directory and set it to the current directory.
$scriptBin = ""
Try { $scriptBin = "$(Split-Path -Parent $MyInvocation.MyCommand.Definition)" } Catch {}
If ([string]::IsNullOrEmpty($scriptBin)) { $scriptBin = $pwd }
Set-Location $scriptBin

#For local builds, appveyor will be provided version
if($version -eq $null){
	$version = & git describe --tags;
}

$artifacts = "$scriptBin/artifacts";

New-Item -Force -ItemType directory -Path $artifacts
$outputDir = Resolve-Path $artifacts;

dotnet --info

dotnet build -c Release -v m;

dotnet test -c Release -v m --no-build;

dotnet pack -c Release /p:Version="$version" -o $outputDir -v m


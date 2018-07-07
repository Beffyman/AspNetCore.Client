# Make so the script will stop when it hits an error.
$ErrorActionPreference = "Stop"

# Get the executing directory and set it to the current directory.
$scriptBin = ""
Try { $scriptBin = "$(Split-Path -Parent $MyInvocation.MyCommand.Definition)" } Catch {}
If ([string]::IsNullOrEmpty($scriptBin)) { $scriptBin = $pwd }
Set-Location $scriptBin

$version = $env:APPVEYOR_BUILD_VERSION;
#For local builds, appveyor will be provided version
if([System.String]::IsNullOrEmpty($version)){
	$version = & git describe --tags;
}

#Filter out - branch commit locally
if($version -Match "-"){
	$version = $version.Split("-")[0];
}

#Filter out +Build# from CI builds
if($version -Match "\+"){
	$version = $version.Split("+")[0];
}

Write-Host "---Version $version will be used---" -ForegroundColor Magenta;

$artifacts = "$scriptBin/artifacts";
$testGenerator = Resolve-Path "$scriptBin/test/AspNetCore.Client.Test.Generator";

Remove-Item $artifacts -Recurse -ErrorAction Ignore
New-Item -Force -ItemType directory -Path $artifacts
$outputDir = Resolve-Path $artifacts;

Write-Host ">> dotnet --info" -ForegroundColor Magenta;
dotnet --info

Write-Host ">> dotnet build -c Release -v m;" -ForegroundColor Magenta;
dotnet build -c Release -v m;

#Run the test project generators
Push-Location -Path $testGenerator -StackName "Run";
Write-Host ">> dotnet run -c Release -v m;" -ForegroundColor Magenta;
dotnet run -c Release -v m;
Pop-Location -StackName "Run";

#Build again, making sure that our clients that were just regenerated via the previous command build
Write-Host ">> dotnet build -c Release -v m;" -ForegroundColor Magenta;
dotnet build -c Release -v m;

Write-Host ">> dotnet test -c Release -v m;" -ForegroundColor Magenta;
dotnet test -c Release -v m;

Write-Host ">> dotnet pack -c Release /p:Version=$version -o $outputDir -v m" -ForegroundColor Magenta;
dotnet pack -c Release /p:Version="$version" -o $outputDir -v m


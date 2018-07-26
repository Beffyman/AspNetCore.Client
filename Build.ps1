# Make so the script will stop when it hits an error.
$ErrorActionPreference = "Stop"

# Get the executing directory and set it to the current directory.
$scriptBin = ""
Try { $scriptBin = "$(Split-Path -Parent $MyInvocation.MyCommand.Definition)" } Catch {}
If ([string]::IsNullOrEmpty($scriptBin)) { $scriptBin = $pwd }
Set-Location $scriptBin

$version = $env:APPVEYOR_BUILD_VERSION;
$localBuild = $false;
#For local builds, appveyor will be provided version
if([System.String]::IsNullOrEmpty($version)){
	$version = & git describe --tags;
	$localBuild = $true;
}
#Filter out - branch commit locally
if($version -Match "-"){
	$version = $version.Split("-")[0];
}

#Filter out +Build# from CI builds
if($version -Match "\+"){
	$version = $version.Split("+")[0];
}


if($localBuild -eq $true){
	$build = & git rev-list --count HEAD;
	$version = "$($version)$build";
}


Write-Host "---Version $version will be used---" -ForegroundColor Magenta;

$artifacts = "$scriptBin/artifacts";
$testGenerator = Resolve-Path "$scriptBin/test/AspNetCore.Client.Test.Generator";

Get-ChildItem -Path $artifacts -Filter "*.nupkg" -Recurse | Remove-item -ErrorAction Ignore;
$outputDir = Resolve-Path $artifacts;

Write-Host ">> dotnet --info" -ForegroundColor Magenta;
dotnet --info

Write-Host ">> dotnet clean -c Release -v m" -ForegroundColor Magenta;
dotnet clean -c Release -v m

Write-Host ">> dotnet build -c Release -v m;" -ForegroundColor Magenta;
dotnet build -c Release -v m;

#Run the test project generators
Push-Location -Path $testGenerator -StackName "Run";
Write-Host ">> dotnet run -c Release -v m;" -ForegroundColor Magenta;
dotnet run -c Release -v m --framework netcoreapp2.1;
Pop-Location -StackName "Run";

#Build again, making sure that our clients that were just regenerated via the previous command build
Write-Host ">> dotnet build -c Release -v m;" -ForegroundColor Magenta;
dotnet build -c Release -v m;

Write-Host ">> dotnet test -c Release -v m;" -ForegroundColor Magenta;
dotnet test -c Release -v m;



Write-Host ">> dotnet pack -c Release /p:Version=$version -o $outputDir -v m" -ForegroundColor Magenta;
dotnet pack -c Release /p:Version="$version" -o $outputDir -v m

Write-Host "Remove Client.cs files so we can regenerate them with the msbuild task to make that works" -ForegroundColor Magenta;
Get-ChildItem -Path "**/Clients.cs" -Recurse | Remove-Item -Force

#Remove the generator from the solution so we don't have two references to it's assembly which causes the generator to fail.

dotnet sln remove "test/AspNetCore.Client.Test.Generator/AspNetCore.Client.Test.Generator.csproj"
dotnet sln remove "src/AspNetCore.Client.Generator/AspNetCore.Client.Generator.csproj"

try{
	Write-Host ">> dotnet clean -c Release -v m" -ForegroundColor Magenta;
	dotnet clean -c Release -v m

	#Build again, making sure our build task works
	Write-Host ">> dotnet build -c Release -v m /p:GenerateWithNuget=true" -ForegroundColor Magenta;
	dotnet build -c Release /p:GenerateWithNuget=true;

	Write-Host ">> dotnet test -c Release -v m /p:GenerateWithNuget=true" -ForegroundColor Magenta;
	dotnet test -c Release;

}
catch{
	throw $_
}
finally{
	#Readd them, for locals
	dotnet sln add "test/AspNetCore.Client.Test.Generator/AspNetCore.Client.Test.Generator.csproj"
	dotnet sln add "src/AspNetCore.Client.Generator/AspNetCore.Client.Generator.csproj"
}





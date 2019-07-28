[CmdletBinding()]
Param(
    [Parameter(Position=0,Mandatory=$false,ValueFromRemainingArguments=$true)]
    [string[]]$BuildArguments
)

Write-Output "Windows PowerShell $($Host.Version)"

Set-StrictMode -Version 2.0;
$ErrorActionPreference = "Stop";
$ConfirmPreference = "None";
trap { Write-Host $_; exit 1 }
$PSScriptRoot = Split-Path $MyInvocation.MyCommand.Path -Parent

###########################################################################
# CONFIGURATION
###########################################################################

$BuildProjectFile = "$PSScriptRoot\build\_build.csproj"
$TempDirectory = "$PSScriptRoot\\.tmp"

$DotNetGlobalFile = "$PSScriptRoot\\global.json"
$DotNetInstallUrl = "https://raw.githubusercontent.com/dotnet/cli/master/scripts/obtain/dotnet-install.ps1"
$DotNetChannel = "Current"

$env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE = 1
$env:DOTNET_CLI_TELEMETRY_OPTOUT = 1

###########################################################################
# EXECUTION
###########################################################################

function HasDotnetVersion{
	Param(
		[string]$expectedVersion
	)

	$sdks = @();

	((dotnet --list-sdks).Split([System.Environment]::NewLine) `
		| % {$sdks += $_.Split(" [")[0].Trim()});

	$expectedMajorVersion = [int]($expectedVersion.Split(".")[0]);
	$expectedMinorVersion = [int]($expectedVersion.Split(".")[1]);
	$expectedPatchVersionFirst = [int]::Parse(($expectedVersion.Split(".")[2][0]));
	$expectedPatchVersion = [int]($expectedVersion.Split(".")[2]);

	foreach($sdk in $sdks){
		try{
			$sdkMajorVersion = [int]($sdk.Split(".")[0]);
			$sdkMinorVersion = [int]($sdk.Split(".")[1]);
			$sdkPatchVersionFirst = [int]::Parse(($sdk.Split(".")[2][0]));
			$sdkPatchVersion = [int]($sdk.Split(".")[2]);

			if($expectedMajorVersion -eq $sdkMajorVersion){
				if($expectedMinorVersion -eq $sdkMinorVersion){
					if($expectedPatchVersionFirst -eq $sdkPatchVersionFirst){
						if($expectedPatchVersion -le $sdkPatchVersion){
							Write-Host "$sdk is installed" -ForegroundColor Magenta;
							return $true;
						}
					}
				}
			}

		}catch{}
	}

	return $false;
}

function ExecSafe([scriptblock] $cmd) {
    & $cmd
    if ($LASTEXITCODE) { exit $LASTEXITCODE }
}

# If global.json exists, load expected version
if (Test-Path $DotNetGlobalFile) {
	$DotNetGlobal = $(Get-Content $DotNetGlobalFile | Out-String | ConvertFrom-Json)
	if ($DotNetGlobal.PSObject.Properties["sdk"] -and $DotNetGlobal.sdk.PSObject.Properties["version"]) {
		$DotNetVersion = $DotNetGlobal.sdk.version
	}
}

# If dotnet is installed locally, and expected version is not set or installation matches the expected version
if ((Get-Command "dotnet" -ErrorAction SilentlyContinue) -ne $null -and `
	$env:ForceSdkDownload -ne $true -and `
     (!(Test-Path variable:DotNetVersion) -or (HasDotnetVersion -expectedVersion $DotNetVersion -eq $true))) {
	$env:DOTNET_EXE = (Get-Command "dotnet").Path
}
else {
	$DotNetDirectory = "$TempDirectory\dotnet-win"
	$env:DOTNET_EXE = "$DotNetDirectory\dotnet.exe"

	# Download install script
	$DotNetInstallFile = "$TempDirectory\dotnet-install.ps1"
	md -force $TempDirectory > $null

	try{
		(New-Object System.Net.WebClient).DownloadFile($DotNetInstallUrl, $DotNetInstallFile)
	}catch{
		throw "There was an issue downloading the dotnet sdk for the tmp dir.  There may be a proxy blocking the sdk download.";
	}

	# Install by channel or version
	if (!(Test-Path variable:DotNetVersion)) {
		ExecSafe { & $DotNetInstallFile -InstallDir $DotNetDirectory -Channel $DotNetChannel -NoPath }
	} else {
		ExecSafe { & $DotNetInstallFile -InstallDir $DotNetDirectory -Version $DotNetVersion -NoPath }
	}
}

Write-Output "Microsoft (R) .NET Core SDK version $(& $env:DOTNET_EXE --version)"

ExecSafe { & $env:DOTNET_EXE build $BuildProjectFile /nodeReuse:false }
ExecSafe { & $env:DOTNET_EXE run --project $BuildProjectFile --no-build -- $BuildArguments }

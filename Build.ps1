[CmdletBinding()]
Param(
    [Parameter(Position=0,Mandatory=$false,ValueFromRemainingArguments=$true)]
    [string[]]$BuildArguments
)

Write-Output "PowerShell $($PSVersionTable.PSEdition) version $($PSVersionTable.PSVersion)"

Set-StrictMode -Version 2.0;
$ErrorActionPreference = "Stop";
$ConfirmPreference = "None";
trap {
	Write-Error $_;
	exit 1
}
$PSScriptRoot = Split-Path $MyInvocation.MyCommand.Path -Parent

###########################################################################
# CONFIGURATION
###########################################################################

$BuildProjectFile = "$PSScriptRoot\build\_build.csproj"
$TempDirectory = "$PSScriptRoot\\.tmp"

$DotNetGlobalFile = "$PSScriptRoot\\global.json"
$DotNetInstallUrlPowershell = "https://raw.githubusercontent.com/dotnet/cli/master/scripts/obtain/dotnet-install.ps1"
$DotNetInstallUrlBash = "https://raw.githubusercontent.com/dotnet/cli/master/scripts/obtain/dotnet-install.sh"
$DotNetChannel = "Current"

$env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE = 1
$env:DOTNET_CLI_TELEMETRY_OPTOUT = 1
$env:NUGET_XMLDOC_MODE = "skip"

###########################################################################
# EXECUTION
###########################################################################

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
if((Get-Variable IsWindows -ErrorAction SilentlyContinue) -eq $null){
	$DotNetSuffix = "win";
	$DotNetExtension = ".exe";
	$DotNetInstallUrl = $DotNetInstallUrlPowershell;
	$DotNetInstallExtension = "ps1";
}
else{
	$DotNetSuffix = if($IsWindows -eq $false -or $IsWindows -eq $null){"unix"} else{"win"};
	$DotNetExtension = if($IsWindows -eq $false -or $IsWindows -eq $null){""} else{".exe"};
	$DotNetInstallUrl =  if($IsWindows -eq $false -or $IsWindows -eq $null){$DotNetInstallUrlBash} else{$DotNetInstallUrlPowershell};
	$DotNetInstallExtension = if($IsWindows -eq $false -or $IsWindows -eq $null){"sh"} else{"ps1"};
}

$DotNetDirectory = "$TempDirectory\dotnet-$DotNetSuffix"
$DotNetVersionDirectory = "$DotNetDirectory\sdk\$DotNetVersion"
$env:DOTNET_EXE = "$DotNetDirectory\dotnet$DotNetExtension"

if ($null -ne (Get-Command "dotnet" -ErrorAction SilentlyContinue) -and `
     (!(Test-Path variable:DotNetVersion) -or $(& dotnet --version | Select-Object -First 1) -eq $DotNetVersion)) {
    $env:DOTNET_EXE = (Get-Command "dotnet").Path
}
else{
	if(!(Test-Path $DotNetVersionDirectory)){
		# Download install script
		$DotNetInstallFile = "$TempDirectory\dotnet-install.$DotNetInstallExtension"
		New-Item -ItemType Directory -Force -Path $TempDirectory | Out-Null
		(New-Object System.Net.WebClient).DownloadFile($DotNetInstallUrl, $DotNetInstallFile)

		# Install by channel or version
		if (!(Test-Path variable:DotNetVersion)) {
			if($DotNetInstallExtension -eq "ps1"){
				ExecSafe { & $DotNetInstallFile -InstallDir $DotNetDirectory -Channel $DotNetChannel -NoPath }
			}
			elseif ($DotNetInstallExtension -eq "sh"){
				ExecSafe { & "$DotNetInstallFile" --install-dir "$DotNetDirectory" --channel "$DotNetChannel" --no-path }
			}
			else{
				throw "Unknown install extension";
			}
		} else {
			if($DotNetInstallExtension -eq "ps1"){
				ExecSafe { & $DotNetInstallFile -InstallDir $DotNetDirectory -Version $DotNetVersion -NoPath }
			}
			elseif ($DotNetInstallExtension -eq "sh"){
				ExecSafe { &  "$DotNetInstallFile" --install-dir "$DotNetDirectory" --version "$DotNetVersion" --no-path }
			}
			else{
				throw "Unknown install extension";
			}
		}
	}
}

$env:PATH += $DotNetDirectory;
$env:DOTNET_ROOT = $DotNetDirectory;

Write-Output "Microsoft (R) .NET Core SDK version $(& $env:DOTNET_EXE --version)"

try{
	ExecSafe { & $env:DOTNET_EXE build $BuildProjectFile -c Release /nodeReuse:false }
	ExecSafe { & $env:DOTNET_EXE run --project $BuildProjectFile -c Release --no-build -- $BuildArguments }
}finally{
	ExecSafe { & $env:DOTNET_EXE build-server shutdown --msbuild --vbcscompiler}
}

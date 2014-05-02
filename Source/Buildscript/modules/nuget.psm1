Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

$NugetVersion = '2.8.1'

$ThisDir = Split-Path $MyInvocation.MyCommand.Path
$NugetPath = Join-Path $ThisDir "..\NuGet-$NugetVersion\NuGet.exe"

function Invoke-NuGet ($Arguments) {

	if (!(Test-Path $NugetPath)){
		Download-Nuget
	}

	$allArguments = $Arguments + @(
		"-NonInteractive"
		"-Verbosity"
		"quiet"
	)

	& $NugetPath $allArguments
	if ($LastExitCode -ne 0) {
		throw "Command failed with exit code $LastExitCode"
	}
}

function Download-Nuget{

	$uriPath = "https://packages.nuget.org/api/v2/package/NuGet.CommandLine/$NugetVersion"
	$response = Invoke-WebRequest $uriPath -UseBasicParsing -UseDefaultCredentials -Proxy 'uk-tmg05.2gis.local' -ProxyUseDefaultCredentials

	Add-Type -Assembly System.IO.Compression
	$zipArchive = New-Object System.IO.Compression.ZipArchive($response.RawContentStream, 'Read')
	$zipArchiveEntry = $zipArchive.GetEntry('tools/NuGet.exe')
	
	$nugetDir = Split-Path $NugetPath
	if (!(Test-Path $nugetDir)){
		md $nugetDir | Out-Null
	}
	
	$fileStream = New-Object System.IO.FileStream($NugetPath, 'Create')
	$zipStream = $zipArchiveEntry.Open()
	$zipStream.CopyTo($fileStream)
	
	$zipStream.Dispose()
	$fileStream.Dispose()
	$zipArchive.Dispose()
}

Export-ModuleMember -Function Invoke-NuGet
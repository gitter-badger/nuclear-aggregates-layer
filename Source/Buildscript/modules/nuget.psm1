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

	$uriString = "https://packages.nuget.org/api/v2/package/NuGet.CommandLine/$NugetVersion"
	
	$request = [System.Net.WebRequest]::Create($uriString)
	$request.UseDefaultCredentials = $true
	$proxy = $request.Proxy
	if ($proxy -ne $null){
		$proxy.Credentials = [System.Net.CredentialCache]::DefaultCredentials
	}
	$response = $request.GetResponse()
	$responseStream = $response.GetResponseStream()

	Add-Type -Assembly System.IO.Compression
	$zipArchive = New-Object System.IO.Compression.ZipArchive($responseStream, 'Read')
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

function Get-PackageDir ($PackageId){

	[xml]$xml = Get-Content 'packages.config' -Encoding UTF8
	$packageNode = $xml.SelectNodes('//package') | Where-Object { $_.id -eq $PackageId}
	if ($packageNode -eq $null){
		throw "Can't find solution package $PackageId"
	}

	$solutionDir = Join-Path $ThisDir '..\..'	
	$packageDir = Join-Path $solutionDir "packages\$PackageId.$($packageNode.version)" 
	if (!(Test-Path $packageDir)){
		throw "Can't find package dir $packageDir"
	}
	
	return $packageDir
}

Export-ModuleMember -Function Invoke-NuGet, Get-PackageDir
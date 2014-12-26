Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

$ThisDir = Split-Path $MyInvocation.MyCommand.Path
$SolutionDir = Join-Path $ThisDir '..\..'

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

function Download-Nuget {

	$uriString = "https://packages.nuget.org/api/v2/package/$($PackageInfo.Id)/$($PackageInfo.Version)"
	
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
	
	$entry = $zipArchive.GetEntry('tools/NuGet.exe')
	$entryFullName = Join-Path $PackageInfo.VersionedDir $entry.FullName
	$entryDir = Split-Path $entryFullName
	if (!(Test-Path $entryDir)){
		md $entryDir | Out-Null	
	}
	
	$fileStream = New-Object System.IO.FileStream($entryFullName, 'Create')
	$zipStream = $entry.Open()
	$zipStream.CopyTo($fileStream)
	
	$zipStream.Dispose()
	$fileStream.Dispose()

	$zipArchive.Dispose()
}

function Get-PackageInfo ($PackageId, $ThrowErrors = $true){

	$packagesConfigFileName = Join-Path $SolutionDir 'Buildscript\packages.config'

	[xml]$xml = Get-Content $packagesConfigFileName -Encoding UTF8
	$packageNode = $xml.SelectNodes('//package') | Where-Object { $_.id -eq $PackageId}
	if ($packageNode -eq $null){
		throw "Can't find solution package $PackageId"
	}

	$versionedDir = Join-Path $solutionDir "packages\$PackageId.$($packageNode.version)" 
	
	if (!(Test-Path $versionedDir)){
		if ($ThrowErrors){
			throw "Can't find package dir $versionedDir"
		}
	}
	
	return @{
		'Id' = $PackageId
		'Version' = $packageNode.version
		'VersionedDir' = $versionedDir
	}
}

$PackageInfo = Get-PackageInfo 'NuGet.CommandLine' $false
$NugetPath = Join-Path $PackageInfo.VersionedDir 'tools\NuGet.exe'

Export-ModuleMember -Function Invoke-NuGet, Get-PackageInfo
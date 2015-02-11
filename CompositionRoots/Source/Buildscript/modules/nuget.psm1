Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

function Invoke-NuGet ($Arguments) {

	if (!(Test-Path $NugetPath)){
		Download-Nuget
	}

	$allArguments = $Arguments + @(
		'-NonInteractive'
		'-Verbosity'
		'quiet'
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

function Get-PackageInfo ($PackageId, $ThrowError = $true){

	[xml]$xml = Get-Content $LocalPackagesConfig -Encoding UTF8
	$packageNode = $xml.SelectNodes('//package') | Where-Object { $_.id -eq $PackageId}
	if ($packageNode -eq $null){
		throw "Can't find solution package $PackageId"
	}

	$versionedDir = Join-Path $global:Context.Dir.Solution "packages\$PackageId.$($packageNode.version)" 
	
	if (!(Test-Path $versionedDir)){
		if ($ThrowError){
			throw "Can't find package dir $versionedDir"
		}
	}
	
	return @{
		'Id' = $PackageId
		'Version' = $packageNode.version
		'VersionedDir' = $versionedDir
	}
}

function Restore-Packages {

	if (Test-Path 'Env:\TEAMCITY_VERSION') {
		Write-Host "##teamcity[progressMessage 'Restore-Packages']"
	}

	$solution = Get-ChildItem $global:Context.Dir.Solution -Filter '*.sln'

	Invoke-NuGet @(
		'restore'
		$solution.FullName
	)

	Invoke-NuGet @(
		'restore'
		$LocalPackagesConfig
		'-SolutionDirectory'
		$global:Context.Dir.Solution
	)
}

$LocalPackagesConfig = "$PSScriptRoot\packages.config"
$PackageInfo = Get-PackageInfo 'NuGet.CommandLine' -ThrowError $false
$NugetPath = Join-Path $PackageInfo.VersionedDir 'tools\NuGet.exe'

Export-ModuleMember -Function Invoke-NuGet, Get-PackageInfo, Restore-Packages
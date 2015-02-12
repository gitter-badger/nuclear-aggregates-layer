Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module "$PSScriptRoot\msbuild.psm1" -DisableNameChecking
Import-Module "$PSScriptRoot\versioning.psm1" -DisableNameChecking

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

# костыль чтобы указать repositoryPath
function Copy-NugetConfig ($SolutionRelatedAllProjectsDir) {
	$content = @"
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <config>
    <add key="repositoryPath" value="$($global:Context.Dir.Solution)\packages" />
  </config>
</configuration>
"@
	$allProjectsDir = Join-Path $global:Context.Dir.Solution $SolutionRelatedAllProjectsDir
	$fileName = Join-Path $allProjectsDir 'NuGet.Config'
	
	if (!(Test-Path $fileName)){
		Set-Content $fileName $content -Encoding UTF8 -Force
	}
}

# нужно создать nuspec файлы для вообще всех проектов, чтобы правильно работал флаг IncludeReferencedProjects
# создаём типовые nuspec-файлы для всех проектов в solution
function Create-NuspecFiles ($SolutionRelatedAllProjectsDir = '.') {

	Copy-NugetConfig $SolutionRelatedAllProjectsDir

	$projects = Find-Projects $SolutionRelatedAllProjectsDir

	$content = @'
<?xml version="1.0" encoding="utf-8"?>
<package>
  <metadata>
    <id>$id$</id>
    <version>$version$</version>
    <authors>$author$</authors>
    <description>$description$</description>
	
	<tags>ERM</tags>
  </metadata>
  <files>
  	<file src="bin\$configuration$\**\$id$.resources.dll" />
  </files>
</package>
'@

	foreach($project in $projects){
	
		$nuspecFileName = [System.IO.Path]::ChangeExtension($project.FullName, '.nuspec')
		if ((Test-Path $nuspecFileName)){
			continue
		}
		
		Set-Content $nuspecFileName $content -Encoding UTF8 -Force
	}
}

function Build-PackagesFromProjects ($Projects, $OutputDirectory){

	foreach($project in $Projects){
		
		$buildFileName = Create-BuildFile $project.FullName
		Invoke-MSBuild $buildFileName
		
		Invoke-NuGet @(
			'pack'
			$buildFileName
			'-Properties'
			# TODO отрефакторить
			'Configuration=Release;VisualStudioVersion=12.0'
			'-IncludeReferencedProjects'
			'-ExcludeEmptyDirectories'
			'-NoPackageAnalysis'
			'-OutputDirectory'
			$OutputDirectory
			# create '.symbols.nupkg'
			'-Symbols'
		)
	}
}

function Build-PackagesFromNuSpecs ($NuSpecs, $OutputDirectory) {

	foreach($NuSpec in $NuSpecs){
		
		Invoke-NuGet @(
			'pack'
			$NuSpec.FullName
			'-Version'
			(Get-Version).SemanticVersion
			'-OutputDirectory'
			$OutputDirectory
		)
	}
}

function Deploy-Packages ($Packages, $ServerUrl, $ApiKey){
	
	foreach($package in $Packages){
		Invoke-NuGet @(
			'push'
			"$($package.FullName)"
			'-Source'
			$ServerUrl
			'-ApiKey'
			$ApiKey
		)
	}
}

$LocalPackagesConfig = "$PSScriptRoot\packages.config"
$PackageInfo = Get-PackageInfo 'NuGet.CommandLine' -ThrowError $false
$NugetPath = Join-Path $PackageInfo.VersionedDir 'tools\NuGet.exe'

Export-ModuleMember -Function Invoke-NuGet, Get-PackageInfo, Restore-Packages, Deploy-Packages, Create-NuspecFiles, Build-PackagesFromProjects, Build-PackagesFromNuSpecs
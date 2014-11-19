Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

$Servers = @{
	'PublishPackages' = @{
		'Url' = 'http://nuget.2gis.local'
		'ApiKey' = ':enrbq rjl'
	}
	'PublishSymbols' = @{
		'Url' = 'http://nuget.2gis.local/SymbolServer/NuGet'
		'ApiKey' = ':enrbq rjl'
	}
}

Import-Module .\modules\nuget.psm1 -DisableNameChecking
Import-Module .\modules\versioning.psm1 -DisableNameChecking
Import-Module .\modules\msbuild.psm1 -DisableNameChecking

Task Build-AutoTestsPackages -Depends Create-GlobalContext, Set-BuildNumber {

	$projectsDir = Join-Path $global:Context.Dir.Solution '..\..\Platform\Source'
	$include = @(
		'2Gis.Erm.Platform.Model.csproj'
		'2Gis.Erm.Platform.Common.csproj'
		'2Gis.Erm.Platform.Resources.Server.csproj'
	)
	
	Build-Packages $projectsDir $include
}

Task Build-PlatformPackages -Depends Create-GlobalContext, Set-BuildNumber {

	$projectsDir = Join-Path $global:Context.Dir.Solution '..\..\Platform\Source'
	$include = '*.csproj'
	
	Build-Packages $projectsDir $include
}

Task Deploy-NuGet {
	$artifactName = Get-Artifacts 'NuGet'
	$packages = Get-ChildItem $artifactName -Filter '*.nupkg' -Recurse
	foreach($package in $packages){
		
		if ($package.Name.EndsWith('.symbols.nupkg')){
			$source = $Servers.PublishSymbols.Url
			$apiKey = $Servers.PublishSymbols.ApiKey
		} else{
			$source = $Servers.PublishPackages.Url
			$apiKey = $Servers.PublishPackages.ApiKey
		}
		
		Invoke-NuGet @(
			'push'
			"$($package.FullName)"
			'-Source'
			$source
			'-ApiKey'
			$apiKey
		)
	}
}

function Build-Packages ($ProjectsDir, $Include){

	$tempDir = Join-Path $global:Context.Dir.Temp 'NuGet'
	md $tempDir | Out-Null

	Copy-NugetConfig

	$assemblyInfos = Get-ChildItem $projectsDir -Filter 'AssemblyInfo.Version.cs' -Recurse
	Update-AssemblyInfo $assemblyInfos

	$projects = Get-ChildItem $projectsDir -Include $include -Recurse
	foreach($project in $projects){
		$projectFileName = $project.FullName
		$projectDir = Split-Path $projectFileName -Parent
		
		$nuSpecFile = Join-Path $projectDir ([System.IO.Path]::GetFileNameWithoutExtension($projectFileName) + '.nuspec')
		if (! (Test-Path $nuSpecFile)){
			continue
		}
		
		Invoke-MSBuild @(
			"""$projectFileName"""
		)
		
		Invoke-NuGet @(
			'pack'
			$projectFileName
			'-Properties'
			'Configuration=Release'
			'-IncludeReferencedProjects'
			'-ExcludeEmptyDirectories'
			'-NoPackageAnalysis'
			'-OutputDirectory'
			$tempDir
			# create '.symbols.nupkg'
			#'-Symbols'
		)
	}
	
	Publish-Artifacts $tempDir 'NuGet'
}

# костыль чтобы указать repositoryPath
function Copy-NugetConfig {
	$content = @"
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <config>
    <add key="repositoryPath" value="CompositionRoots\Source\packages" />
  </config>
</configuration>
"@
	$fileName = Join-Path $global:Context.Dir.Solution '..\..\NuGet.Config'
	
	Set-Content $fileName $content -Encoding UTF8 -Force
}
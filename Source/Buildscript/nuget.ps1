Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

#$PublishUrl = 'http://uk-tfs01.2gis.local:8086/nuget'
#$PublishUrl = 'http://nuget.2gis.local/nuget'
$PublishUrl = 'D:\NuGet'
$ApiKey = ':enrbq rjl'

Import-Module .\modules\nuget.psm1 -DisableNameChecking
Import-Module .\modules\versioning.psm1 -DisableNameChecking
Import-Module .\modules\msbuild.psm1 -DisableNameChecking

Task Build-AutoTestsPackages {

	$projectsDir = Join-Path $global:Context.Dir.Solution '..\..\Platform\Source'
	$filter = '2Gis.Erm.Platform.Model.csproj'
	
	Build-Packages $projectsDir $filter
}

Task Build-PlatformPackages {

	$projectsDir = Join-Path $global:Context.Dir.Solution '..\..\Platform\Source'
	$filter = '*.csproj'
	
	Build-Packages $projectsDir $filter
}

Task Deploy-NuGet {
	$artifactName = Get-Artifacts 'NuGet'
	$packages = Get-ChildItem $artifactName -Filter '*.nupkg' -Recurse
	foreach($package in $packages){
		
		Invoke-NuGet @(
			'push'
			"$($package.FullName)"
			'-Source'
			$PublishUrl
			'-ApiKey'
			$ApiKey
		)
	}
}

function Build-Packages ($ProjectsDir, $Filter){

	$tempDir = Join-Path $global:Context.Dir.Temp 'NuGet'
	md $tempDir | Out-Null

	$assemblyInfos = Get-ChildItem $projectsDir -Filter 'AssemblyInfo.Version.cs' -Recurse
	Update-AssemblyInfo $assemblyInfos

	$projects = Get-ChildItem $projectsDir -Filter $Filter -Recurse
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
			"$projectFileName"
			'-Properties'
			'Configuration=Release'
			'-IncludeReferencedProjects'
			'-ExcludeEmptyDirectories'
			'-OutputDirectory'
			"$tempDir"
			'-Symbols'
		)
	}
	
	Publish-Artifacts $tempDir 'NuGet'
}
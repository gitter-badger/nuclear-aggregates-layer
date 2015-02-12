﻿Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module "$PSScriptRoot\modules\nuget.psm1" -DisableNameChecking

Task Build-AutoTestsPackages -Depends Set-BuildNumber, Update-AssemblyInfo {

	# все проекты лежат на две папки выше solution
	$SolutionRelatedAllProjectsDir = '..\..'

	$projectDirs = @(
		Join-Path $SolutionRelatedAllProjectsDir 'Platform'
		Join-Path $SolutionRelatedAllProjectsDir 'BLCore'
		Join-Path $SolutionRelatedAllProjectsDir 'BLQuerying'
	)
	
	$include = @(
		'2Gis.Erm.Platform.Model.csproj'
		'2Gis.Erm.Platform.Common.csproj'
		'2Gis.Erm.Platform.API.ServiceBusBroker.csproj'
		'2Gis.Erm.Platform.API.Core.csproj'
		'2Gis.Erm.BLCore.API.Releasing.csproj'
		'2Gis.Erm.BLCore.API.Operations.Special.csproj'
	
		'2Gis.Erm.Qds.API.Operations.csproj'
	)

	$tempDir = Join-Path $global:Context.Dir.Temp 'NuGet'
	if (!(Test-Path $tempDir)){
		md $tempDir | Out-Null
	}

	Create-NuspecFiles $SolutionRelatedAllProjectsDir

	$projects = Find-Projects $projectDirs $include
	Build-PackagesFromProjects $projects $tempDir
	
	Publish-Artifacts $tempDir 'NuGet'
}

Task Deploy-NuGet {
	$artifactName = Get-Artifacts 'NuGet'

	$packges = Get-ChildItem $artifactName -Include '*.nupkg' -Exclude '*.symbols.nupkg' -Recurse
	Deploy-Packages $packges 'http://nuget.2gis.local' ':enrbq rjl'

	$symbolPackges = Get-ChildItem $artifactName -Include '*.symbols.nupkg' -Recurse
	Deploy-Packages $symbolPackges 'http://nuget.2gis.local/SymbolServer/NuGet' ':enrbq rjl'
}
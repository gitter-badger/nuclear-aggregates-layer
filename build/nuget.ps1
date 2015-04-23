Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module "$BuildToolsRoot\modules\metadata.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\nuget.psm1" -DisableNameChecking

Task Build-AutoTestsPackages -Depends Set-BuildNumber, Update-AssemblyInfo {

	# все проекты лежат на две папки выше solution
	$SolutionRelatedAllProjectsDir = '..\..'

	$include = @(
		'2Gis.Erm.Platform.Model.csproj'
		'2Gis.Erm.Platform.Common.csproj'
		'2Gis.Erm.Platform.API.ServiceBusBroker.csproj'
		'2Gis.Erm.Platform.API.Core.csproj'
		'2Gis.Erm.BLCore.API.Releasing.csproj'
		'2Gis.Erm.BLCore.API.Operations.Special.csproj'

		'2Gis.Erm.Qds.API.Operations.csproj'
	)

	$commonMetadata = Get-Metadata 'Common'

	$tempDir = Join-Path $commonMetadata.Dir.Temp 'NuGet'
	if (!(Test-Path $tempDir)){
		md $tempDir | Out-Null
	}

	Create-NuspecFiles $SolutionRelatedAllProjectsDir

	$projects = Find-Projects $SolutionRelatedAllProjectsDir $include
	Build-PackagesFromProjects $projects $tempDir

	Publish-Artifacts $tempDir 'NuGet'
}

Task Deploy-NuGet {
	$artifactName = Get-Artifacts 'NuGet'
	Deploy-Packages $artifactName
}

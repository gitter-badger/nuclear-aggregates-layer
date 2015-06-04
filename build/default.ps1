Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module "$BuildToolsRoot\modules\msbuild.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\nuget.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\metadata.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\unittests.psm1" -DisableNameChecking

Task Default -depends Hello
Task Hello { "Билдскрипт запущен без цели, укажите цель" }

Task Build-NuGetPackages {

    $SolutionRelatedAllProjectsDir = '.'

	$commonMetadata = Get-Metadata 'Common'

	$tempDir = Join-Path $commonMetadata.Dir.Temp 'NuGet'
    if (!(Test-Path $tempDir)){
        md $tempDir | Out-Null
    }

    $projects = Find-Projects $SolutionRelatedAllProjectsDir -Filter '*.nuproj'
	Build-PackagesFromNuProjs $projects $tempDir

    Publish-Artifacts $tempDir 'NuGet'
}

Task Deploy-NuGet {
	$artifactName = Get-Artifacts 'NuGet'
	Deploy-Packages $artifactName
}

Task Run-UnitTests {
	$SolutionRelatedAllProjectsDir = '.'
	
	$projects = Find-Projects $SolutionRelatedAllProjectsDir '*Tests*'
	foreach($project in $Projects){
		$buildFileName = Create-BuildFile $project.FullName
		Invoke-MSBuild $buildFileName
	}

	Run-UnitTests $projects
}
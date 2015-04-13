Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module "$BuildToolsRoot\modules\nuget.psm1" -DisableNameChecking

Task Build-AutoTestsPackages -Depends Set-BuildNumber, Update-AssemblyInfo {

	# все проекты лежат на две папки выше solution
    $SolutionRelatedAllProjectsDir = '..\..'

    $tempDir = Join-Path $global:Context.Dir.Temp 'NuGet'
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
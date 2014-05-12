Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module .\modules\msbuild.psm1 -DisableNameChecking

$IncludeProjects = @('*Tests.Unit*')

Task Build-CoreTeamUnitTests {

	$projectDirs = @(
		'..\..\Platform'
		'..\..\BLCore'
	)

	$projects = Find-Projects $projectDirs $IncludeProjects
	Build-UnitTestProjects $projects
}

Task Build-BLTeamUnitTests {

	$projectDirs = @(
		'..\..\BL'
		'..\..\BLFlex'
	)

	$projects = Find-Projects $projectDirs $IncludeProjects
	Build-UnitTestProjects $projects
}

Task Build-ResearchTeamUnitTests {

	$projectDirs = @('..\..\BLQuerying')
	
	$projects = Find-Projects $projectDirs $IncludeProjects
	Build-UnitTestProjects $projects
}

Task Build-CompositionTeamUnitTests {

	$projectDirs = @('.')
	
	$projects = Find-Projects $projectDirs $IncludeProjects
	Build-UnitTestProjects $projects
}

function Build-UnitTestProjects ($Projects){

	foreach($project in $Projects){
		Invoke-MSBuild @(
			"""$($project.FullName)"""
		)
		
		$projectDir = Split-Path $project.FullName
		$convensionalArtifactName = Join-Path $projectDir 'bin\Release'
		Publish-Artifacts $convensionalArtifactName 'Unit Tests'
	}
}
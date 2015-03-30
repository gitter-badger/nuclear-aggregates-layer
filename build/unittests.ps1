Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module "$BuildToolsRoot\modules\unittests.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\msbuild.psm1" -DisableNameChecking

$IncludeProjects = @('*Tests.Unit*')

Task Run-CoreTeamUnitTests {

	$projectDirs = @(
		'..\..\Platform'
		'..\..\BLCore'
	)

	$projects = Find-Projects $projectDirs $IncludeProjects
	Build-UnitTestProjects $projects
	Run-UnitTests $projects
}

Task Run-BLTeamUnitTests {

	$projectDirs = @(
		'..\..\BL'
		'..\..\BLFlex'
	)

	$projects = Find-Projects $projectDirs $IncludeProjects
	Build-UnitTestProjects $projects
	Run-UnitTests $projects
}

Task Run-ResearchTeamUnitTests {

	$projectDirs = @('..\..\BLQuerying')
	
	$projects = Find-Projects $projectDirs $IncludeProjects
	Build-UnitTestProjects $projects
	Run-UnitTests $projects
}

Task Run-CompositionTeamUnitTests {

	$projectDirs = @('.')
	
	$projects = Find-Projects $projectDirs $IncludeProjects
	Build-UnitTestProjects $projects
	Run-UnitTests $projects
}

function Build-UnitTestProjects ($Projects){

	foreach($project in $Projects){
		$buildFileName = Create-BuildFile $project.FullName
		Invoke-MSBuild $buildFileName
	}
}
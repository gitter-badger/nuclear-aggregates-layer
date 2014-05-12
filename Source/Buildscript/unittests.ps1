Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module .\modules\nuget.psm1 -DisableNameChecking
Import-Module .\modules\msbuild.psm1 -DisableNameChecking

$MSpecDir = Get-PackageDir 'Machine.Specifications-Signed'
$MSpecPath = Join-Path $MSpecDir "tools\mspec-clr4.exe"

$IncludeProjects = @('*Tests.Unit*')

Task Run-CoreTeamUnitTests {

	$projectDirs = @(
		'..\..\Platform'
		'..\..\BLCore'
	)

	$projects = Find-Projects $projectDirs $IncludeProjects
	Build-UnitTestProjects $projects
	Execute-UnitTests $projects
}

Task Run-BLTeamUnitTests {

	$projectDirs = @(
		'..\..\BL'
		'..\..\BLFlex'
	)

	$projects = Find-Projects $projectDirs $IncludeProjects
	Build-UnitTestProjects $projects
	Execute-UnitTests $projects
}

Task Run-ResearchTeamUnitTests {

	$projectDirs = @('..\..\BLQuerying')
	
	$projects = Find-Projects $projectDirs $IncludeProjects
	Build-UnitTestProjects $projects
	Execute-UnitTests $projects
}

Task Run-CompositionTeamUnitTests {

	$projectDirs = @('.')
	
	$projects = Find-Projects $projectDirs $IncludeProjects
	Build-UnitTestProjects $projects
	Execute-UnitTests $projects
}

function Build-UnitTestProjects ($Projects){

	foreach($project in $Projects){
		Invoke-MSBuild @(
			"""$($project.FullName)"""
		)
	}
}

function Execute-UnitTests ($Projects){

	$assemblies = $Projects | Select-Object  @{Name = 'Expand' ; Expression = {
		$conventionalAssemblyName = [System.IO.Path]::ChangeExtension($_.Name, '.dll')
		$convensionalAssemblyDir = Join-Path (Split-Path $_.FullName) 'bin\Release'
		$convensionalAssemblyFileName = Join-Path $convensionalAssemblyDir $conventionalAssemblyName
		if (!(Test-Path $convensionalAssemblyFileName)){
			throw "Can't find project dir $convensionalAssemblyFileName"
		}
		return $convensionalAssemblyFileName
	}} | Select-Object -ExpandProperty 'Expand'

	Invoke-MSpec $assemblies
}

function Invoke-MSpec ($Arguments) {

	$allArguments = @()
	if (Test-Path 'Env:\TEAMCITY_VERSION') {
		$allArguments += '--teamcity'
	}

	$allArguments += @(
		'--silent'
	) + $Arguments
	
	& $MSpecPath $allArguments
	
	# MSpec exit codes: 0 - Success; 1 - Some tests failed; -1 - Error in MSpec
	if ($lastExitCode -eq -1) {
		throw "Command failed with exit code $lastExitCode"
	}
}
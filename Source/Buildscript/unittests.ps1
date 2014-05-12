Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module .\modules\msbuild.psm1 -DisableNameChecking

Task Build-ERM-BL-UnitTests {

	$projectFileName = Get-ProjectFileName '..\..\BL\Source\Tests' '2Gis.Erm.BL.Tests.Unit'
	Build-UnitTestProject $projectFileName

	$projectFileName = Get-ProjectFileName '..\..\BLFlex\Source\Tests' '2Gis.Erm.BLFlex.Tests.Unit'
	Build-UnitTestProject $projectFileName
}

Task Build-ERM-Research-UnitTests {

	$projectFileName = Get-ProjectFileName '..\..\CompositionRoots\Source\Tests' '2Gis.Erm.Qds.IndexService.Tests.Unit'
	Build-UnitTestProject $projectFileName

	$projectFileName = Get-ProjectFileName '..\..\BLQuerying\Source\Tests' '2Gis.Erm.Qds.Etl.Tests.Unit'
	Build-UnitTestProject $projectFileName

	$projectFileName = Get-ProjectFileName '..\..\BLQuerying\Source\Tests' '2Gis.Erm.Elastic.Nest.Qds.Tests.Unit'
	Build-UnitTestProject $projectFileName
}

function Build-UnitTestProject($ProjectFileName){

	Invoke-MSBuild @(
	"""$ProjectFileName"""
	)
	
	$convensionalArtifactName = Join-Path (Split-Path $projectFileName) 'bin\Release'
	Publish-Artifacts $convensionalArtifactName 'Unit Tests'
}
Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module .\modules\msbuild.psm1 -DisableNameChecking

Task Build-UnitTests {

	$platformProjectFileName = Get-ProjectFileName '..\..\Platform\Source\Tests' '2Gis.Erm.Platform.Tests.Unit'
	Build-UnitTestProject $platformProjectFileName

	$platformProjectFileName = Get-ProjectFileName '..\..\BL\Source\Tests' '2Gis.Erm.BL.Tests.Unit'
	Build-UnitTestProject $platformProjectFileName

	$platformProjectFileName = Get-ProjectFileName '..\..\BLFlex\Source\Tests' '2Gis.Erm.BLFlex.Tests.Unit'
	Build-UnitTestProject $platformProjectFileName
}

function Build-UnitTestProject($ProjectFileName){

	# билдим unit tests в debug, чтобы применился InternalsVisibleTo, в release этот атрибут не проставляется
	Invoke-MSBuild @(
	"""$ProjectFileName"""
	) `
	-Configuration 'Debug'
	
	$convensionalArtifactName = Join-Path (Split-Path $projectFileName) 'bin\Debug'
	Publish-Artifacts $convensionalArtifactName 'Unit Tests'
}
Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module "$BuildToolsRoot\modules\ssdt.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\sqlserver.psm1" -DisableNameChecking

$sourceConnectionString = 'Data Source=uk-sql01;Initial Catalog=Erm33;Integrated Security=True'
$targetConnectionString = 'Data Source=uk-sql01;Initial Catalog=Erm06;Integrated Security=True'
$sharedFolder = '\\uk-test-olap03\ERM'

Task SchemaCompare {

	$tempDir = Join-Path $global:Context.Dir.Temp 'Erm.SchemaChange'
	if (!(Test-Path $tempDir)){
		md $tempDir | Out-Null
	}

	$reportFilePath = Join-Path $tempDir 'ERM.SchemaCompare.xml'

	Generate-SchemaCompareXmlReport $sourceConnectionString $targetConnectionString $reportFilePath

	[xml]$report = Get-Content $reportFilePath
	if ($report.DocumentElement.ChildNodes.Count -eq 0){
		return
	}

	$dumpFilePath = Join-Path $tempDir 'ERM.SchemaDump.sql'
	Dump-TableSchemas $sourceConnectionString $dumpFilePath
	
	Publish-Artifacts $tempDir 'Erm.SchemaChange'
	
	# copy artifacts OLAP shared folder
	$artifactSourcePath = Get-Artifacts 'Erm.SchemaChange'
	$artifactDestPath = Join-Path $sharedFolder (Split-Path $artifactName -Leaf)
	Copy-Item $artifactSourcePath $artifactDestPath -Force -Recurse
}
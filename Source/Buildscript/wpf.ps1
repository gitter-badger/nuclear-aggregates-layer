Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module .\modules\msbuild.psm1 -DisableNameChecking
Import-Module .\modules\msdeploy.psm1 -DisableNameChecking

Properties{ $OptionWpfClient=$false }
Task Build-WpfClient -Precondition { return $OptionWpfClient } -Depends Update-AssemblyInfo, Build-WpfClientModule {
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.UI.Desktop.WPF'
	$publishDir = Join-Path $global:Context.Dir.Temp 'WpfClient'

	# index.htm не генерится, надо самому

	$applicationVersion = $global:Context.Version.ToString(4)

	Invoke-MSBuild @(
	"""$projectFileName"""
	"/t:Publish"
	"/p:ApplicationVersion=$applicationVersion"
	"/p:PublishDir=""$publishDir\\"""
	)
	
	$packageFileName = Get-PackageFileName 'WpfClient' 'zip'
	
	Sync-MSDeploy `
	-Source "contentPath=""$publishDir""" `
	-Dest "package=""$packageFileName"""

	Write-Output "##teamcity[publishArtifacts '$packageFileName => Web']"
}

Task Build-WpfClientModule {
	$projectFileName = Get-ProjectFileName '..\..\BL\Source\EntryPoints\UI\Desktop\' '2Gis.Erm.BL.UI.WPF.Client'
	
	Invoke-MSBuild @(
	"""$projectFileName"""
	)
}

Task Deploy-WpfClient -Precondition { return $OptionWpfClient } -Depends Build-WpfClient {

}
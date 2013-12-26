Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module .\modules\transform.psm1 -DisableNameChecking
Import-Module .\modules\msbuild.psm1 -DisableNameChecking
Import-Module .\modules\msdeploy.psm1 -DisableNameChecking

Properties{ $OptionWpfClient=$false }
Task Build-WpfClient -Precondition { return $OptionWpfClient } -Depends Update-AssemblyInfo {

	Build-WpfClientModule
	Build-WpfShell
	Build-WpfPackage
}

function Build-WpfPackage {
	$publishDir = Join-Path $global:Context.Dir.Temp 'Wpf'
	$packageFileName = Get-PackageFileName '2Gis.Erm.UI.Desktop.WPF' 'zip'
	
	# пока замапим на имя сайта "WPF Client", как создадут dns имена так перенастроим на них
	$webSite = "WPF Client $($global:Context.EnvironmentName)"
	Create-ContentPackage $publishDir $webSite $packageFileName
	
	Write-Output "##teamcity[publishArtifacts '$packageFileName => Web']"
}

function Build-WpfShell {
	$publishDir = Join-Path $global:Context.Dir.Temp 'Wpf'
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.UI.Desktop.WPF'
	
	Transform-Log4NetConfig $projectFileName
	
	$applicationVersion = $global:Context.Version.ToString(4)
	$productName = "2GIS ERM WPF Client $($global:Context.EnvironmentName) $applicationVersion"
	
	# пока замапим на 81 порт, как создадут dns имена так перенастроим на них
	$hostName = $global:Context.TargetHostName
	$updateUrl = New-Object System.UriBuilder('http', $hostName, 81)

	Invoke-MSBuild @(
	"""$projectFileName"""
	"/t:Publish"
	"/p:ApplicationVersion=$applicationVersion"
	"/p:UpdateUrl=$updateUrl"
	"/p:ProductName=$productName"
	"/p:PublishDir=""$publishDir\\"""
	)
	
	# TODO: auto generate publish.htm file
}

function Build-WpfClientModule {
	$projectFileName = Get-ProjectFileName '..\..\BL\Source\EntryPoints\UI\Desktop\' '2Gis.Erm.BL.UI.WPF.Client'
	
	Transform-AppConfig $projectFileName
	
	Invoke-MSBuild @(
	"""$projectFileName"""
	)
}

Task Deploy-WpfClient -Precondition { return $OptionWpfClient } -Depends Build-WpfClient {
	$hostName = $global:Context.TargetHostName
	$packageFileName = Get-PackageFileName '2Gis.Erm.UI.Desktop.WPF' 'zip'
	
	Sync-MSDeploy `
	-Source "package=""$packageFileName""" `
	-HostName $hostName
}
Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module .\modules\transform.psm1 -DisableNameChecking
Import-Module .\modules\msbuild.psm1 -DisableNameChecking
Import-Module .\modules\msdeploy.psm1 -DisableNameChecking
Import-Module .\modules\web.psm1 -DisableNameChecking

Properties{ $OptionWpfClient=$false }
Task Build-WpfClient -Precondition { return $OptionWpfClient } -Depends Update-AssemblyInfo {

	Build-WpfClientModule
	Build-WpfShell
}

Task Deploy-WpfClient -Precondition { return $OptionWpfClient } -Depends Build-WpfClient {
	$hostName = $global:Context.TargetHostName
	
	$wpfIisAppPath = Get-WpfIisAppPath
	Create-RemoteWebsite $wpfIisAppPath $hostName

	$artifactName = Get-Artifacts '' '2Gis.Erm.UI.Desktop.WPF.zip'
	
	Sync-MSDeploy `
	-Source "package=""$artifactName""" `
	-HostName $hostName
}

function Build-WpfShell {
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.UI.Desktop.WPF'
	
	Transform-Log4NetConfig $projectFileName
	
	$applicationVersion = $global:Context.Version.ToString(4)
	$productName = "2GIS ERM WPF Client $($global:Context.EnvironmentName) $applicationVersion"
	
	$wpfIisAppPath = Get-WpfIisAppPath
	$installUrl = New-Object System.UriBuilder('https', $wpfIisAppPath)

	$projectDir = Split-Path $projectFileName -Parent
	$conventionalPublishDir = Join-Path $projectDir 'publish'
	
	Invoke-MSBuild @(
	"""$projectFileName"""
	"/t:Publish"
	"/p:ApplicationVersion=$applicationVersion"
	"/p:IsWebBootstrapper=true"
	"/p:InstallUrl=$installUrl"
	"/p:UpdateUrl=$installUrl"
	"/p:ProductName=$productName"
	"/p:PublishDir=""$conventionalPublishDir\\"""
	)
	
	# TODO: auto generate publish.htm file
	Copy-Item (Join-Path $projectDir 'index.htm') $conventionalPublishDir

	# build zip package
	$wpfIisAppPath = Get-WpfIisAppPath
	$convensionalArtifactName = Create-ContentPackage $conventionalPublishDir $wpfIisAppPath
	$artifactName = Join-Path $global:Context.Dir.Temp '2Gis.Erm.UI.Desktop.WPF.zip'
	Copy-Item $convensionalArtifactName $artifactName
	Publish-Artifacts $artifactName
}

function Build-WpfClientModule {
	$projectFileName = Get-ProjectFileName '..\..\BL\Source\EntryPoints\UI\Desktop' '2Gis.Erm.BL.UI.WPF.Client'
	
	Transform-AppConfig $projectFileName
	
	Invoke-MSBuild @(
	"""$projectFileName"""
	)
}

# TODO: пока что локальный hashtable, потом запилим это в god-config
function Get-WpfIisAppPath {
	$iisAppPathMap = @{
		'Test.03' = 'wpf-app03.test.erm.russia'
		'Test.05' = 'wpf-app05.test.erm.russia'
		'Test.07' = 'wpf-app07.test.erm.russia'
		'Test.11' = 'wpf-app11.test.erm.russia'
		'Test.12' = 'wpf-app12.test.erm.russia'
		'Test.17' = 'wpf-app17.test.erm.russia'
		'Production.Russia' = 'wpf-app.erm.russia'
	}
	
	$environmentName = $global:Context.EnvironmentName
	$iisAppPath = $iisAppPathMap[$environmentName]
	if ($iisAppPath -eq $null){
		throw "Cannot find host for WPF environment $environmentName"
	}
	
	return $iisAppPath
}
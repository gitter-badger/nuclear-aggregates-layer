Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module .\modules\transform.psm1 -DisableNameChecking
Import-Module .\modules\msbuild.psm1 -DisableNameChecking
Import-Module .\modules\msdeploy.psm1 -DisableNameChecking
Import-Module .\modules\web.psm1 -DisableNameChecking
Import-Module .\modules\metadata.psm1 -DisableNameChecking

Properties{ $OptionWpfClient=$false }
Task Build-WpfClient -Precondition { return $OptionWpfClient } -Depends Update-AssemblyInfo {

	Build-WpfClientModule
	Build-WpfShell
}

Task Deploy-WpfClient -Precondition { return $OptionWpfClient } -Depends Build-WpfClient {
	
	$entryPointMetadata = Get-EntryPointMetadata '2Gis.Erm.UI.Desktop.WPF'
	$artifactName = Get-Artifacts '' '2Gis.Erm.UI.Desktop.WPF.zip'	
	
	foreach($targetHost in $EntryPointMetadata.TargetHosts){
		Create-RemoteWebsite $targetHost $entryPointMetadata.IisAppPath

		Sync-MSDeploy `
		-Source "package=""$artifactName""" `
		-HostName $targetHost
	}
}

function Build-WpfShell {
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.UI.Desktop.WPF'
	
	$applicationVersion = $global:Context.Version.ToString(4)
	$productName = "2GIS ERM WPF Client $($global:Context.EnvironmentName) $applicationVersion"
	
	$entryPointMetadata = Get-EntryPointMetadata '2Gis.Erm.UI.Desktop.WPF'
	$installUrl = New-Object System.UriBuilder('https', $entryPointMetadata.IisAppPath)

	$projectDir = Split-Path $projectFileName -Parent
	$conventionalPublishDir = Join-Path $projectDir 'publish'

	Transform-Log4NetConfig $projectFileName
	try {
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
	}
	finally {
		Restore-Log4NetConfig $projectFileName
	}
	
	# TODO: auto generate publish.htm file
	Copy-Item (Join-Path $projectDir 'index.htm') $conventionalPublishDir

	# build zip package
	$convensionalArtifactName = Create-ContentPackage $conventionalPublishDir $entryPointMetadata.IisAppPath
	$artifactName = Join-Path $global:Context.Dir.Temp '2Gis.Erm.UI.Desktop.WPF.zip'
	Copy-Item $convensionalArtifactName $artifactName
	Publish-Artifacts $artifactName
}

function Build-WpfClientModule {
	$projectFileName = Get-ProjectFileName '..\..\BLCore\Source\EntryPoints\UI\Desktop' '2Gis.Erm.BLCore.UI.WPF.Client'

	Transform-AppConfig $projectFileName
	try {
		Invoke-MSBuild @(
		"""$projectFileName"""
		)
	}
	finally {
		Restore-AppConfig $projectFileName
	}
}
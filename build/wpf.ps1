Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module "$BuildToolsRoot\modules\transform.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\msbuild.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\msdeploy.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\web.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\metadata.psm1" -DisableNameChecking

Properties { $OptionWpfClient = $true }

# WPF client disabled
Task Build-WpfClient -Precondition { $false } -Depends Update-AssemblyInfo {

	Build-WpfClientModule
	Build-WpfShell
}

Task Deploy-WpfClient -Precondition { $OptionWpfClient } {
	$artifactName = Get-Artifacts '' '2Gis.Erm.UI.Desktop.WPF.zip'
	
	$entryPointMetadata = Get-Metadata '2Gis.Erm.UI.Desktop.WPF'
	foreach($targetHost in $EntryPointMetadata.TargetHosts){
		Create-RemoteWebsite $targetHost $entryPointMetadata.IisAppPath

		Invoke-MSDeploy `
		-Source "package=""$artifactName""" `
		-HostName $targetHost
	}
}

function Build-WpfShell {
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.UI.Desktop.WPF'
	
	$commonMetadata = Get-Metadata 'Common'

	$productName = "2GIS ERM WPF Client $($commonMetadata.EnvironmentName) $($commonMetadata.Version.SemanticVersion)"
	
	$entryPointMetadata = Get-Metadata '2Gis.Erm.UI.Desktop.WPF'
	$installUrl = New-Object System.UriBuilder('https', $entryPointMetadata.IisAppPath)

	$projectDir = Split-Path $projectFileName
	$conventionalPublishDir = Join-Path $projectDir 'publish'

	$configFileName = Join-Path $projectDir 'log4net.config'
	$configXml = Transform-Config $configFileName

	$buildFileName = Create-BuildFile $projectFileName -Targets 'Publish' -Properties @{
		'ApplicationVersion' = $commonMetadata.Version.NumericVersion
		'IsWebBootstrapper' = $true
		'InstallUrl' = $installUrl
		'UpdateUrl' = $installUrl
		'ProductName' = $productName
		'PublishDir' = $conventionalPublishDir + '\'
	}  -CustomXmls $configXml
	
	Invoke-MSBuild $buildFileName

	# TODO: auto generate publish.htm file
	Copy-Item (Join-Path $projectDir 'index.htm') $conventionalPublishDir

	# build zip package
	$convensionalArtifactName = Create-ContentPackage $conventionalPublishDir $entryPointMetadata.IisAppPath
	$artifactName = Join-Path $commonMetadata.Dir.Temp '2Gis.Erm.UI.Desktop.WPF.zip'
	Copy-Item $convensionalArtifactName $artifactName
	Publish-Artifacts $artifactName
}

function Build-WpfClientModule {
	$projectFileName = Get-ProjectFileName '..\..\BLCore\Source\EntryPoints\UI\Desktop' '2Gis.Erm.BLCore.UI.WPF.Client'
	$projectDir = Split-Path $projectFileName
	
	$configFileName = Join-Path $projectDir 'app.config'
	$configXml = Transform-Config $configFileName
	
	$buildFileName = Create-BuildFile $projectFileName -Properties @{ 'AppConfig' = 'app.transformed.config' } -CustomXmls $configXml
	Invoke-MSBuild $buildFileName
}
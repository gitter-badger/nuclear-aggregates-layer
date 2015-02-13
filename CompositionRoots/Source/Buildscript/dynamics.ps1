Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module "$BuildToolsRoot\modules\msbuild.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\msdeploy.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\dynamics.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\metadata.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\sqlserver.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\web.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\transform.psm1" -DisableNameChecking

Properties{ $OptionDynamics=$false }
Task Build-Dynamics -Precondition { return $OptionDynamics } -Depends `
Build-HackFiles, `
Build-Plugins

Task Deploy-Dynamics -Precondition { return $OptionDynamics } -Depends `
Deploy-HackFiles, `
Deploy-Plugins, `
Update-CustomizationsXml, `
Replace-DynamicsStoredProcs

Task Build-HackFiles {
	
	$hackFilesDir = Join-Path $global:Context.Dir.Solution '..\..\BLCore\Source\ApplicationServices\2Gis.Erm.BLCore.MsCRM.Plugins\Customizations\Hack'
	$convensionalArtifactName = Create-ContentPackage $hackFilesDir 'Microsoft Dynamics CRM'
	$artifactFileName = Join-Path $global:Context.Dir.Temp '2Gis.Erm.BLCore.MsCRM.HackFiles.zip'

	Copy-Item $convensionalArtifactName $artifactFileName
	Publish-Artifacts $artifactFileName
}

Task Deploy-HackFiles {
	$artifactName = Get-Artifacts '' '2Gis.Erm.BLCore.MsCRM.HackFiles.zip'
	
	$entryPointMetadata = Get-EntryPointMetadata 'Dynamics'
	foreach($crmHost in $EntryPointMetadata.CrmHosts){
		# DoNotDeleteRule чтобы не затирать существующий сайт Dynamics CRM а дописывать в него контент
		Invoke-MSDeploy `
		-Source "package=""$artifactName""" `
		-HostName $crmHost `
		-Arguments @(
			"-enableRule:DoNotDeleteRule"
		)
	}
}

Task Build-Plugins -Depends Update-AssemblyInfo {

	$projectFileName = Get-ProjectFileName '..\..\BLCore\Source\ApplicationServices' '2Gis.Erm.BLCore.MsCRM.Plugins'
	$buildFileName = Create-BuildFile $projectFileName
	Invoke-MSBuild $buildFileName

	$convensionalArtifactName = Join-Path (Split-Path $projectFileName) 'bin\Release'
	Publish-Artifacts $convensionalArtifactName 'Plugin Registration'
}

Task Deploy-Plugins {
	$artifactName = Get-Artifacts 'Plugin Registration'
	
	$crmConnectionString = Get-ConnectionString 'CrmConnection'
	Unregister-Plugins $crmConnectionString '2Gis.Erm.*'

	$entryPointMetadata = Get-EntryPointMetadata '2Gis.Erm.API.WCF.Operations'
	$uriBuilder = New-Object System.UriBuilder('https', $entryPointMetadata.IisAppPath, -1, 'MsCrm.svc/Soap')
	$pluginRegistrationXml = Join-Path $artifactName 'PluginRegistration.xml'
	Register-Plugins $crmConnectionString $uriBuilder.ToString() $pluginRegistrationXml
}

Task Update-CustomizationsXml {
	$crmConnectionString = Get-ConnectionString 'CrmConnection'
	
	$xml = Export-CustomizationsXml $crmConnectionString

	$entryPointMetadata = Get-EntryPointMetadata '2Gis.Erm.UI.Web.Mvc'
	$uriBuilder = New-Object System.UriBuilder('https', $entryPointMetadata.IisAppPath)
	
	$replaceText = $uriBuilder.ToString()
	$processedXml = $xml -replace '(http|https)://((?!(www.w3.org)|(schemas.microsoft.com)|(schemas.xmlsoap.org))).*?/', $replaceText
	
	Import-CustomizationsXml $crmConnectionString $processedXml
}

Task Replace-DynamicsStoredProcs {
	
	$erm = Get-ConnectionString 'Erm'
	$ermReports = Get-ConnectionString 'ErmReports'
	$dynamics = Get-ConnectionString 'CrmConnection'
	
	$parsed = Parse-CrmConnectionString $dynamics
	
	Replace-StoredProcs $erm '\bDoubleGis[0-9]*' $parsed.OrganizationName
	Replace-StoredProcs $ermReports '\bDoubleGis[0-9]*' $parsed.OrganizationName
}
Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module .\modules\msbuild.psm1 -DisableNameChecking
Import-Module .\modules\msdeploy.psm1 -DisableNameChecking
Import-Module .\modules\dynamics.psm1 -DisableNameChecking
Import-Module .\modules\metadata.psm1 -DisableNameChecking
Import-Module .\modules\sqlserver.psm1 -DisableNameChecking
Import-Module .\modules\web.psm1 -DisableNameChecking
Import-Module .\modules\config.psm1 -DisableNameChecking

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

Task Deploy-HackFiles -Depends Build-HackFiles {
	$artifactName = Get-Artifacts '' '2Gis.Erm.BLCore.MsCRM.HackFiles.zip'
	
	$entryPointMetadata = Get-EntryPointMetadata '2Gis.Erm.UI.Web.Mvc'
	
	foreach($targetHost in $EntryPointMetadata.TargetHosts){
		# DoNotDeleteRule чтобы не затирать существующий сайт Dynamics CRM а дописывать в него контент
		Invoke-MSDeploy `
		-Source "package=""$artifactName""" `
		-HostName $targetHost `
		-Arguments @(
			"-enableRule:DoNotDeleteRule"
		)
	}
}

Task Build-Plugins -Depends Update-AssemblyInfo {
	$projectFileName = Get-ProjectFileName '..\..\BLCore\Source\ApplicationServices' '2Gis.Erm.BLCore.MsCRM.Plugins'
	
	Invoke-MSBuild @(
	"""$projectFileName"""
	)

	$convensionalArtifactName = Join-Path (Split-Path $projectFileName) 'bin\Release'
	Publish-Artifacts $convensionalArtifactName 'Plugin Registration'
}

Task Deploy-Plugins -Depends Build-Plugins {
	
	$crmConnectionString = Get-ConnectionString 'CrmConnection'
	
	Unregister-Plugins $crmConnectionString '2Gis.Erm.*'

	$entryPointMetadata = Get-EntryPointMetadata '2Gis.Erm.API.WCF.Operations'
	$uriBuilder = New-Object System.UriBuilder('https', $entryPointMetadata.IisAppPath, -1, 'MsCrm.svc/Soap')

	$artifactName = Get-Artifacts 'Plugin Registration'
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

Task Take-DynamicsOffline -Precondition { return $OptionDynamics } {
	$entryPointMetadata = Get-EntryPointMetadata '2Gis.Erm.UI.Web.Mvc'
	Take-WebsiteOffline $entryPointMetadata.TargetHosts '\"Default Web Site\"'
}

Task Take-DynamicsOnline -Precondition { return $OptionDynamics } {
	$entryPointMetadata = Get-EntryPointMetadata '2Gis.Erm.UI.Web.Mvc'
	Take-WebsiteOnline $entryPointMetadata.TargetHosts '\"Default Web Site\"'
}
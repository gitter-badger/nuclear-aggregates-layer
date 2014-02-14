Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module .\modules\msbuild.psm1 -DisableNameChecking
Import-Module .\modules\msdeploy.psm1 -DisableNameChecking
Import-Module .\modules\dynamics.psm1 -DisableNameChecking
Import-Module .\modules\web.psm1 -DisableNameChecking
Import-Module .\modules\sqlserver.psm1 -DisableNameChecking
Import-Module .\modules\config.psm1 -DisableNameChecking

Properties{ $OptionDynamics=$false }
Task Build-Dynamics -precondition { return $OptionDynamics } -Depends `
Build-HackFiles, `
Build-Plugins

Task Deploy-Dynamics -precondition { return $OptionDynamics } -Depends `
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
	$hostName = $global:Context.TargetHostName
	$artifactName = Get-Artifacts '' '2Gis.Erm.BLCore.MsCRM.HackFiles.zip'
	
	# DoNotDeleteRule чтобы не затирать существующий сайт Dynamics CRM а дописывать в него контент
	Sync-MSDeploy `
	-Source "package=""$artifactName""" `
	-HostName $hostName `
	-Arguments @(
		"-enableRule:DoNotDeleteRule"
	)
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

	$publishProfile = Get-PublishProfileForOperations
	
	# используем протокол http, пока клиент экспорта н готов использовать https
	$publishProfile.UriBuilder.Scheme = 'https'
	$publishProfile.UriBuilder.Path = 'MsCrm.svc/Soap'

	$artifactName = Get-Artifacts 'Plugin Registration'
	$pluginRegistrationXml = Join-Path $artifactName 'PluginRegistration.xml'

	Register-Plugins $crmConnectionString $publishProfile.UriBuilder.ToString() $pluginRegistrationXml
}

Task Update-CustomizationsXml {
	$crmConnectionString = Get-ConnectionString 'CrmConnection'
	
	$xml = Export-CustomizationsXml $crmConnectionString

	$publishProfile = Get-PublishProfileForWebApp
	$replaceText = $publishProfile.UriBuilder.ToString()
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
Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module .\modules\msbuild.psm1 -DisableNameChecking
Import-Module .\modules\msdeploy.psm1 -DisableNameChecking
Import-Module .\modules\dynamics.psm1 -DisableNameChecking
Import-Module .\modules\web.psm1 -DisableNameChecking
Import-Module .\modules\sqlserver.psm1 -DisableNameChecking
Import-Module .\modules\godconfig.psm1 -DisableNameChecking

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
	
	$hackFilesDir = Join-Path $global:Context.Dir.Solution '..\..\BL\Source\ApplicationServices\2Gis.Erm.BL.MsCRM.Plugins\Customizations\Hack' -Resolve
	$packageFileName = Get-PackageFileName 'HackFiles' 'zip'

	Create-ContentPackage $hackFilesDir 'Microsoft Dynamics CRM' $packageFileName
	
	Write-Output "##teamcity[publishArtifacts '$packageFileName => Dynamics CRM']"
}

Task Deploy-HackFiles -Depends Build-HackFiles {
	$hostName = $global:Context.TargetHostName
	$packageFileName = Get-PackageFileName 'HackFiles' 'zip'
	
	Sync-MSDeploy `
	-Source "package=""$packageFileName""" `
	-HostName $hostName `
	-Arguments @(
		"-enableRule:DoNotDeleteRule"
	)
}

Task Build-Plugins -Depends Update-AssemblyInfo {
	$projectFileName = Get-ProjectFileName '..\..\BL\Source\ApplicationServices' '2Gis.Erm.BL.MsCRM.Plugins'
	$packageFileName = Get-PackageFileName 'plugins'
	
	Invoke-MSBuild @(
	"""$projectFileName"""
	"/p:OutDir=$packageFileName"
	)

	Write-Output "##teamcity[publishArtifacts '$packageFileName => Dynamics CRM\PluginRegistration']"
}

Task Deploy-Plugins -Depends Build-Plugins {
	
	$crmConnectionString = Get-ConnectionString 'CrmConnection'
	
	Unregister-Plugins $crmConnectionString '2Gis.Erm.*'

	$publishProfile = Get-WebAppPublishProfile
	$uriBuilder = New-Object System.UriBuilder($publishProfile.Uri)
	$uriBuilder.Path = 'ErmService.svc'

	$packageFileName = Get-PackageFileName 'plugins'
	$pluginRegistrationXml = Join-Path $packageFileName 'PluginRegistration.xml'

	Register-Plugins $crmConnectionString $uriBuilder.Uri.ToString() $pluginRegistrationXml
}

Task Update-CustomizationsXml {
	$crmConnectionString = Get-ConnectionString 'CrmConnection'
	
	$xml = Export-CustomizationsXml $crmConnectionString

	$publishProfile = Get-WebAppPublishProfile
	$replaceText = $publishProfile.Uri.ToString()
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
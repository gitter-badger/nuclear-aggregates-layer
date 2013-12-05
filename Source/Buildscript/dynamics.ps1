Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module .\modules\msbuild.psm1 -DisableNameChecking
Import-Module .\modules\msdeploy.psm1 -DisableNameChecking
Import-Module .\modules\dynamics.psm1 -DisableNameChecking
Import-Module .\modules\web.psm1 -DisableNameChecking
Import-Module .\modules\sqlserver.psm1 -DisableNameChecking

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
	
	$hackFiles = Join-Path $global:Context.Dir.Solution '..\..\BL\Source\ApplicationServices\Customizations\Hack\*'
	$hackFilesManifestPath = Join-Path $global:Context.Dir.Temp 'HackFilesManifest.xml'
	$packageFileName = Get-PackageFileName 'HackFiles' 'zip'

	Copy-Item $hackFiles 'C:\inetpub\wwwroot' -Recurse -Force
	
	[xml]$hackFilesManifest = @"
<sitemanifest>

  <!-- dynamics crm hack files -->
  <filePath path="C:\inetpub\wwwroot\_root\bar_Top.aspx" />
  <filePath path="C:\inetpub\wwwroot\_root\HomePage.aspx" />
  <filePath path="C:\inetpub\wwwroot\_static\_common\scripts\Global.js" />
  <filePath path="C:\inetpub\wwwroot\_static\_controls\lookup\LookupDialogsAppGrid.js" />
  <filePath path="C:\inetpub\wwwroot\_static\_grid\action.js" />
  <filePath path="C:\inetpub\wwwroot\_static\_grid\grid.htc" />
  <filePath path="C:\inetpub\wwwroot\Activities\dlg_create.aspx" />
  <filePath path="C:\inetpub\wwwroot\AdvancedFind\fetchData.aspx" />
  <filePath path="C:\inetpub\wwwroot\MA\MiniCampaign\MiniCampaign.aspx" />
  <filePath path="C:\inetpub\wwwroot\UserDefined\edit.aspx" />

</sitemanifest>
"@
	$hackFilesManifest.Save($hackFilesManifestPath)
	
	Sync-MSDeploy `
	-Source "manifest=""$hackFilesManifestPath""" `
	-Dest "package=""$packageFileName"""
	
	Write-Output "##teamcity[publishArtifacts '$packageFileName => Dynamics CRM']"
}

Task Deploy-HackFiles -Depends Build-HackFiles {
	$hostName = $global:Context.TargetHostName
	$packageFileName = Get-PackageFileName 'HackFiles' 'zip'
	
	Sync-MSDeploy `
	-Source "package=""$packageFileName""" `
	-HostName $hostName
}

Task Build-Plugins -Depends Update-AssemblyInfo {
	$projectFileName = Get-ProjectFileName '..\..\BL\Source\ApplicationServices' '2Gis.Erm.BL.MsCRM.Plugins'
	$packageFileName = Get-PackageFileName 'plugins'
	
	Invoke-MSBuild -Arguments @(
	"""$projectFileName"""
	"/p:OutDir=$packageFileName"
	)

	Write-Output "##teamcity[publishArtifacts '$packageFileName => Dynamics CRM\PluginRegistration']"
}

Task Deploy-Plugins -Depends Build-Plugins {
	$packageFileName = Get-PackageFileName 'plugins'
	
	$assemblyFileName = Join-Path $packageFileName '2Gis.Erm.BL.MsCRM.Plugins.dll'
	$webAppInfo = Get-WebAppInfo
	$crmConnectionString = Get-ConnectionString $webAppInfo.TransformedConfigFileName 'CrmConnection'
	
	Unregister-Plugins $crmConnectionString $assemblyFileName
	
	$uriBuilder = New-Object System.UriBuilder('http', $webAppInfo.PublishProfile.IisAppPath)
	$uriBuilder.Path = 'ErmService.svc'
	$pluginRegistrationXml = Join-Path $packageFileName 'PluginRegistration.xml'
	Register-Plugins $crmConnectionString $uriBuilder.Uri.ToString() $pluginRegistrationXml
}

Task Update-CustomizationsXml {
	$webAppInfo = Get-WebAppInfo
	$uriBuilder = New-Object System.UriBuilder('https', $webAppInfo.PublishProfile.IisAppPath)
	$crmConnectionString = Get-ConnectionString $webAppInfo.TransformedConfigFileName 'CrmConnection'
	
	Update-CustomizationsXml $crmConnectionString $uriBuilder.Uri.ToString()
}

Task Replace-DynamicsStoredProcs {
	$webAppInfo = Get-WebAppInfo
	
	Replace-StoredProcs $webAppInfo.TransformedConfigFileName 'Erm' 'CrmConnection'
	Replace-StoredProcs $webAppInfo.TransformedConfigFileName 'ErmReports' 'CrmConnection'
}
Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module "$PSScriptRoot\nuget.psm1" -DisableNameChecking

$PackageInfo = Get-PackageInfo 'Microsoft.CrmSdk'
Add-Type -Path (Join-Path $PackageInfo.VersionedDir 'lib\net35\microsoft.crm.sdk.dll')
Add-Type -Path (Join-Path $PackageInfo.VersionedDir 'lib\net35\microsoft.crm.sdktypeproxy.dll')
Add-Type -Path (Join-Path $PackageInfo.VersionedDir 'lib\net35\microsoft.crm.sdktypeproxy.xmlserializers.dll')
Add-Type -Path (Join-Path $PackageInfo.VersionedDir 'lib\net35\microsoft.xrm.client.dll')

$PluginRegistrationPath = Join-Path $PackageInfo.VersionedDir 'tools\PluginRegistration.exe'
[System.Reflection.Assembly]::LoadFrom($PluginRegistrationPath) | Out-Null

function Create-CrmDataContext ($CrmConnectionString) {

	$crmConnection = [Microsoft.Xrm.Client.CrmConnection]::Parse($CrmConnectionString)
	$crmConnection.Timeout = [System.Threading.Timeout]::Infinite
	$сrmDataContext = New-Object Microsoft.Xrm.Client.Data.Services.CrmDataContext($null, {
		return New-Object Microsoft.Xrm.Client.Services.OrganizationService($null, $crmConnection)
	}.GetNewClosure())

	return $сrmDataContext
}

function Import-CustomizationsXml($CrmConnectionString, $Xml, $WaitAttempts = 5){
	$crmDataContext = Create-CrmDataContext $CrmConnectionString

	for ($i = 0; $i -lt $WaitAttempts; $i++){

		try{
			$crmDataContext.UsingService([System.Action`1[Microsoft.Xrm.Client.Services.IOrganizationService]]{
				param($Service)

				$importAllXmlRequest = New-Object Microsoft.Crm.SdkTypeProxy.ImportAllXmlRequest
				$importAllXmlRequest.CustomizationXml = $Xml
				$Service.Execute($importAllXmlRequest)

				$publishAllXmlRequest = New-Object Microsoft.Crm.SdkTypeProxy.PublishAllXmlRequest
				$Service.Execute($publishAllXmlRequest)
			})
			break
		}
		catch [System.Web.Services.Protocols.SoapException] {

			# may fail because of deadlocks or optimistic concurrency
			if ($_.Exception.Detail.InnerText -match 'Generic SQL error'){
				Write-Host "'Generic SQL error' while import (attempt $($i + 1))"
				Start-Sleep -Second 5
			}
			else {
				throw
			}
		}
	}

	if ($i -eq $WaitAttempts){
		throw "Failed to update customizations.xml after $WaitAttempts attempts"
	}
}

function Export-CustomizationsXml($CrmConnectionString){
	$crmDataContext = Create-CrmDataContext $CrmConnectionString

	try{
		$exportXmlResponse = $crmDataContext.UsingService([System.Func`2[Microsoft.Xrm.Client.Services.IOrganizationService, Microsoft.Crm.SdkTypeProxy.ExportXmlResponse]]{
	        param($Service)

	        $exportXmlRequest = New-Object Microsoft.Crm.SdkTypeProxy.ExportXmlRequest
	        $exportXmlRequest.ParameterXml = @"
<importexportxml>
    <entities></entities>
        <nodes>
            <node>sitemap</node>
            <node>isvconfig</node>
        </nodes>
    <securityroles></securityroles>
    <settings></settings>
    <workflows></workflows>
</importexportxml>
"@

	        return $Service.Execute($exportXmlRequest)
	    })

	    return $exportXmlResponse.ExportXml	
	}
	catch [System.Web.Services.Protocols.SoapException] {
		throw $_.Exception.Detail.InnerText
	}
}

function Unregister-Plugins ($CrmConnectionString, $PluginNamePattern) {
	
	$crmDataContext = Create-CrmDataContext $CrmConnectionString
	
	try{
		$pluginAssemblies = $crmDataContext.GetEntities([Microsoft.Crm.SdkTypeProxy.EntityName]::pluginassembly.ToString()) | where { $_['name'].Value -match  $PluginNamePattern }

		foreach($pluginAssembly in $pluginAssemblies){
			$CrmDataContext.UsingService([System.Action`1[Microsoft.Xrm.Client.Services.IOrganizationService]]{
				param($Service)

				$unregisterSolutionRequest = New-Object Microsoft.Crm.SdkTypeProxy.UnregisterSolutionRequest
				$unregisterSolutionRequest.PluginAssemblyId = $pluginAssembly.Id

				$Service.Execute($unregisterSolutionRequest)
			})
		}
	}
	catch [System.Web.Services.Protocols.SoapException] {
		throw $_.Exception.Detail.InnerText
	}
}

function Register-Plugins ($CrmConnectionString, $ErmIntegrationUrl, $PathToPluginRegistrationXml) {

	Replace-ErmIntegrationUrl $PathToPluginRegistrationXml $ErmIntegrationUrl
	$parsed = Parse-CrmConnectionString $CrmConnectionString

	try{
		$organization = Get-Organization $parsed
		$crmPluginAssemblies = [PluginRegistrationTool.ImportExport]::GetAssemblyNodes($organization, $PathToPluginRegistrationXml, $true)
		$crmPluginAssemblyDir = Split-Path $PathToPluginRegistrationXml -Parent
		
		$assemblyPathsDict = New-Object 'System.Collections.Generic.Dictionary``2[System.Guid, System.String]'
		foreach($crmPluginAssembly in $crmPluginAssemblies){
		
			$crmPluginAssemblyPath = Join-Path $crmPluginAssemblyDir $CrmPluginAssembly.ServerFileName
	        if (!(Test-Path $crmPluginAssemblyPath)){
		        throw "Не найден путь '$crmPluginAssemblyPath'"
	        }

	        # Name
			$assemblyName = [System.Reflection.AssemblyName]::GetAssemblyName($crmPluginAssemblyPath)
	        $crmPluginAssembly.AssemblyName = $assemblyName.Name

	        # Culture
	        if (![string]::IsNullOrEmpty($assemblyName.CultureName)){
		        $crmPluginAssembly.Culture = $assemblyName.CultureName
	        }
	        else{
		        $crmPluginAssembly.Culture = 'neutral'
	        }

	        # PublicKeyToken
	        $stringBuilder = New-Object System.Text.StringBuilder
	        foreach($byte in $assemblyName.GetPublicKeyToken()){
		        $stringBuilder.Append($byte.ToString('x')) | Out-Null
	        }
	        $crmPluginAssembly.PublicKeyToken = $stringBuilder.ToString()

	        # Version
	        $crmPluginAssembly.Version = $assemblyName.Version.ToString()

			$assemblyPathsDict.Add($crmPluginAssembly.AssemblyId, $crmPluginAssemblyPath)
		}
		
		[PluginRegistrationTool.ImportExport]::ImportSolution($organization, $crmPluginAssemblies, $null, $assemblyPathsDict)	
	}
	catch [System.Web.Services.Protocols.SoapException] {
		throw $_.Exception.Detail.InnerText
	}
}

function Replace-ErmIntegrationUrl($PathToPluginRegistrationXml, $ErmIntegrationUrl){
    [xml]$pluginRegistrationXml = Get-Content -Path $PathToPluginRegistrationXml -Encoding UTF8

    foreach($step in $pluginRegistrationXml.SelectNodes('//Step')){
        if ($step.PluginTypeName -like '*UpdateAndReplicateOpportunityToErmPlugin'){ $step.CustomConfiguration = $ErmIntegrationUrl }
        if ($step.PluginTypeName -like '*AfterSalePhonecallStateChangedPlugin'){ $step.CustomConfiguration = $ErmIntegrationUrl }
    }

    $pluginRegistrationXml.Save($PathToPluginRegistrationXml)
}

function Get-Organization($Parsed){

    $сrmConnection = New-Object PluginRegistrationTool.CrmConnection('Unknown', $Parsed.DynamicsCrmServer, $Parsed.Port, $null, $null, $null)
    $сrmConnection.RetrieveOrganizations()

    $organization = $сrmConnection.Organizations | where { $_.OrganizationUniqueName -eq $Parsed.OrganizationName } | select -First 1
    if ($organization -eq $null){
        throw "Не найдена организация '$($Parsed.OrganizationName)'"
    }
    $organization.CrmServiceUrl = $Parsed.DynamicsCrmServer + '/MSCrmServices/2007/CrmService.asmx'

    $messagesDict = New-Object 'System.Collections.Generic.Dictionary``2[System.Guid, PluginRegistrationTool.CrmMessage]'
    $messages = [PluginRegistrationTool.OrganizationHelper]::LoadMessages($organization)
    foreach($message in $messages){
        $messagesDict.Add($message.MessageId, $message)
    }

    $crmEntityDictionay = New-Object 'PluginRegistrationTool.CrmEntityDictionary``1[PluginRegistrationTool.CrmMessage]'($messagesDict)
    [PluginRegistrationTool.OrganizationHelper]::OpenConnection($organization, $crmEntityDictionay)

    return $organization
}

function Parse-CrmConnectionString($CrmConnectionString){

    $connectionStringBuilder = New-Object System.Data.Common.DbConnectionStringBuilder
    $connectionStringBuilder.set_ConnectionString($CrmConnectionString)

    $organizationUrl = $connectionStringBuilder['server']
    $uriBuilder = New-Object System.UriBuilder($organizationUrl)
    $dynamicsCrmServer = $uriBuilder.Scheme + '://' + $uriBuilder.Host;
    $organizationName = $uriBuilder.Path.Trim('/');

    return @{
        'DynamicsCrmServer' = $dynamicsCrmServer
        'OrganizationName' = $organizationName
        'Port' = $uriBuilder.Port
    }
}

Export-ModuleMember -Function Parse-CrmConnectionString, Unregister-Plugins, Register-Plugins, Import-CustomizationsXml, Export-CustomizationsXml
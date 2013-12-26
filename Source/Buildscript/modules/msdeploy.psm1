Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

$MsDeploySdk_3_x64 = "${Env:ProgramFiles}\IIS\Microsoft Web Deploy V3"
$MsDeploy_3_x64 = Join-Path $MsDeploySdk_3_x64 'msdeploy.exe'

function Sync-MSDeploy ($Source, $Dest = 'auto', $HostName = $null, $AuthType = 'NTLM', $Arguments = @()) {

	$finalDest = $Dest
	if ($HostName -ne $null){
		$uriBuilder = New-Object System.UriBuilder('https', $HostName, 8172, 'msdeploy.axd')
		$finalDest = "$Dest,ComputerName=""$uriBuilder"",AuthType=$AuthType"
	}

	$allArguments = @(
	"-verb:sync"
	"-source:$Source"
	"-dest:$finalDest"
	"-allowUntrusted"
	"-retryAttempts:2"
	"-disableLink:AppPoolExtension"
	"-disableLink:ContentExtension"
	"-disableLink:CertificateExtension"
	"-disablerule:BackupRule"
	) + $Arguments

	& $MsDeploy_3_x64 $allArguments | Out-Null
	if ($LastExitCode -ne 0) {
		throw "Command failed with exit code $LastExitCode"
	}
}

function Create-ContentPackage($SourceContentPath, $DestContentPath, $PackageFileName){
	$declareParamFileName = Join-Path $global:Context.Dir.Temp 'parameters.xml'
	
	[xml]$declareParamXml = @"
<?xml version="1.0" encoding="utf-8"?>
<parameters>
  <parameter name="ContentPath" defaultValue="$DestContentPath">
    <parameterEntry kind="ProviderPath" scope="contentPath" />
  </parameter>
</parameters>
"@
	New-Item $declareParamFileName -Type File -Force | Out-Null
	$declareParamXml.Save($declareParamFileName)
	
	Sync-MSDeploy `
	-Source "contentPath=""$SourceContentPath""" `
	-Dest "package=""$PackageFileName""" `
	-Arguments @(
		"-declareParamFile=""$declareParamFileName"""
	)
}

Export-ModuleMember -Function Sync-MSDeploy, Create-ContentPackage
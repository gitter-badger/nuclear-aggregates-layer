Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

$MsDeployVersion = '3'

$MsDeployDir_x64 = "${Env:ProgramFiles}\IIS\Microsoft Web Deploy V$MsDeployVersion"
$MsDeployPath_x64 = Join-Path $MsDeployDir_x64 'msdeploy.exe'

function Invoke-MSDeploy ($Source, $Dest = 'auto', $HostName = $null, $AuthType = 'NTLM', $Arguments = @(), $Verb = 'sync') {

	$finalDest = $Dest
	if ($HostName -ne $null){
		$uriBuilder = New-Object System.UriBuilder('https', $HostName, 8172, 'msdeploy.axd')
		$finalDest = "$Dest,ComputerName=$uriBuilder,AuthType=$AuthType"
	}
	
	$finalSource = "-source:$Source"
	$finalDest = "-dest:$finalDest"

	# msdeploy commandline parsed requires parameters with spaces should be splitted
	$sourceArray = $finalSource.Split(@(' '), 'RemoveEmptyEntries')
	$destArray = $finalDest.Split(@(' '), 'RemoveEmptyEntries')

	$allArguments = @(
	"-verb:$Verb") + `
	$sourceArray + `
	$destArray + @(
	'-allowUntrusted'
	'-retryAttempts:5'
	'-retryInterval:5000'
	'-disableLink:AppPoolExtension'
	'-disableLink:ContentExtension'
	'-disableLink:CertificateExtension'
	'-disableRule:BackupRule'
	) + $Arguments

	#& 'path-to-echoargs.exe' $allArguments
	& $MsDeployPath_x64 $allArguments | Out-Null
	if ($LastExitCode -ne 0) {
		throw "Command failed with exit code $LastExitCode"
	}
}

function Create-ContentPackage($SourceContentPath, $DestContentPath){
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
	
	$resolvedSourceContentPath = Resolve-Path $SourceContentPath

	# convension: пакет создаётся на одну папку выше чем ContentPath
	$packageFileName =  Join-Path $SourceContentPath '..\package.zip'
	$resolvedPackageFileName = [System.IO.Path]::GetFullPath($packageFileName)
	
	Invoke-MSDeploy `
	-Source "contentPath=""$resolvedSourceContentPath""" `
	-Dest "package=""$resolvedPackageFileName""" `
	-Arguments @(
		"-declareParamFile=""$declareParamFileName"""
	)
	
	return $resolvedPackageFileName
}

Export-ModuleMember -Function Invoke-MSDeploy, Create-ContentPackage
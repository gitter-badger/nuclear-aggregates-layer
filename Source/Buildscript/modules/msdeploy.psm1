Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

$MsDeploySdk_3_x64 = "${Env:ProgramFiles}\IIS\Microsoft Web Deploy V3"
$MsDeploy_3_x64 = Join-Path $MsDeploySdk_3_x64 'msdeploy.exe'

function Sync-MSDeploy ($Source, $Dest = 'auto', $HostName = $null, $AuthType = 'NTLM') {

	$finalDest = $Dest
	if ($HostName -ne $null){
		$uriBuilder = New-Object System.UriBuilder('https', $HostName, 8172, 'msdeploy.axd')
		$finalDest = $Dest + ",ComputerName=""$($uriBuilder.Uri)"",AuthType=""$AuthType"""
	}

	$arguments = @(
	"-verb:sync"
	"-source:$Source"
	"-dest:$finalDest"
	"-allowUntrusted"
	"-retryAttempts:2"
	"-disableLink:AppPoolExtension"
	"-disableLink:ContentExtension"
	"-disableLink:CertificateExtension"
	"-disablerule:BackupRule"
	)

	& $MsDeploy_3_x64 $arguments | Out-Null
	if ($LastExitCode -ne 0) {
		throw "Command failed with exit code $LastExitCode"
	}
}

Export-ModuleMember -Function Sync-MSDeploy
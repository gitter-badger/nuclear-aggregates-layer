Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

$MsDeploy3x64 = "${Env:ProgramFiles}\IIS\Microsoft Web Deploy V3\msdeploy.exe"

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

	Exec {
		& $MsDeploy3x64 $arguments | Out-Null
	}
}

Export-ModuleMember -Function Sync-MSDeploy
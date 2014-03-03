Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

$ThisDir = Split-Path $MyInvocation.MyCommand.Path -Parent

$NuGet_2_7_3 = Join-Path $ThisDir '..\..\packages\NuGet.CommandLine.2.7.3\tools\NuGet.exe'

function Invoke-NuGet ($Arguments) {

	$allArguments = $Arguments + @(
		"-NonInteractive"
		"-Verbosity"
		"quiet"
	)

	$LastExitCode = 0
	& $NuGet_2_7_3 $allArguments
	if ($LastExitCode -ne 0) {
		throw "Command failed with exit code $LastExitCode"
	}
}

Export-ModuleMember -Function Invoke-NuGet
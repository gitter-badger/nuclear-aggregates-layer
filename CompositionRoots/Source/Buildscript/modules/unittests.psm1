Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module "$PSScriptRoot\nuget.psm1" -DisableNameChecking

$PackageInfo = Get-PackageInfo 'Machine.Specifications-Signed'
$MSpecPath = Join-Path $PackageInfo.VersionedDir "tools\mspec-clr4.exe"

function Invoke-MSpec ($Arguments) {

	$allArguments = @()
	if (Test-Path 'Env:\TEAMCITY_VERSION') {
		$allArguments += '--teamcity'
	}

	$allArguments += @(
		'--silent'
	) + $Arguments

	& $MSpecPath $allArguments
	
	# MSpec exit codes: 0 - Success; 1 - Some tests failed; -1 - Error in MSpec
	if ($lastExitCode -eq -1) {
		throw "Command failed with exit code $lastExitCode"
	}
}

function Run-UnitTests ($Projects){

	$assemblies = $Projects | Select-Object  @{Name = 'Expand' ; Expression = {
		$conventionalAssemblyName = [System.IO.Path]::ChangeExtension($_.Name, '.dll')
		$convensionalAssemblyDir = Join-Path (Split-Path $_.FullName) 'bin\Release'
		$convensionalAssemblyFileName = Join-Path $convensionalAssemblyDir $conventionalAssemblyName
		if (!(Test-Path $convensionalAssemblyFileName)){
			throw "Can't find project dir $convensionalAssemblyFileName"
		}
		return $convensionalAssemblyFileName
	}} | Select-Object -ExpandProperty 'Expand'

	Invoke-MSpec $assemblies
}

Export-ModuleMember -Function Run-UnitTests
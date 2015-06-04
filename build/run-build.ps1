param([string[]]$TaskList = @(), [hashtable]$Properties = @{})

if ($TaskList.Count -eq 0){
	$TaskList = @('Run-UnitTests', 'Build-NuGetPackages', 'Deploy-NuGet')
}

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------
cls

$Properties.SemanticVersion = '0.1.0'
$Properties.BuildFile = Join-Path $PSScriptRoot 'default.ps1'
$Properties.SolutionDir = Join-Path $PSScriptRoot '..'

# Restore-Packages
& {
	$NugetPath = Join-Path $Properties.SolutionDir '.nuget\NuGet.exe'
	if (!(Test-Path $NugetPath)){
		$webClient = New-Object System.Net.WebClient
		$webClient.UseDefaultCredentials = $true
		$webClient.Proxy.Credentials = $webClient.Credentials
		$webClient.DownloadFile('https://www.nuget.org/nuget.exe', $NugetPath)
	}
	$solution = Get-ChildItem $Properties.SolutionDir -Filter '*.sln'
	& $NugetPath @('restore', $solution.FullName, '-NonInteractive', '-Verbosity', 'quiet')
}

Import-Module "$($Properties.SolutionDir)\packages\2GIS.NuClear.BuildTools.0.0.42\tools\buildtools.psm1" -DisableNameChecking -Force
Run-Build $TaskList $Properties

param([string[]]$TaskList = @(), [hashtable]$Properties = @{})

if ($TaskList.Count -eq 0){
	$TaskList = @('Build-TaskService', 'Deploy-TaskService')
}
if ($Properties.Count -eq 0){
	$Properties.EnvironmentName = 'Test.21'
	}

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------
cls

$Properties.SemanticVersion = '2.95.0'
$Properties.SolutionDir = Join-Path $PSScriptRoot '..\CompositionRoots\Source'
$Properties.BuildFile = Join-Path $PSScriptRoot 'default.ps1'

Import-Module "$PSScriptRoot\metadata.psm1" -DisableNameChecking
$Properties.EnvironmentMetadata = $EnvironmentMetadata

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

Import-Module "$($Properties.SolutionDir)\packages\2GIS.NuClear.BuildTools.0.0.40\tools\buildtools.psm1" -DisableNameChecking -Force
Run-Build $TaskList $Properties

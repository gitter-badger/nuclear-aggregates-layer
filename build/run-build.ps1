param([string[]]$TaskList = @(), [hashtable]$Properties = @{})

if ($TaskList.Count -eq 0){
<<<<<<< HEAD
	$TaskList = @('Run-UnitTests', 'Build-NuGetPackages', 'Deploy-NuGet')
}
=======
	$TaskList = @('Build-TaskService', 'Deploy-TaskService')
}
if ($Properties.Count -eq 0){
	$Properties.EnvironmentName = 'Test.21'
	}
>>>>>>> feature/ERM-6409-Aggregates

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------
cls

<<<<<<< HEAD
$Properties.SemanticVersion = '0.1.0'
$Properties.BuildFile = Join-Path $PSScriptRoot 'default.ps1'
$Properties.SolutionDir = Join-Path $PSScriptRoot '..'
=======
$Properties.SemanticVersion = '2.96.0'
$Properties.SolutionDir = Join-Path $PSScriptRoot '..\CompositionRoots\Source'
$Properties.BuildFile = Join-Path $PSScriptRoot 'default.ps1'

Import-Module "$PSScriptRoot\metadata.psm1" -DisableNameChecking
$Properties.EnvironmentMetadata = $EnvironmentMetadata
>>>>>>> feature/ERM-6409-Aggregates

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

<<<<<<< HEAD
Import-Module "$($Properties.SolutionDir)\packages\2GIS.NuClear.BuildTools.0.0.42\tools\buildtools.psm1" -DisableNameChecking -Force
=======
Import-Module "$($Properties.SolutionDir)\packages\2GIS.NuClear.BuildTools.0.0.40\tools\buildtools.psm1" -DisableNameChecking -Force
>>>>>>> feature/ERM-6409-Aggregates
Run-Build $TaskList $Properties

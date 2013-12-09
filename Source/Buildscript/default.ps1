Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

Properties{
	$Revision = 0
	$Build = 0
	$TargetHostName = 'NOT SET'
    $EnvironmentName = 'NOT SET'
	$Region = 'NOT SET'
	$Verbosity = 'normal'
}

Include versioning.ps1
Include migrations.ps1
Include web.ps1
Include wpf.ps1
Include taskservice.ps1
Include dynamics.ps1
Include reports.ps1
Include unittests.ps1

# имена тасков в формате TeamCity
FormatTaskName "##teamcity[progressMessage '{0}']"

Task Default -depends Hello
Task Hello { "Билдскрипт запущен без цели, укажите цель" }

Task Build-Packages -Depends `
Create-GlobalContext, `
Set-BuildNumber, `
Build-TaskService, `
Build-Web, `
Build-Wpf, `
Build-Dynamics, `
Add-AdditionalArtifacts

Task Deploy-Packages -Depends `
Create-GlobalContext, `
Set-BuildNumber, `
Deploy-TaskService, `
Deploy-Web, `
Deploy-Dynamics, `
Deploy-Reports

Task Create-GlobalContext {

	$buildDir = Resolve-Path .

	$global:Context = @{
		'Version' = New-Object System.Version(2, 0, $Revision, $Build)
		'Dir' = @{
			'Solution' = Join-Path $buildDir '..'
			'Temp' = Join-Path $buildDir 'temp'
			'Artifacts' = Join-Path $buildDir 'artifacts'
		}
		'TargetHostName' = $TargetHostName
		'EnvironmentName' = $EnvironmentName
		'Region' = $Region
		'Verbosity' = $Verbosity
	}
	
	# create dirs
	if (Test-Path $global:Context.Dir.Temp){
		rd $global:Context.Dir.Temp -Recurse -Force | Out-Null
	}
	md $global:Context.Dir.Temp | Out-Null
	
	if (Test-Path $global:Context.Dir.Artifacts){
		rd $global:Context.Dir.Artifacts -Recurse -Force | Out-Null
	}
	md $global:Context.Dir.Artifacts | Out-Null
}

Task Set-BuildNumber {
	Write-Host "##teamcity[buildNumber '$($global:Context.Version)']"
}

Task Add-AdditionalArtifacts {
	$buildDir = Resolve-Path .
	$howToDeployFileName = Join-Path $buildDir 'how to deploy.txt'
	
	Write-Host "##teamcity[publishArtifacts '$howToDeployFileName']"
}
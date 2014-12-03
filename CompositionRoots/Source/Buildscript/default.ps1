﻿Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

if (Test-Path 'Env:\TEAMCITY_VERSION') {
	FormatTaskName "##teamcity[progressMessage '{0}']"
}

Properties{
	$Revision = $null
	$Build = $null
	$Branch = $null

    $EnvironmentName = 'NOT SET'
}

Include versioning.ps1
Include nuget.ps1
Include migrations.ps1
Include searchmigrations.ps1
Include web.ps1
Include wpf.ps1
Include taskservice.ps1
Include dynamics.ps1
Include reports.ps1
Include unittests.ps1
Include integrationtests.ps1
Include fxcop.ps1

Task Default -depends Hello
Task Hello { "Билдскрипт запущен без цели, укажите цель" }

Task Build-Packages -Depends `
Create-GlobalContext, `
Set-BuildNumber, `
Build-Migrations, `
Build-WebApp, `
Build-BasicOperations, `
Build-TaskService, `
Build-Modi, `
Build-Metadata, `
Build-OrderValidation, `
Build-FinancialOperations, `
Build-Releasing, `
Build-WpfClient, `
Build-Dynamics

Task Deploy-Packages -Depends `
Create-GlobalContext, `
Set-BuildNumber, `
Take-TaskServiceOffline, `
Take-WebAppOffline, `
Deploy-Migrations, `
Deploy-SearchMigrations, `
Deploy-WebApp, `
Deploy-BasicOperations, `
Take-WebAppOnline, `
Deploy-TaskService, `
Take-TaskServiceOnline, `
Deploy-Modi, `
Deploy-Metadata, `
Deploy-OrderValidation, `
Deploy-FinancialOperations, `
Deploy-Releasing, `
Deploy-WpfClient, `
Deploy-Dynamics, `
Deploy-Reports

Task Create-GlobalContext {

	$buildDir = Resolve-Path .

	$global:Context = @{
		'Revision' = $Revision
		'Build' = $Build
		'Branch' = $Branch
		'EnvironmentName' = $EnvironmentName
		'Dir' = @{
			'Solution' = Join-Path $buildDir '..'
			'Temp' = Join-Path $buildDir 'temp'
			'Artifacts' = Join-Path $buildDir 'artifacts'
		}
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
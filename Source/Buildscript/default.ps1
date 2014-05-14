Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

if (Test-Path 'Env:\TEAMCITY_VERSION') {
	FormatTaskName "##teamcity[progressMessage '{0}']"
}

$MajorVersion = 2
$MinorVersion = 1

Properties{
	$Revision = 0
	$Build = 0
	$PackageVersion = $null
	$Branch = ''

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
Build-TaskService, `
Build-Migrations, `
Build-WebApp, `
Build-BasicOperations, `
Build-Modi, `
Build-Metadata, `
Build-OrderValidation, `
Build-FinancialOperations, `
Build-Releasing, `
Build-WpfClient, `
Build-Dynamics, `
Add-AdditionalArtifacts

Task Deploy-Packages -Depends `
Create-GlobalContext, `
Set-BuildNumber, `
Deploy-TaskService, `
Take-WebAppOffline, `
Deploy-Migrations, `
` # Deploy-SearchMigrations, `
Deploy-WebApp, `
Deploy-BasicOperations, `
Take-WebAppOnline, `
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

	$packageVersion = $PackageVersion
	if (![string]::IsNullOrEmpty($packageVersion)){
		$packageVersion += '-{0:0000}' -f $Build
	}

	$global:Context = @{
		'Version' = New-Object System.Version($MajorVersion, $MinorVersion, $Revision, $Build)
		'PackageVersion' = $packageVersion
		'Branch' = $Branch
		'Dir' = @{
			'Solution' = Join-Path $buildDir '..'
			'Temp' = Join-Path $buildDir 'temp'
			'Artifacts' = Join-Path $buildDir 'artifacts'
		}
		'EnvironmentName' = $EnvironmentName
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
	if (![string]::IsNullOrEmpty($global:Context.PackageVersion)){
		$buildNumber = $global:Context.PackageVersion
	}
	else {
		$buildNumber = $global:Context.Version
	}

	if (Test-Path 'Env:\TEAMCITY_VERSION') {
		Write-Host "##teamcity[buildNumber '$buildNumber']"
	}
}

Task Add-AdditionalArtifacts {
	$buildDir = Resolve-Path .
	$howToDeployFileName = Join-Path $buildDir 'how to deploy.txt'
	
	if (Test-Path 'Env:\TEAMCITY_VERSION') {
		Write-Host "##teamcity[publishArtifacts '$howToDeployFileName']"
	}
}
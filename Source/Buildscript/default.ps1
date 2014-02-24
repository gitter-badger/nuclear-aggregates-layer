Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

# имена тасков в формате TeamCity
FormatTaskName "##teamcity[progressMessage '{0}']"

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

Task Default -depends Hello
Task Hello { "Билдскрипт запущен без цели, укажите цель" }

Task Build-Packages -Depends `
Create-GlobalContext, `
Set-BuildNumber, `
Build-TaskService, `
Build-Migrations, `
Build-SearchMigrations, `
Build-Web, `
Build-WpfClient, `
Build-Dynamics, `
Add-AdditionalArtifacts

Task Deploy-Packages -Depends `
Create-GlobalContext, `
Set-BuildNumber, `
Take-WebAppOffline, `
Deploy-TaskService, `
Deploy-Migrations, `
Deploy-SearchMigrations, `
Deploy-Web, `
Take-WebAppOnline, `
Validate-WebApp, `
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
		'Version' = New-Object System.Version(2, 0, $Revision, $Build)
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

	Write-Host "##teamcity[buildNumber '$buildNumber']"
}

Task Add-AdditionalArtifacts {
	$buildDir = Resolve-Path .
	$howToDeployFileName = Join-Path $buildDir 'how to deploy.txt'
	
	Write-Host "##teamcity[publishArtifacts '$howToDeployFileName']"
}
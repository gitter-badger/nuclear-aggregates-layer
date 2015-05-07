Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

if (Test-Path 'Env:\TEAMCITY_VERSION') {
	FormatTaskName "##teamcity[progressMessage '{0}']"
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
Include schemacompare.ps1

Task Default -depends Hello
Task Hello { "Билдскрипт запущен без цели, укажите цель" }

Task Build-Packages -Depends `
Set-BuildNumber, `
Build-Migrations, `
Build-SearchMigrations, `
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
Build-Packages, `
Take-TaskServiceOffline, `
Take-WebAppOffline, `
Deploy-Migrations, `
Deploy-SearchMigrations, `
Deploy-WebApp, `
Deploy-BasicOperations, `
Deploy-TaskService, `
Deploy-Modi, `
Deploy-Metadata, `
Deploy-OrderValidation, `
Deploy-FinancialOperations, `
Deploy-Releasing, `
Deploy-WpfClient, `
Deploy-Dynamics, `
Deploy-Reports
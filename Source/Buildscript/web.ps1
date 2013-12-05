Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module .\modules\msbuild.psm1 -DisableNameChecking
Import-Module .\modules\web.psm1 -DisableNameChecking

Task Build-Web -Depends `
Build-WebApp, `
Build-BasicOperations, `
Build-FinancialOperations, `
Build-Modi, `
Build-Metadata, `
Build-OrderValidation

Task Deploy-Web -Depends `
Deploy-WebApp, `
Deploy-BasicOperations, `
Deploy-FinancialOperations, `
Deploy-Modi, `
Deploy-Metadata, `
Deploy-OrderValidation

Properties{ $OptionWebApp=$false }
Task Build-WebApp -Precondition { return $OptionWebApp } -Depends Update-AssemblyInfo, Build-Migrations {
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.UI.Web.Mvc'
	# for silverlight project we need to build in x86 mode
	Build-WebPackage $projectFileName -MsBuildPlatform 'x86'
}
Task Deploy-WebApp -Precondition { return $OptionWebApp } -Depends Deploy-Migrations, Build-WebApp {
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.UI.Web.Mvc'
	Deploy-WebPackage $projectFileName
	Validate-WebSite $projectFileName
}

Properties{ $OptionBasicOperations=$false }
Task Build-BasicOperations -Precondition { return $OptionBasicOperations } -Depends Update-AssemblyInfo {
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.API.WCF.Operations'
	Build-WebPackage $projectFileName
}
Task Deploy-BasicOperations -Precondition { return $OptionBasicOperations } -Depends Build-BasicOperations {
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.API.WCF.Operations'
	Deploy-WebPackage $projectFileName
	Validate-WebSite $projectFileName 'Delete.svc'
}

Properties{ $OptionModi=$false }
Task Build-Modi -Precondition { return $OptionModi } -Depends Update-AssemblyInfo {
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.API.WCF.MoDi'
	Build-WebPackage $projectFileName
}
Task Deploy-Modi -Precondition { return $OptionModi } -Depends Build-Modi {
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.API.WCF.MoDi'
	Deploy-WebPackage $projectFileName
	Validate-WebSite $projectFileName 'Reports.svc'
}

Properties{ $OptionMetadata=$false }
Task Build-Metadata -Precondition { return $OptionMetadata } -Depends Update-AssemblyInfo {
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.API.WCF.Metadata'
	Build-WebPackage $projectFileName
}
Task Deploy-Metadata -Precondition { return $OptionMetadata } -Depends Build-Metadata {
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.API.WCF.Metadata'
	Deploy-WebPackage $projectFileName
	Validate-WebSite $projectFileName 'Metadata.svc'
}

Properties{ $OptionOrderValidation=$false }
Task Build-OrderValidation -Precondition { return $OptionOrderValidation } -Depends Update-AssemblyInfo {
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.API.WCF.OrderValidation'
	Build-WebPackage $projectFileName
}
Task Deploy-OrderValidation -Precondition { return $OptionOrderValidation } -Depends Build-OrderValidation {
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.API.WCF.OrderValidation'
	Deploy-WebPackage $projectFileName
	Validate-WebSite $projectFileName 'Validate.svc'
}
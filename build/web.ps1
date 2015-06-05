Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module "$BuildToolsRoot\modules\msbuild.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\web.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\metadata.psm1" -DisableNameChecking

Properties { $OptionWebApp = $true }
Task Build-WebApp -Precondition { $OptionWebApp } -Depends Update-AssemblyInfo {
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.UI.Web.Mvc'
	# for silverlight project we need to build in x86 mode
	Build-WebPackage $projectFileName '2Gis.Erm.UI.Web.Mvc' -MsBuildPlatform 'x86'
}
Task Deploy-WebApp -Precondition { $OptionWebApp } -Depends Take-WebAppOffline {
	# don't touch App_offline.htm file if it presented
	Deploy-WebPackage '2Gis.Erm.UI.Web.Mvc' -Argument @(
		"-skip:File=App_offline.htm"
	)
}

Properties { $OptionBasicOperations = $true }
Task Build-BasicOperations -Precondition { $OptionBasicOperations } -Depends Update-AssemblyInfo {
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.API.WCF.Operations'
	Build-WebPackage $projectFileName '2Gis.Erm.API.WCF.Operations'
}
Task Deploy-BasicOperations -Precondition { $OptionBasicOperations } {
	Deploy-WebPackage '2Gis.Erm.API.WCF.Operations'
	Validate-WebSite '2Gis.Erm.API.WCF.Operations' 'Delete.svc'
}

Properties { $OptionModi = $true }
Task Build-Modi -Precondition { $OptionModi -and (Get-Metadata '2Gis.Erm.API.WCF.MoDi').OptionModi } -Depends Update-AssemblyInfo {
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.API.WCF.MoDi'
	Build-WebPackage $projectFileName '2Gis.Erm.API.WCF.MoDi'
}
Task Deploy-Modi -Precondition { $OptionModi -and (Get-Metadata '2Gis.Erm.API.WCF.MoDi').OptionModi } {
	Deploy-WebPackage '2Gis.Erm.API.WCF.MoDi'
	Validate-WebSite '2Gis.Erm.API.WCF.MoDi' 'Reports.svc'
}

Properties { $OptionMetadata = $true }
Task Build-Metadata -Precondition { $OptionMetadata } -Depends Update-AssemblyInfo {
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.API.WCF.Metadata'
	Build-WebPackage $projectFileName '2Gis.Erm.API.WCF.Metadata'
}
Task Deploy-Metadata -Precondition { $OptionMetadata } {
	Deploy-WebPackage '2Gis.Erm.API.WCF.Metadata'
	Validate-WebSite '2Gis.Erm.API.WCF.Metadata' 'Metadata.svc'
}

Properties { $OptionOrderValidation = $true }
Task Build-OrderValidation -Precondition { $OptionOrderValidation } -Depends Update-AssemblyInfo {
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.API.WCF.OrderValidation'
	Build-WebPackage $projectFileName '2Gis.Erm.API.WCF.OrderValidation'
}
Task Deploy-OrderValidation -Precondition { $OptionOrderValidation } {
	Deploy-WebPackage '2Gis.Erm.API.WCF.OrderValidation'
	Validate-WebSite '2Gis.Erm.API.WCF.OrderValidation' 'Validate.svc'
}

Properties { $OptionFinancialOperations = $true }
Task Build-FinancialOperations -Precondition { $OptionFinancialOperations } -Depends Update-AssemblyInfo {
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.API.WCF.Operations.Special'
	Build-WebPackage $projectFileName '2Gis.Erm.API.WCF.Operations.Special'
}
Task Deploy-FinancialOperations -Precondition { $OptionFinancialOperations } {
	Deploy-WebPackage '2Gis.Erm.API.WCF.Operations.Special'
	Validate-WebSite '2Gis.Erm.API.WCF.Operations.Special' 'Calculate.svc'
}

Properties { $OptionReleasing = $true }
Task Build-Releasing -Precondition { $OptionReleasing } -Depends Update-AssemblyInfo {
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.API.WCF.Releasing'
	Build-WebPackage $projectFileName '2Gis.Erm.API.WCF.Releasing'
}
Task Deploy-Releasing -Precondition { $OptionReleasing } {
	Deploy-WebPackage '2Gis.Erm.API.WCF.Releasing'
	Validate-WebSite '2Gis.Erm.API.WCF.Releasing' 'Release.svc'
}

Task Take-WebAppOffline -Precondition { $OptionWebApp -and (Get-Metadata '2Gis.Erm.UI.Web.Mvc').TakeOffline } {
	Take-WebsiteOffline '2Gis.Erm.UI.Web.Mvc'
}
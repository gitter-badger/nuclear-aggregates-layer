Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module "$BuildToolsRoot\modules\msbuild.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\web.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\metadata.psm1" -DisableNameChecking

Properties { $OptionWebApp = $true }
Task Build-WebApp -Precondition { $OptionWebApp } -Depends Update-AssemblyInfo {
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.UI.Web.Mvc'
	$entryPointMetadata = Get-Metadata '2Gis.Erm.UI.Web.Mvc'
	
	# for silverlight project we need to build in x86 mode
	Build-WebPackage $projectFileName $entryPointMetadata -MsBuildPlatform 'x86'
}
Task Deploy-WebApp -Precondition { $OptionWebApp } {
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.UI.Web.Mvc'
	$entryPointMetadata = Get-Metadata '2Gis.Erm.UI.Web.Mvc'
	
	# don't touch App_offline.htm file if it presented
	Deploy-WebPackage $projectFileName $entryPointMetadata -Argument @(
		"-skip:File=App_offline.htm"
	)
}

Properties { $OptionBasicOperations = $true }
Task Build-BasicOperations -Precondition { $OptionBasicOperations } -Depends Update-AssemblyInfo {
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.API.WCF.Operations'
	$entryPointMetadata = Get-Metadata '2Gis.Erm.API.WCF.Operations'
	
	Build-WebPackage $projectFileName $entryPointMetadata
}
Task Deploy-BasicOperations -Precondition { $OptionBasicOperations } {
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.API.WCF.Operations'
	$entryPointMetadata = Get-Metadata '2Gis.Erm.API.WCF.Operations'
	
	Deploy-WebPackage $projectFileName $entryPointMetadata
	Validate-WebSite $entryPointMetadata 'Delete.svc'
}

Properties { $OptionModi = $true }
Task Build-Modi -Precondition { $OptionModi -and (Get-Metadata '2Gis.Erm.API.WCF.MoDi').OptionModi } -Depends Update-AssemblyInfo {
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.API.WCF.MoDi'
	$entryPointMetadata = Get-Metadata '2Gis.Erm.API.WCF.MoDi'
	
	Build-WebPackage $projectFileName $entryPointMetadata
}
Task Deploy-Modi -Precondition { $OptionModi -and (Get-Metadata '2Gis.Erm.API.WCF.MoDi').OptionModi } {
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.API.WCF.MoDi'
	$entryPointMetadata = Get-Metadata '2Gis.Erm.API.WCF.MoDi'
	
	Deploy-WebPackage $projectFileName $entryPointMetadata
	Validate-WebSite $entryPointMetadata 'Reports.svc'
}

Properties { $OptionMetadata = $true }
Task Build-Metadata -Precondition { $OptionMetadata } -Depends Update-AssemblyInfo {
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.API.WCF.Metadata'
	$entryPointMetadata = Get-Metadata '2Gis.Erm.API.WCF.Metadata'
	
	Build-WebPackage $projectFileName $entryPointMetadata
}
Task Deploy-Metadata -Precondition { $OptionMetadata } {
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.API.WCF.Metadata'
	$entryPointMetadata = Get-Metadata '2Gis.Erm.API.WCF.Metadata'
	
	Deploy-WebPackage $projectFileName $entryPointMetadata
	Validate-WebSite $entryPointMetadata 'Metadata.svc'
}

Properties { $OptionOrderValidation = $true }
Task Build-OrderValidation -Precondition { $OptionOrderValidation } -Depends Update-AssemblyInfo {
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.API.WCF.OrderValidation'
	$entryPointMetadata = Get-Metadata '2Gis.Erm.API.WCF.OrderValidation'
	
	Build-WebPackage $projectFileName $entryPointMetadata
}
Task Deploy-OrderValidation -Precondition { $OptionOrderValidation } {
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.API.WCF.OrderValidation'
	$entryPointMetadata = Get-Metadata '2Gis.Erm.API.WCF.OrderValidation'
	
	Deploy-WebPackage $projectFileName $entryPointMetadata
	Validate-WebSite $entryPointMetadata 'Validate.svc'
}

Properties { $OptionFinancialOperations = $true }
Task Build-FinancialOperations -Precondition { $OptionFinancialOperations -and (Get-Metadata '2Gis.Erm.API.WCF.Operations.Special').OptionFinancialOperations } -Depends Update-AssemblyInfo {
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.API.WCF.Operations.Special'
	$entryPointMetadata = Get-Metadata '2Gis.Erm.API.WCF.Operations.Special'
	
	Build-WebPackage $projectFileName $entryPointMetadata
}
Task Deploy-FinancialOperations -Precondition { $OptionFinancialOperations -and (Get-Metadata '2Gis.Erm.API.WCF.Operations.Special').OptionFinancialOperations } {
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.API.WCF.Operations.Special'
	$entryPointMetadata = Get-Metadata '2Gis.Erm.API.WCF.Operations.Special'
	
	Deploy-WebPackage $projectFileName $entryPointMetadata
	Validate-WebSite $entryPointMetadata 'Calculate.svc'
}

Properties { $OptionReleasing = $true }
Task Build-Releasing -Precondition { $OptionReleasing } -Depends Update-AssemblyInfo {
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.API.WCF.Releasing'
	$entryPointMetadata = Get-Metadata '2Gis.Erm.API.WCF.Releasing'
	
	Build-WebPackage $projectFileName $entryPointMetadata
}
Task Deploy-Releasing -Precondition { $OptionReleasing } {
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.API.WCF.Releasing'
	$entryPointMetadata = Get-Metadata '2Gis.Erm.API.WCF.Releasing'
	
	Deploy-WebPackage $projectFileName $entryPointMetadata
	Validate-WebSite $entryPointMetadata 'Release.svc'
}

Task Take-WebAppOffline -Precondition { $OptionWebApp -and (Get-Metadata '2Gis.Erm.UI.Web.Mvc').TakeOffline } {
	$entryPointMetadata = Get-Metadata '2Gis.Erm.UI.Web.Mvc'
	Take-WebsiteOffline $entryPointMetadata.TargetHosts $entryPointMetadata.IisAppPath
}

Task Take-WebAppOnline -Precondition { $OptionWebApp -and (Get-Metadata '2Gis.Erm.UI.Web.Mvc').TakeOffline } {
	$entryPointMetadata = Get-Metadata '2Gis.Erm.UI.Web.Mvc'
	Take-WebsiteOnline $entryPointMetadata.TargetHosts $entryPointMetadata.IisAppPath
	Validate-WebSite $entryPointMetadata
}
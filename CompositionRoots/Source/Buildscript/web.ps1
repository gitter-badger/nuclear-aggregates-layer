Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module "$BuildToolsRoot\modules\msbuild.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\web.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\metadata.psm1" -DisableNameChecking

Properties{ $OptionWebApp=$false }
Task Build-WebApp -Precondition { return $OptionWebApp } -Depends Update-AssemblyInfo {
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.UI.Web.Mvc'
	$entryPointMetadata = Get-EntryPointMetadata '2Gis.Erm.UI.Web.Mvc'
	
	# for silverlight project we need to build in x86 mode
	Build-WebPackage $projectFileName $entryPointMetadata -MsBuildPlatform 'x86'
}
Task Deploy-WebApp -Precondition { return $OptionWebApp } {
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.UI.Web.Mvc'
	$entryPointMetadata = Get-EntryPointMetadata '2Gis.Erm.UI.Web.Mvc'
	
	# don't touch App_offline.htm file if it presented
	Deploy-WebPackage $projectFileName $entryPointMetadata -Argument @(
		"-skip:File=App_offline.htm"
	)
}

Properties{ $OptionBasicOperations=$false }
Task Build-BasicOperations -Precondition { return $OptionBasicOperations } -Depends Update-AssemblyInfo {
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.API.WCF.Operations'
	$entryPointMetadata = Get-EntryPointMetadata '2Gis.Erm.API.WCF.Operations'
	
	Build-WebPackage $projectFileName $entryPointMetadata
}
Task Deploy-BasicOperations -Precondition { return $OptionBasicOperations } {
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.API.WCF.Operations'
	$entryPointMetadata = Get-EntryPointMetadata '2Gis.Erm.API.WCF.Operations'
	
	Deploy-WebPackage $projectFileName $entryPointMetadata
	Validate-WebSite $entryPointMetadata 'Delete.svc'
}

Properties{ $OptionModi=$false }
Task Build-Modi -Precondition { return $OptionModi } -Depends Update-AssemblyInfo {
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.API.WCF.MoDi'
	$entryPointMetadata = Get-EntryPointMetadata '2Gis.Erm.API.WCF.MoDi'
	
	Build-WebPackage $projectFileName $entryPointMetadata
}
Task Deploy-Modi -Precondition { return $OptionModi } {
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.API.WCF.MoDi'
	$entryPointMetadata = Get-EntryPointMetadata '2Gis.Erm.API.WCF.MoDi'
	
	Deploy-WebPackage $projectFileName $entryPointMetadata
	Validate-WebSite $entryPointMetadata 'Reports.svc'
}

Properties{ $OptionMetadata=$false }
Task Build-Metadata -Precondition { return $OptionMetadata } -Depends Update-AssemblyInfo {
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.API.WCF.Metadata'
	$entryPointMetadata = Get-EntryPointMetadata '2Gis.Erm.API.WCF.Metadata'
	
	Build-WebPackage $projectFileName $entryPointMetadata
}
Task Deploy-Metadata -Precondition { return $OptionMetadata } {
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.API.WCF.Metadata'
	$entryPointMetadata = Get-EntryPointMetadata '2Gis.Erm.API.WCF.Metadata'
	
	Deploy-WebPackage $projectFileName $entryPointMetadata
	Validate-WebSite $entryPointMetadata 'Metadata.svc'
}

Properties{ $OptionOrderValidation=$false }
Task Build-OrderValidation -Precondition { return $OptionOrderValidation } -Depends Update-AssemblyInfo {
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.API.WCF.OrderValidation'
	$entryPointMetadata = Get-EntryPointMetadata '2Gis.Erm.API.WCF.OrderValidation'
	
	Build-WebPackage $projectFileName $entryPointMetadata
}
Task Deploy-OrderValidation -Precondition { return $OptionOrderValidation } {
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.API.WCF.OrderValidation'
	$entryPointMetadata = Get-EntryPointMetadata '2Gis.Erm.API.WCF.OrderValidation'
	
	Deploy-WebPackage $projectFileName $entryPointMetadata
	Validate-WebSite $entryPointMetadata 'Validate.svc'
}

Properties{ $OptionFinancialOperations=$false }
Task Build-FinancialOperations -Precondition { return $OptionFinancialOperations } -Depends Update-AssemblyInfo {
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.API.WCF.Operations.Special'
	$entryPointMetadata = Get-EntryPointMetadata '2Gis.Erm.API.WCF.Operations.Special'
	
	Build-WebPackage $projectFileName $entryPointMetadata
}
Task Deploy-FinancialOperations -Precondition { return $OptionFinancialOperations } {
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.API.WCF.Operations.Special'
	$entryPointMetadata = Get-EntryPointMetadata '2Gis.Erm.API.WCF.Operations.Special'
	
	Deploy-WebPackage $projectFileName $entryPointMetadata
	Validate-WebSite $entryPointMetadata 'Calculate.svc'
}

Properties{ $OptionReleasing=$false }
Task Build-Releasing -Precondition { return $OptionReleasing } -Depends Update-AssemblyInfo {
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.API.WCF.Releasing'
	$entryPointMetadata = Get-EntryPointMetadata '2Gis.Erm.API.WCF.Releasing'
	
	Build-WebPackage $projectFileName $entryPointMetadata
}
Task Deploy-Releasing -Precondition { return $OptionReleasing } {
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.API.WCF.Releasing'
	$entryPointMetadata = Get-EntryPointMetadata '2Gis.Erm.API.WCF.Releasing'
	
	Deploy-WebPackage $projectFileName $entryPointMetadata
	Validate-WebSite $entryPointMetadata 'Release.svc'
}

Task Take-WebAppOffline -Precondition { return $OptionWebApp -and (Get-EntryPointMetadata '2Gis.Erm.UI.Web.Mvc').TakeOffline } {
	$entryPointMetadata = Get-EntryPointMetadata '2Gis.Erm.UI.Web.Mvc'
	Take-WebsiteOffline $entryPointMetadata.TargetHosts $entryPointMetadata.IisAppPath
}

Task Take-WebAppOnline -Precondition { return $OptionWebApp -and (Get-EntryPointMetadata '2Gis.Erm.UI.Web.Mvc').TakeOffline } {
	$entryPointMetadata = Get-EntryPointMetadata '2Gis.Erm.UI.Web.Mvc'
	Take-WebsiteOnline $entryPointMetadata.TargetHosts $entryPointMetadata.IisAppPath
	Validate-WebSite $entryPointMetadata
}
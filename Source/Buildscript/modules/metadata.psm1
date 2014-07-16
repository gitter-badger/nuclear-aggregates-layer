Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

$CountryShortName = @{
	'Russia' = 'RU'
	'Cyprus' = 'CY'
	'Czech' = 'CZ'
	'Chile' = 'CL'
	'Ukraine' = 'UA'
	'Emirates' = 'AE'
}

function Get-WebProjectTargetHosts ($EnvType, $Country, $EntryPoint, $Index, $WithCrm){

	switch ($EnvType) {
		'Test' {
			switch ($Country) {
				'Russia' {
					if ($WithCrm) {
						return @{ 'TargetHosts' = @("uk-erm-test$Index") }
					}
				}
			}

			return @{ 'TargetHosts' = @('uk-erm-test07') }
		}
		'Edu' {
			return @{ 'TargetHosts' = @('uk-erm-edu01') }
		}
		'Production' {
			switch ($Country) {
				'Russia' {
					switch ($EntryPoint){
						'2Gis.Erm.API.WCF.Releasing' {
							return @{ 'TargetHosts' = @('uk-crm01') }
						}
					}

					return @{ 'TargetHosts' = @('uk-crm01', 'uk-crm02', 'uk-crm03') }
				}
				default {
					switch ($EntryPoint){
						'2Gis.Erm.TaskService.Installer' {
							return @{ 'TargetHosts' = @('uk-erm-iis01', 'uk-erm-iis02', 'uk-erm-iis03') }
						}
					}

					return @{ 'TargetHosts' = @('uk-erm-iis03') }
				}
			}
		}
		'Int' {
			switch ($Country) {
				'Russia' {
					return @{ 'TargetHosts' = @('uk-test-int02') }
				}
				default {
					return @{ 'TargetHosts' = @('uk-test-int01') }
				}
			}
		}
		'Load' {
			return @{ 'TargetHosts' = @('uk-erm-load03', 'uk-erm-load04') }
		}
	}
}

function Get-WebProjectIisAppPath ($EnvType, $Country, $EntryPoint) {

	switch ($EntryPoint) {
		'2Gis.Erm.UI.Web.Mvc' { $Prefix = "web-app$Index" }
		'2Gis.Erm.API.WCF.Operations' { $Prefix = "basic-operations$Index.api" }
		'2Gis.Erm.API.WCF.MoDi' { $Prefix = "money-distribution$Index.api" }
		'2Gis.Erm.API.WCF.Metadata' { $Prefix = "metadata$Index.api" }
		'2Gis.Erm.API.WCF.OrderValidation' { $Prefix = "order-validation$Index.api" }
		'2Gis.Erm.API.WCF.Operations.Special' { $Prefix = "financial-operations$Index.api" }
		'2Gis.Erm.API.WCF.Releasing' { $Prefix = "releasing$Index.api" }
		'2Gis.Erm.UI.Desktop.WPF' { $Prefix = "wpf-app$Index" }
	}

	$envTypeLower = $EnvType.ToLowerInvariant()
	$countryLower = $Country.ToLowerInvariant()

	switch ($EnvType) {
		'Production' {
			switch ($Country) {
				'Russia' {
					return @{ 'IisAppPath' = "$Prefix.prod.erm.2gis.ru" }
				}
				default {
					return @{ 'IisAppPath' = "$Prefix.prod.erm.$countryLower" }
				}
			}
		}
		default {
			switch ($Country) {
				default {
					return @{ 'IisAppPath' = "$Prefix.$envTypeLower.erm.$countryLower" }
				}
			}
		}
	}
}

function Get-WebProjectMetadata ($EnvType, $Country, $EntryPoint, $Index, $WithCrm) {

	$targetHosts = Get-WebProjectTargetHosts $EnvType $Country $EntryPoint $Index $WithCrm
	$iisAppPath = Get-WebProjectIisAppPath $EnvType $Country $EntryPoint
	return $targetHosts + $iisAppPath
}

function Get-ReportsMetadata ($EnvType, $Country, $Index){
	switch ($EnvType){
		'Test' {
			return @{
				'ServerUrls' = @('http://uk-sql01/ReportServer')
				'ReportsFolder' = "/Test.$Index"
				'Region' = $CountryShortName[$Country]
			}
		}
		'Edu' {
			return @{
				'ServerUrls' = @('http://uk-erm-edu01/ReportServer')
				'ReportsFolder' = "/$EnvType.$Country"
				'Region' = $CountryShortName[$Country]
			}
		}
		'Production' {
			switch ($Country) {
				'Russia' {
					return @{
						'ServerUrls' = @('http://uk-rpt/ReportServer')
						'ReportsFolder' = '/MSCRM'
						'Region' = $CountryShortName[$Country]
					}
				}
				default {
					return @{
						'ServerUrls' = @('http://uk-rpt/ReportServer')
						'ReportsFolder' = '/ERM_ENG'
						'Region' = $CountryShortName[$Country]
					}
				}
			}
		}
		'Int' {
			return @{
				'ServerUrls' = @('http://uk-test-int03/ReportServer')
				'ReportsFolder' = "/$EnvType.$Country"
				'Region' = $CountryShortName[$Country]
			}
		}
		'Load' {
			return @{
				'ServerUrls' = @('http://uk-test-sql01/ReportServer')
				'ReportsFolder' = "/$EnvType.$Country"
				'Region' = $CountryShortName[$Country]
			}
		}
	}
}

function Get-ConfigTransformMetadata ($EnvType, $Country, $Index, $WithCrm){
	
	$xdt = @(
		'Common\log4net.Release.config'
		'Common\Erm.Release.config'
	)
	$regex = @{}

	switch ($EnvType){
		'Test' {
			switch ($Country){
				'Russia' {
					if ($WithCrm){
						return @{
							'Xdt' = $xdt + @('Templates\Erm.Test.Russia.CRM.config')
							'Regex' = $regex + @{ '{EnvNum}' = "$Index" }
						}
					} else {
						return @{
							'Xdt' = $xdt + @("Templates\Erm.Test.$Country.config")
							'Regex' = $regex + @{ '{EnvNum}' = "$Index" }
						}
					}
				}
				default {
					return @{
						'Xdt' = $xdt + @("Templates\Erm.Test.$Country.config")
						'Regex' = $regex + @{ '{EnvNum}' = "$Index" }
					}
				}
			}
		}
		default {
			return @{
				'Xdt' = $xdt
				'Regex' = $regex
			}
		}
	}
}

function Get-TaskServiceAutoStartMetadata ($EnvType){
	switch ($EnvType) {
		{ @('Production', 'Load') -contains $_ } { return @{ 'AutoStart' = $false} }
		default { return @{ 'AutoStart' = $true} }
	}
}

function Get-TaskServiceMetadata ($EnvType, $Country, $Index, $WithCrm) {
	$targetHosts = Get-WebProjectTargetHosts $EnvType $Country '2Gis.Erm.TaskService.Installer' $Index $WithCrm
	$autoStart = Get-TaskServiceAutoStartMetadata $EnvType
	return $targetHosts + $autoStart
}

function Get-EntryPointMetadata ($EnvType, $Country, $Index, $WithCrm) {
	return @{
		'ConfigTransform' = Get-ConfigTransformMetadata $EnvType $Country $Index $WithCrm
		'2Gis.Erm.UI.Web.Mvc' = Get-WebProjectMetadata $EnvType $Country '2Gis.Erm.UI.Web.Mvc' $Index $WithCrm
		'2Gis.Erm.API.WCF.Operations' = Get-WebProjectMetadata $EnvType $Country '2Gis.Erm.API.WCF.Operations' $Index $WithCrm
		'2Gis.Erm.API.WCF.MoDi' = Get-WebProjectMetadata $EnvType $Country '2Gis.Erm.API.WCF.MoDi' $Index $WithCrm
		'2Gis.Erm.API.WCF.Metadata' = Get-WebProjectMetadata $EnvType $Country '2Gis.Erm.API.WCF.Metadata' $Index $WithCrm
		'2Gis.Erm.API.WCF.OrderValidation' = Get-WebProjectMetadata $EnvType $Country '2Gis.Erm.API.WCF.OrderValidation' $Index $WithCrm
		'2Gis.Erm.API.WCF.Operations.Special' = Get-WebProjectMetadata $EnvType $Country '2Gis.Erm.API.WCF.Operations.Special' $Index $WithCrm
		'2Gis.Erm.API.WCF.Releasing' = Get-WebProjectMetadata $EnvType $Country '2Gis.Erm.API.WCF.Releasing' $Index $WithCrm
		'2Gis.Erm.UI.Desktop.WPF' = Get-WebProjectMetadata $EnvType $Country '2Gis.Erm.UI.Desktop.WPF' $Index $WithCrm
		'2Gis.Erm.TaskService.Installer' = Get-TaskServiceMetadata $EnvType $Country $Index $WithCrm
		'ErmReports' = Get-ReportsMetadata $EnvType $Country $Index
	}
}

$AllMetadata = @{

	'Edu.Chile' = Get-EntryPointMetadata 'Edu' 'Chile'
	'Edu.Cyprus' = Get-EntryPointMetadata 'Edu' 'Cyprus'
	'Edu.Emirates' = Get-EntryPointMetadata 'Edu' 'Emirates'
	'Edu.Czech' = Get-EntryPointMetadata 'Edu' 'Czech'
	'Edu.Ukraine' = Get-EntryPointMetadata 'Edu' 'Ukraine'
	'Edu.Russia' = Get-EntryPointMetadata 'Edu' 'Russia'

	'Int.Chile' = Get-EntryPointMetadata 'Edu' 'Chile'
	'Int.Cyprus' = Get-EntryPointMetadata 'Edu' 'Cyprus'
	'Int.Czech' = Get-EntryPointMetadata 'Edu' 'Czech'
	'Int.Russia' = Get-EntryPointMetadata 'Edu' 'Russia'
	
	'Load.Russia' = Get-EntryPointMetadata 'Load' 'Russia'

	'Production.Chile' = Get-EntryPointMetadata 'Production' 'Chile'
	'Production.Cyprus' = Get-EntryPointMetadata 'Production' 'Cyprus'
	'Production.Emirates' = Get-EntryPointMetadata 'Production' 'Emirates'
	'Production.Czech' = Get-EntryPointMetadata 'Production' 'Czech'
	'Production.Ukraine' = Get-EntryPointMetadata 'Production' 'Ukraine'
	'Production.Russia' = Get-EntryPointMetadata 'Production' 'Russia'

	'Test.01' = Get-EntryPointMetadata 'Test' 'Russia' '01' $true
	'Test.02' = Get-EntryPointMetadata 'Test' 'Russia' '02' $true
	'Test.03' = Get-EntryPointMetadata 'Test' 'Russia' '03' $true
	'Test.04' = Get-EntryPointMetadata 'Test' 'Russia' '04' $true
	'Test.05' = Get-EntryPointMetadata 'Test' 'Russia' '05' $true
	'Test.06' = Get-EntryPointMetadata 'Test' 'Russia' '06' $true

	'Test.07' = Get-EntryPointMetadata 'Test' 'Russia' '07'
	'Test.08' = Get-EntryPointMetadata 'Test' 'Russia' '08'
	'Test.09' = Get-EntryPointMetadata 'Test' 'Russia' '09'
	'Test.10' = Get-EntryPointMetadata 'Test' 'Russia' '10'
	'Test.11' = Get-EntryPointMetadata 'Test' 'Russia' '11'
	'Test.12' = Get-EntryPointMetadata 'Test' 'Russia' '12'
	'Test.13' = Get-EntryPointMetadata 'Test' 'Russia' '13'
	'Test.14' = Get-EntryPointMetadata 'Test' 'Russia' '14'
	'Test.15' = Get-EntryPointMetadata 'Test' 'Russia' '15'
	'Test.16' = Get-EntryPointMetadata 'Test' 'Russia' '16'
	'Test.17' = Get-EntryPointMetadata 'Test' 'Russia' '17'
	'Test.18' = Get-EntryPointMetadata 'Test' 'Russia' '18'
	'Test.19' = Get-EntryPointMetadata 'Test' 'Russia' '19'
	'Test.20' = Get-EntryPointMetadata 'Test' 'Russia' '20'
	'Test.21' = Get-EntryPointMetadata 'Test' 'Russia' '21'
	'Test.22' = Get-EntryPointMetadata 'Test' 'Russia' '22'
	'Test.23' = Get-EntryPointMetadata 'Test' 'Russia' '23'
	'Test.24' = Get-EntryPointMetadata 'Test' 'Russia' '24'
	'Test.25' = Get-EntryPointMetadata 'Test' 'Russia' '25'
	
	'Test.101' = Get-EntryPointMetadata 'Test' 'Cyprus' '101'
	'Test.102' = Get-EntryPointMetadata 'Test' 'Cyprus' '102'
	'Test.108' = Get-EntryPointMetadata 'Test' 'Cyprus' '108'

	'Test.201' = Get-EntryPointMetadata 'Test' 'Czech' '201'
	'Test.202' = Get-EntryPointMetadata 'Test' 'Czech' '202'
	'Test.208' = Get-EntryPointMetadata 'Test' 'Czech' '208'

	'Test.301' = Get-EntryPointMetadata 'Test' 'Chile' '301'
	'Test.302' = Get-EntryPointMetadata 'Test' 'Chile' '302'
	'Test.308' = Get-EntryPointMetadata 'Test' 'Chile' '308'
	'Test.320' = Get-EntryPointMetadata 'Test' 'Chile' '320'
	
	'Test.401' = Get-EntryPointMetadata 'Test' 'Ukraine' '401'
	'Test.402' = Get-EntryPointMetadata 'Test' 'Ukraine' '402'
	'Test.408' = Get-EntryPointMetadata 'Test' 'Ukraine' '408'

	'Test.501' = Get-EntryPointMetadata 'Test' 'Emirates' '501'
	'Test.502' = Get-EntryPointMetadata 'Test' 'Emirates' '502'
	'Test.503' = Get-EntryPointMetadata 'Test' 'Emirates' '503'
	'Test.508' = Get-EntryPointMetadata 'Test' 'Emirates' '508'
}

function Get-EntryPointMetadata([ValidateSet(
'ConfigTransform',
'2Gis.Erm.UI.Web.Mvc',
'2Gis.Erm.API.WCF.Operations',
'2Gis.Erm.API.WCF.MoDi',
'2Gis.Erm.API.WCF.Metadata',
'2Gis.Erm.API.WCF.OrderValidation',
'2Gis.Erm.API.WCF.Operations.Special',
'2Gis.Erm.API.WCF.Releasing',
'2Gis.Erm.TaskService.Installer',
'2Gis.Erm.UI.Desktop.WPF',
'ErmReports'
)]$EntryPoint){

	$environmentName = $global:Context.EnvironmentName

	if (!$AllMetadata.ContainsKey($environmentName)){
		throw "Can't find metadata for environment '$environmentName'!"
	}
	$environmentMetadata = $AllMetadata[$environmentName]
	
	if (!$environmentMetadata.ContainsKey($EntryPoint)){
		throw "Can't find metadata for entry point '$EntryPoint' in environment '$environmentName'!"
	}
	$entryPointMetadata = $environmentMetadata[$EntryPoint]
	
	return $entryPointMetadata
}

Export-ModuleMember -Function Get-EntryPointMetadata
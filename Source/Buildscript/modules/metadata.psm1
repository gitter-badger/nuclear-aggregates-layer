Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

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
			}
		}
		'Edu' {
			return @{
				'ServerUrls' = @('http://uk-erm-edu01/ReportServer')
				'ReportsFolder' = "/$EnvType.$Country"
			}
		}
		'Production' {
			switch ($Country) {
				'Russia' {
					return @{
						'ServerUrls' = @('http://uk-rpt/ReportServer')
						'ReportsFolder' = '/MSCRM'
					}
				}
				default {
					return @{
						'ServerUrls' = @('http://uk-rpt/ReportServer')
						'ReportsFolder' = '/ERM_ENG'
					}
				}
			}
		}
		'Int' {
			return @{
				'ServerUrls' = @('http://uk-test-int03/ReportServer')
				'ReportsFolder' = "/$EnvType.$Country"
			}
		}
		'Load' {
			return @{
				'ServerUrls' = @('http://uk-test-sql01/ReportServer')
				'ReportsFolder' = "/$EnvType.$Country"
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

function Get-TaskServiceQuartzConfigMetadata ($EnvType, $Country, $WithCrm){

	switch ($EnvType){
		{ @('Test', 'Int', 'Edu', 'Load') -contains $_ } {
			switch ($Country){
				'Russia' {
					$quartzConfigs = @('Templates\quartz.Test.Russia.config')
					if ($WithCrm){
						$quartzConfigs += @('Templates\quartz.Test.Russia.CRM.config')
					}
				}
				default {
					$quartzConfigs = @('Templates\quartz.Test.MultiCulture.config')
				}
			}
		}
		'Production' {
			switch ($Country){
				'Russia' {
					$quartzConfigs = @('quartz.Production.Russia.config')
				}
				default {
					$quartzConfigs = @('quartz.Production.MultiCulture.config')
				}
			}
		}
		default {
			switch ($Country){
				'Russia' {
					$quartzConfigs = @("quartz.$EnvType.Russia.config")
				}
				default {
					$quartzConfigs = @("quartz.$EnvType.MultiCulture.config")
				}
			}
		}
	}

	return @{ 'QuartzConfigs' =  $quartzConfigs }
}

function Get-TaskServiceMetadata ($Guid, $EnvType, $Country, $Index, $WithCrm) {
	$targetHosts = Get-WebProjectTargetHosts $EnvType $Country '2Gis.Erm.TaskService.Installer' $Index $WithCrm
	$autoStart = Get-TaskServiceAutoStartMetadata $EnvType
	$quartzConfigs = Get-TaskServiceQuartzConfigMetadata $EnvType $Country $WithCrm
	
	return $targetHosts + $autoStart + $quartzConfigs + @{
		'UpgradeCode' = $Guid
	}
}

function Get-MigrationsMetadata ($Country, $WithCrm){
	switch ($Country){
		'Russia' {
			if ($WithCrm) {
				return @{ 'RunElasticsearchMigrations' = $true }
			} else {
				return @{ 'RunElasticsearchMigrations' = $false }
			}
		}
		default {return @{ 'RunElasticsearchMigrations' = $false }}
	}
}

function Get-EntryPointMetadata ($Guid, $EnvType, $Country, $Index, $WithCrm) {
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
		'2Gis.Erm.TaskService.Installer' = Get-TaskServiceMetadata $Guid $EnvType $Country $Index $WithCrm
		'ErmReports' = Get-ReportsMetadata $EnvType $Country $Index
		'Migrations' = Get-MigrationsMetadata $Country $WithCrm
	}
}

$AllMetadata = @{

	'Edu.Chile' = Get-EntryPointMetadata 'CF135E98-2D85-4BC2-8FDE-011BB7F6CF6A' 'Edu' 'Chile'
	'Edu.Cyprus' = Get-EntryPointMetadata '33B02E9D-2860-452F-A2D6-049577715930' 'Edu' 'Cyprus'
	'Edu.Czech' = Get-EntryPointMetadata 'A3548458-DCEF-58D5-B4B5-2D37232525D1' 'Edu' 'Czech'
	'Edu.Emirates' = Get-EntryPointMetadata '33B09E1D-3600-452F-A2D6-089577715930' 'Edu' 'Emirates'
	'Edu.Russia' = Get-EntryPointMetadata 'CAE92B57-6FF7-4257-82C9-8C42AE17C106' 'Edu' 'Russia' $null $true
	'Edu.Ukraine' = Get-EntryPointMetadata 'B58DF457-52D6-47B9-9F0F-D5C8C8BD2494' 'Edu' 'Ukraine'

	'Int.Chile' = Get-EntryPointMetadata 'B049AF18-7D2C-4A82-A0F4-6DD1BCBBD619' 'Int' 'Chile'
	'Int.Cyprus' = Get-EntryPointMetadata 'B7F0FD87-84D0-48F6-AF39-725BF6DEBB8D' 'Int' 'Cyprus'
	'Int.Czech' = Get-EntryPointMetadata 'A3548458-DCEF-47D7-B4B5-2D37232525D1' 'Int' 'Czech'
	'Int.Russia' = Get-EntryPointMetadata 'CAE92B67-6FF7-4257-82E9-8C42AE17C106' 'Int' 'Russia' $null $true
	
	'Load.Russia' = Get-EntryPointMetadata '7563b08b-ea51-4115-b894-43a136af8801' 'Load' 'Russia' $null $true

	'Production.Chile' = Get-EntryPointMetadata 'BB622DAF-9686-4428-9061-42B1F9B643D0' 'Production' 'Chile'
	'Production.Cyprus' = Get-EntryPointMetadata 'AF99F81A-715D-4687-A170-52D7E88B4DDF' 'Production' 'Cyprus'
	'Production.Czech' = Get-EntryPointMetadata 'A3548845-DCEF-49D5-B4B5-2D37238325D1' 'Production' 'Czech'
	'Production.Emirates' = Get-EntryPointMetadata 'AF99F81A-780D-4687-A170-52D7E88B4DDF' 'Production' 'Emirates'
	'Production.Russia' = Get-EntryPointMetadata '7563b07b-ea51-4115-b894-43a136af8801' 'Production' 'Russia' $null $true
	'Production.Ukraine' = Get-EntryPointMetadata '3CC2E766-5DA5-4459-9C8A-0BC5138CFAE4' 'Production' 'Ukraine'

	'Test.01' = Get-EntryPointMetadata '06CEDA4F-722D-4D1E-A3F9-C36A8DDA026F' 'Test' 'Russia' '01' $true
	'Test.02' = Get-EntryPointMetadata '0CEBBB45-C539-4846-875E-6552B5FE937D' 'Test' 'Russia' '02' $true
	'Test.03' = Get-EntryPointMetadata '93C848C6-F228-4F89-BCD9-D4E2585F15C2' 'Test' 'Russia' '03' $true
	'Test.04' = Get-EntryPointMetadata '5862AB78-F9CC-4992-BC80-B032EE541396' 'Test' 'Russia' '04' $true
	'Test.05' = Get-EntryPointMetadata '5C24AB83-0BF3-4DC5-9A44-1CCF7313A583' 'Test' 'Russia' '05' $true
	'Test.06' = Get-EntryPointMetadata 'CC6794C1-F798-4532-AE1E-23D2B9CC282D' 'Test' 'Russia' '06' $true

	'Test.07' = Get-EntryPointMetadata '6DABC3D9-6DE4-4CA6-89F0-357FEE60C16F' 'Test' 'Russia' '07'
	'Test.08' = Get-EntryPointMetadata '3BF90797-6F3D-4181-834A-882211A87294' 'Test' 'Russia' '08'
	'Test.09' = Get-EntryPointMetadata '2C412D6C-7E70-4F08-B0B2-5D643E1FC3D1' 'Test' 'Russia' '09'
	'Test.10' = Get-EntryPointMetadata '2C412D6C-1E70-4F08-B0B7-5D643E1FC3D1' 'Test' 'Russia' '10'
	'Test.11' = Get-EntryPointMetadata '0D424C6C-3875-4F6C-82BC-50F769E1F72A' 'Test' 'Russia' '11'
	'Test.12' = Get-EntryPointMetadata 'C0DEDD27-B2F4-4EE0-98F8-0AFA56533FF6' 'Test' 'Russia' '12'
	'Test.13' = Get-EntryPointMetadata 'C6ABE531-8B67-4AAF-82A1-B46A8911B21A' 'Test' 'Russia' '13'
	'Test.14' = Get-EntryPointMetadata 'A3548458-BCEF-49D5-B4B0-2D37238325D7' 'Test' 'Russia' '14'
	'Test.15' = Get-EntryPointMetadata 'A3548458-DCEF-50D5-B4B5-2D37238325D7' 'Test' 'Russia' '15'
	'Test.16' = Get-EntryPointMetadata 'A3548458-DCCF-49D5-B4B5-2D37238325D7' 'Test' 'Russia' '16'
	'Test.17' = Get-EntryPointMetadata 'A3548458-DCEF-49D5-B4B2-2D37238325D7' 'Test' 'Russia' '17'
	'Test.18' = Get-EntryPointMetadata 'A3548458-DCEF-49D5-B9B5-2D37238325D7' 'Test' 'Russia' '18'
	'Test.19' = Get-EntryPointMetadata 'A3548458-DCEF-49D5-B4B5-2D97238325D7' 'Test' 'Russia' '19'
	'Test.20' = Get-EntryPointMetadata 'A3548458-DCEF-49D5-B4B5-2D37238325D1' 'Test' 'Russia' '20'
	'Test.21' = Get-EntryPointMetadata 'A3548458-BCEF-91D5-B4B0-2D37238325D7' 'Test' 'Russia' '21'
	'Test.22' = Get-EntryPointMetadata '59B49313-00DE-4BFB-8662-4D8F311B7D4D' 'Test' 'Russia' '22'
	'Test.23' = Get-EntryPointMetadata 'A3548458-DCEF-49D5-B4B5-2D37232525D1' 'Test' 'Russia' '23'
	'Test.24' = Get-EntryPointMetadata 'DCE72E82-64A6-4B16-8764-EF883CF6A83E' 'Test' 'Russia' '24'
	'Test.25' = Get-EntryPointMetadata '10CAF954-6D6D-4B5A-84B9-A272F3E17F5B' 'Test' 'Russia' '25'
	
	'Test.101' = Get-EntryPointMetadata '59F62505-F286-4681-A4CB-A74024F754C8' 'Test' 'Cyprus' '101'
	'Test.102' = Get-EntryPointMetadata '0DF48E0B-48B4-4131-A0DB-CCCD68FDB5D0' 'Test' 'Cyprus' '102'
	'Test.108' = Get-EntryPointMetadata 'E9DC1036-4647-4439-ABEF-F40BA7D9B241' 'Test' 'Cyprus' '108'

	'Test.201' = Get-EntryPointMetadata '082BA5D7-AE8E-43E1-B172-1BFD865AD89E' 'Test' 'Czech' '201'
	'Test.202' = Get-EntryPointMetadata '919D03F3-2114-4B86-B1E0-840691262B8B' 'Test' 'Czech' '202'
	'Test.208' = Get-EntryPointMetadata 'B4A8B900-BBCA-4449-8920-572C40933FF7' 'Test' 'Czech' '208'

	'Test.301' = Get-EntryPointMetadata '01FC3A5A-50BE-43CF-B993-86AF9EBD8678' 'Test' 'Chile' '301'
	'Test.302' = Get-EntryPointMetadata '1C029286-1862-4CCE-982A-8D0C69519B59' 'Test' 'Chile' '302'
	'Test.308' = Get-EntryPointMetadata 'D0E15669-68CF-4E52-8E7F-AAC560FED7A9' 'Test' 'Chile' '308'
	'Test.320' = Get-EntryPointMetadata '0EE40877-4B9A-4A4D-A77C-650C0586F52B' 'Test' 'Chile' '320'
	
	'Test.401' = Get-EntryPointMetadata '87BB7A3D-1D98-4202-9583-2C37BFAD8725' 'Test' 'Ukraine' '401'
	'Test.402' = Get-EntryPointMetadata '71D6E609-2139-43DB-8B10-2DE10E65A86A' 'Test' 'Ukraine' '402'
	'Test.408' = Get-EntryPointMetadata '8D79DA63-8A78-4273-9A76-749E97A1EA89' 'Test' 'Ukraine' '408'

	'Test.501' = Get-EntryPointMetadata 'E1D4AF62-5442-4BBA-AB62-A3B30663B7AD' 'Test' 'Emirates' '501'
	'Test.502' = Get-EntryPointMetadata '9A3555B7-7CF9-4C05-9859-8D36FC83EBF9' 'Test' 'Emirates' '502'
	'Test.503' = Get-EntryPointMetadata '3F7EBD0E-6E9C-418D-ADD9-B6C5CF5C414A' 'Test' 'Emirates' '503'
	'Test.508' = Get-EntryPointMetadata '44B17827-9F9D-49F9-8471-E07E7002BCF0' 'Test' 'Emirates' '508'
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
'ErmReports',
'Migrations'
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
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

function FromTemplate-Test.Russia.CRM($Index){
	return @{
		'ConfigTransform' = @{
			'Xdt' = @('Erm.Test.Russia.CRM.config')
			'Regex' = @{ '{EnvNum}' = "$Index" }
		}
		'2Gis.Erm.UI.Web.Mvc' = @{
			'IisAppPath' = "web-app$Index.test.erm.russia"
			'TargetHosts' = @("uk-erm-test$Index")
		}
		'2Gis.Erm.API.WCF.Operations' = @{
			'IisAppPath' = "basic-operations$Index.api.test.erm.russia"
			'TargetHosts' = @("uk-erm-test$Index")
		}
		'2Gis.Erm.API.WCF.MoDi' = @{
			'IisAppPath' = "money-distribution$Index.api.test.erm.russia"
			'TargetHosts' = @("uk-erm-test$Index")
		}
		'2Gis.Erm.API.WCF.Metadata' = @{
			'IisAppPath' = "metadata$Index.api.test.erm.russia"
			'TargetHosts' = @("uk-erm-test$Index")
		}
		'2Gis.Erm.API.WCF.OrderValidation' = @{
			'IisAppPath' = "order-validation$Index.api.test.erm.russia"
			'TargetHosts' = @("uk-erm-test$Index")
		}
		'2Gis.Erm.API.WCF.Operations.Special' = @{
			'IisAppPath' = "financial-operations$Index.api.test.erm.russia"
			'TargetHosts' = @("uk-erm-test$Index")
		}
		'2Gis.Erm.API.WCF.Releasing' = @{
			'IisAppPath' = "releasing$Index.api.test.erm.russia"
			'TargetHosts' = @("uk-erm-test$Index")
		}
		'2Gis.Erm.TaskService.Installer' = @{
			'TargetHosts' = @("uk-erm-test$Index")
		}
		'ErmReports' = @{
			'ServerUrls' = @('http://uk-sql01/ReportServer')
			'ReportsFolder' = "/Test.$Index"
			'Region' = 'RU'
		}
	}	
}

function FromTemplate-Test.Russia($Index){
	
	$template = FromTemplate-Test 'Russia' $Index

	return $template + @{
		'2Gis.Erm.API.WCF.MoDi' = @{
			'IisAppPath' = "money-distribution$Index.api.test.erm.russia"
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Operations.Special' = @{
			'IisAppPath' = "financial-operations$Index.api.test.erm.russia"
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.UI.Desktop.WPF' = @{
			'IisAppPath' = "wpf-app$Index.test.erm.russia"
			'TargetHosts' = @('uk-erm-test07')
		}
	}
}

function FromTemplate-Test($Country, $Index){

	$countryLower = $Country.ToLowerInvariant()
	
	return @{
		'ConfigTransform' = @{
			'Xdt' = @("Erm.Test.$Country.config")
			'Regex' = @{ '{EnvNum}' = "$Index" }
		}
		'2Gis.Erm.UI.Web.Mvc' = @{
			'IisAppPath' = "web-app$Index.test.erm.$countryLower"
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Operations' = @{
			'IisAppPath' = "basic-operations$Index.api.test.erm.$countryLower"
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Metadata' = @{
			'IisAppPath' = "metadata$Index.api.test.erm.$countryLower"
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.OrderValidation' = @{
			'IisAppPath' = "order-validation$Index.api.test.erm.$countryLower"
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Releasing' = @{
			'IisAppPath' = "releasing$Index.api.test.erm.$countryLower"
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.TaskService.Installer' = @{
			'TargetHosts' = @('uk-erm-test07')
		}
		'ErmReports' = @{
			'ServerUrls' = @('http://uk-sql01/ReportServer')
			'ReportsFolder' = "/Test.$Index"
			'Region' = $CountryShortName[$Country]
		}
	}
}

function FromTemplate-Edu($Country){

	$countryLower = $Country.ToLowerInvariant()

	return @{
		'ConfigTransform' = $null
		'2Gis.Erm.UI.Web.Mvc' = @{
			'IisAppPath' = "web-app.edu.erm.$countryLower"
			'TargetHosts' = @('uk-erm-edu01')
		}
		'2Gis.Erm.API.WCF.Operations' = @{
			'IisAppPath' = "basic-operations.api.edu.erm.$countryLower"
			'TargetHosts' = @('uk-erm-edu01')
		}
		'2Gis.Erm.API.WCF.Metadata' = @{
			'IisAppPath' = "metadata.api.edu.erm.$countryLower"
			'TargetHosts' = @('uk-erm-edu01')
		}
		'2Gis.Erm.API.WCF.OrderValidation' = @{
			'IisAppPath' = "order-validation.api.edu.erm.$countryLower"
			'TargetHosts' = @('uk-erm-edu01')
		}
		'2Gis.Erm.API.WCF.Releasing' = @{
			'IisAppPath' = "releasing.api.edu.erm.$countryLower"
			'TargetHosts' = @('uk-erm-edu01')
		}
		'2Gis.Erm.TaskService.Installer' = @{
			'TargetHosts' = @('uk-erm-edu01')
		}
		'ErmReports' = @{
			'ServerUrls' = @('http://uk-erm-edu01/ReportServer')
			'ReportsFolder' = "/Edu.$Country"
			'Region' = $CountryShortName[$Country]
		}
	}
}

function FromTemplate-Production($Country){

	$countryLower = $Country.ToLowerInvariant()

	return @{
		'ConfigTransform' = $null
		'2Gis.Erm.UI.Web.Mvc' = @{
			'IisAppPath' = "web-app.prod.erm.$countryLower"
			'TargetHosts' = @('uk-erm-iis03')
		}
		'2Gis.Erm.API.WCF.Operations' = @{
			'IisAppPath' = "basic-operations.api.prod.erm.$countryLower"
			'TargetHosts' = @('uk-erm-iis03')
		}
		'2Gis.Erm.API.WCF.Metadata' = @{
			'IisAppPath' = "metadata.api.prod.erm.$countryLower"
			'TargetHosts' = @('uk-erm-iis03')
		}
		'2Gis.Erm.API.WCF.OrderValidation' = @{
			'IisAppPath' = "order-validation.api.prod.erm.$countryLower"
			'TargetHosts' = @('uk-erm-iis03')
		}
		'2Gis.Erm.API.WCF.Releasing' = @{
			'IisAppPath' = "releasing.api.prod.erm.$countryLower"
			'TargetHosts' = @('uk-erm-iis03')
		}
		'2Gis.Erm.TaskService.Installer' = @{
			'TargetHosts' = @('uk-erm-iis01', 'uk-erm-iis02', 'uk-erm-iis03')
		}
		'ErmReports' = @{
			'ServerUrls' = @('http://uk-rpt/ReportServer')
			'ReportsFolder' = '/ERM_ENG'
			'Region' = $CountryShortName[$Country]
		}
	}
}

$AllMetadata = @{

	'AllEmpty' = @{
		'ConfigTransform' = @{
			'Xdt' = $null
			'Regex' = $null
		}
		'2Gis.Erm.UI.Web.Mvc' = @{
			'IisAppPath' = $null
			'TargetHosts' = $null
		}
		'2Gis.Erm.API.WCF.Operations' = @{
			'IisAppPath' = $null
			'TargetHosts' = $null
		}
		'2Gis.Erm.API.WCF.MoDi' = @{
			'IisAppPath' = $null
			'TargetHosts' = $null
		}
		'2Gis.Erm.API.WCF.Metadata' = @{
			'IisAppPath' = $null
			'TargetHosts' = $null
		}
		'2Gis.Erm.API.WCF.OrderValidation' = @{
			'IisAppPath' = $null
			'TargetHosts' = $null
		}
		'2Gis.Erm.API.WCF.Operations.Special' = @{
			'IisAppPath' = $null
			'TargetHosts' = $null
		}
		'2Gis.Erm.API.WCF.Releasing' = @{
			'IisAppPath' = $null
			'TargetHosts' = $null
		}
		'2Gis.Erm.TaskService.Installer' = @{
			'TargetHosts' = $null
		}
		'2Gis.Erm.UI.Desktop.WPF' = @{
			'IisAppPath' = $null
			'TargetHosts' = $null
		}
	}

	'Edu.Chile' = FromTemplate-Edu 'Chile'
	'Edu.Cyprus' = FromTemplate-Edu 'Cyprus'
	'Edu.Emirates' = FromTemplate-Edu 'Emirates'
	'Edu.Czech' = FromTemplate-Edu 'Czech'
	'Edu.Ukraine' = FromTemplate-Edu 'Ukraine'
	'Edu.Russia' = FromTemplate-Edu 'Russia' + @{
		'2Gis.Erm.API.WCF.MoDi' = @{
			'IisAppPath' = 'money-distribution.api.edu.erm.russia'
			'TargetHosts' = @('uk-erm-edu01')
		}
		'2Gis.Erm.API.WCF.Operations.Special' = @{
			'IisAppPath' = 'financial-operations.api.edu.erm.russia'
			'TargetHosts' = @('uk-erm-edu01')
		}
	}

	'Expl.Cyprus' = @{
		'ConfigTransform' = $null
		'2Gis.Erm.UI.Web.Mvc' = @{
			'IisAppPath' = 'web-app.erm-cyprus.2gis.local'
			'TargetHosts' = @('uk-expl02')
		}
		'2Gis.Erm.API.WCF.Operations' = @{
			'IisAppPath' = 'basic-operations.api.erm-cyprus.2gis.local'
			'TargetHosts' = @('uk-expl02')
		}
		'2Gis.Erm.API.WCF.Metadata' = @{
			'IisAppPath' = 'metadata.api.erm-cyprus.2gis.local'
			'TargetHosts' = @('uk-expl02')
		}
		'2Gis.Erm.API.WCF.OrderValidation' = @{
			'IisAppPath' = 'order-validation.api.erm-cyprus.2gis.local'
			'TargetHosts' = @('uk-expl02')
		}
		'2Gis.Erm.API.WCF.Releasing' = @{
			'IisAppPath' = 'releasing.api.erm-cyprus.2gis.local'
			'TargetHosts' = @('uk-expl02')
		}
		'2Gis.Erm.TaskService.Installer' = @{
			'TargetHosts' = @('uk-expl02')
		}
	}

	'Expl.Russia' = @{
		'ConfigTransform' = $null
		'2Gis.Erm.UI.Web.Mvc' = @{
			'IisAppPath' = 'web-app.erm-russia.2gis.local'
			'TargetHosts' = @('uk-expl03')
		}
		'2Gis.Erm.API.WCF.Operations' = @{
			'IisAppPath' = 'basic-operations.api.erm-russia.2gis.local'
			'TargetHosts' = @('uk-expl03')
		}
		'2Gis.Erm.API.WCF.MoDi' = @{
			'IisAppPath' = 'money-distribution.api.erm-russia.2gis.local'
			'TargetHosts' = @('uk-expl03')
		}
		'2Gis.Erm.API.WCF.Metadata' = @{
			'IisAppPath' = 'metadata.api.erm-russia.2gis.local'
			'TargetHosts' = @('uk-expl03')
		}
		'2Gis.Erm.API.WCF.OrderValidation' = @{
			'IisAppPath' = 'order-validation.api.erm-russia.2gis.local'
			'TargetHosts' = @('uk-expl03')
		}
		'2Gis.Erm.API.WCF.Operations.Special' = @{
			'IisAppPath' = 'financial-operations.api.erm-russia.2gis.local'
			'TargetHosts' = @('uk-expl03')
		}
		'2Gis.Erm.API.WCF.Releasing' = @{
			'IisAppPath' = 'releasing.api.erm-russia.2gis.local'
			'TargetHosts' = @('uk-expl03')
		}
		'2Gis.Erm.TaskService.Installer' = @{
			'TargetHosts' = @('uk-expl03')
		}
	}

	'Int.Chile' = @{
		'ConfigTransform' = $null
		'2Gis.Erm.UI.Web.Mvc' = @{
			'IisAppPath' = 'web-app.int.erm.chile'
			'TargetHosts' = @('uk-test-int01')
		}
		'2Gis.Erm.API.WCF.Operations' = @{
			'IisAppPath' = 'basic-operations.api.int.erm.chile'
			'TargetHosts' = @('uk-test-int01')
		}
		'2Gis.Erm.API.WCF.Metadata' = @{
			'IisAppPath' = 'metadata.api.int.erm.chile'
			'TargetHosts' = @('uk-test-int01')
		}
		'2Gis.Erm.API.WCF.OrderValidation' = @{
			'IisAppPath' = 'order-validation.api.int.erm.chile'
			'TargetHosts' = @('uk-test-int01')
		}
		'2Gis.Erm.API.WCF.Releasing' = @{
			'IisAppPath' = 'releasing.api.int.erm.chile'
			'TargetHosts' = @('uk-test-int01')
		}
		'2Gis.Erm.TaskService.Installer' = @{
			'TargetHosts' = @('uk-test-int01')
		}
		'ErmReports' = @{
			'ServerUrls' = @('http://uk-test-int03/ReportServer')
			'ReportsFolder' = '/Int.Chile'
			'Region' = 'CL'
		}
	}
	
	'Int.Cyprus' = @{
		'ConfigTransform' = $null
		'2Gis.Erm.UI.Web.Mvc' = @{
			'IisAppPath' = 'web-app.int.erm.cyprus'
			'TargetHosts' = @('uk-test-int01')
		}
		'2Gis.Erm.API.WCF.Operations' = @{
			'IisAppPath' = 'basic-operations.api.int.erm.cyprus'
			'TargetHosts' = @('uk-test-int01')
		}
		'2Gis.Erm.API.WCF.Metadata' = @{
			'IisAppPath' = 'metadata.api.int.erm.cyprus'
			'TargetHosts' = @('uk-test-int01')
		}
		'2Gis.Erm.API.WCF.OrderValidation' = @{
			'IisAppPath' = 'order-validation.api.int.erm.cyprus'
			'TargetHosts' = @('uk-test-int01')
		}
		'2Gis.Erm.API.WCF.Releasing' = @{
			'IisAppPath' = 'releasing.api.int.erm.cyprus'
			'TargetHosts' = @('uk-test-int01')
		}
		'2Gis.Erm.TaskService.Installer' = @{
			'TargetHosts' = @('uk-test-int01')
		}
		'ErmReports' = @{
			'ServerUrls' = @('http://uk-test-int03/ReportServer')
			'ReportsFolder' = '/Int.Cyprus'
			'Region' = 'CY'
		}
	}

	'Int.Czech' = @{
		'ConfigTransform' = $null
		'2Gis.Erm.UI.Web.Mvc' = @{
			'IisAppPath' = 'web-app.int.erm.czech'
			'TargetHosts' = @('uk-test-int01')
		}
		'2Gis.Erm.API.WCF.Operations' = @{
			'IisAppPath' = 'basic-operations.api.int.erm.czech'
			'TargetHosts' = @('uk-test-int01')
		}
		'2Gis.Erm.API.WCF.Metadata' = @{
			'IisAppPath' = 'metadata.api.int.erm.czech'
			'TargetHosts' = @('uk-test-int01')
		}
		'2Gis.Erm.API.WCF.OrderValidation' = @{
			'IisAppPath' = 'order-validation.api.int.erm.czech'
			'TargetHosts' = @('uk-test-int01')
		}
		'2Gis.Erm.API.WCF.Releasing' = @{
			'IisAppPath' = 'releasing.api.int.erm.czech'
			'TargetHosts' = @('uk-test-int01')
		}
		'2Gis.Erm.TaskService.Installer' = @{
			'TargetHosts' = @('uk-test-int01')
		}
		'ErmReports' = @{
			'ServerUrls' = @('http://uk-test-int03/ReportServer')
			'ReportsFolder' = '/Int.Czech'
			'Region' = 'CZ'
		}
	}

	'Int.Russia' = @{
		'ConfigTransform' = $null
		'2Gis.Erm.UI.Web.Mvc' = @{
			'IisAppPath' = 'web-app.int.erm.russia'
			'TargetHosts' = @('uk-test-int02')
		}
		'2Gis.Erm.API.WCF.Operations' = @{
			'IisAppPath' = 'basic-operations.api.int.erm.russia'
			'TargetHosts' = @('uk-test-int02')
		}
		'2Gis.Erm.API.WCF.MoDi' = @{
			'IisAppPath' = 'money-distribution.api.int.erm.russia'
			'TargetHosts' = @('uk-test-int02')
		}
		'2Gis.Erm.API.WCF.Metadata' = @{
			'IisAppPath' = 'metadata.api.int.erm.russia'
			'TargetHosts' = @('uk-test-int02')
		}
		'2Gis.Erm.API.WCF.OrderValidation' = @{
			'IisAppPath' = 'order-validation.api.int.erm.russia'
			'TargetHosts' = @('uk-test-int02')
		}
		'2Gis.Erm.API.WCF.Operations.Special' = @{
			'IisAppPath' = 'financial-operations.api.int.erm.russia'
			'TargetHosts' = @('uk-test-int02')
		}
		'2Gis.Erm.API.WCF.Releasing' = @{
			'IisAppPath' = 'releasing.api.int.erm.russia'
			'TargetHosts' = @('uk-test-int02')
		}
		'2Gis.Erm.TaskService.Installer' = @{
			'TargetHosts' = @('uk-test-int02')
		}
		'ErmReports' = @{
			'ServerUrls' = @('http://uk-test-int03/ReportServer')
			'ReportsFolder' = '/Int.Russia'
			'Region' = 'RU'
		}
	}

	'Load.Russia' = @{
		'ConfigTransform' = $null
		'2Gis.Erm.UI.Web.Mvc' = @{
			'IisAppPath' = 'web-app.load.erm.russia'
			'TargetHosts' = @('uk-erm-load03', 'uk-erm-load04')
		}
		'2Gis.Erm.API.WCF.Operations' = @{
			'IisAppPath' = 'basic-operations.api.load.erm.russia'
			'TargetHosts' = @('uk-erm-load03', 'uk-erm-load04')
		}
		'2Gis.Erm.API.WCF.MoDi' = @{
			'IisAppPath' = 'money-distribution.api.load.erm.russia'
			'TargetHosts' = @('uk-erm-load03', 'uk-erm-load04')
		}
		'2Gis.Erm.API.WCF.Metadata' = @{
			'IisAppPath' = 'metadata.api.load.erm.russia'
			'TargetHosts' = @('uk-erm-load03', 'uk-erm-load04')
		}
		'2Gis.Erm.API.WCF.OrderValidation' = @{
			'IisAppPath' = 'order-validation.api.load.erm.russia'
			'TargetHosts' = @('uk-erm-load03', 'uk-erm-load04')
		}
		'2Gis.Erm.API.WCF.Operations.Special' = @{
			'IisAppPath' = 'financial-operations.api.load.erm.russia'
			'TargetHosts' = @('uk-erm-load03', 'uk-erm-load04')
		}
		'2Gis.Erm.API.WCF.Releasing' = @{
			'IisAppPath' = 'releasing.api.load.erm.russia'
			'TargetHosts' = @('uk-erm-load03', 'uk-erm-load04')
		}
		'2Gis.Erm.TaskService.Installer' = @{
			'TargetHosts' = @('uk-erm-load03', 'uk-erm-load04')
		}
		'ErmReports' = @{
			'ServerUrls' = @('http://uk-test-sql01/ReportServer')
			'ReportsFolder' = '/Load.Russia'
			'Region' = 'RU'
		}
	}

	'Production.Chile' = FromTemplate-Production 'Chile'
	'Production.Cyprus' = FromTemplate-Production 'Cyprus'
	'Production.Emirates' = FromTemplate-Production 'Emirates'
	'Production.Czech' = FromTemplate-Production 'Czech'
	'Production.Ukraine' = FromTemplate-Production 'Ukraine'
	'Production.Russia' = @{
		'ConfigTransform' = $null
		'2Gis.Erm.UI.Web.Mvc' = @{
			'IisAppPath' = 'web-app.prod.erm.2gis.ru'
			'TargetHosts' = @('uk-crm01', 'uk-crm02', 'uk-crm03')
		}
		'2Gis.Erm.API.WCF.Operations' = @{
			'IisAppPath' = 'basic-operations.api.prod.erm.2gis.ru'
			'TargetHosts' = @('uk-crm01', 'uk-crm02', 'uk-crm03')
		}
		'2Gis.Erm.API.WCF.MoDi' = @{
			'IisAppPath' = 'money-distribution.api.prod.erm.2gis.ru'
			'TargetHosts' = @('uk-crm01', 'uk-crm02', 'uk-crm03')
		}
		'2Gis.Erm.API.WCF.Metadata' = @{
			'IisAppPath' = 'metadata.api.prod.erm.2gis.ru'
			'TargetHosts' = @('uk-crm01', 'uk-crm02', 'uk-crm03')
		}
		'2Gis.Erm.API.WCF.OrderValidation' = @{
			'IisAppPath' = 'order-validation.api.prod.erm.2gis.ru'
			'TargetHosts' = @('uk-crm01', 'uk-crm02', 'uk-crm03')
		}
		'2Gis.Erm.API.WCF.Operations.Special' = @{
			'IisAppPath' = 'financial-operations.api.prod.erm.2gis.ru'
			'TargetHosts' = @('uk-crm01', 'uk-crm02', 'uk-crm03')
		}
		# releasing на отдёльном серваке
		'2Gis.Erm.API.WCF.Releasing' = @{
			'IisAppPath' = 'releasing.api.prod.erm.2gis.ru'
			'TargetHosts' = @('uk-crm01')
		}
		'2Gis.Erm.TaskService.Installer' = @{
			'TargetHosts' = @('uk-crm01', 'uk-crm02', 'uk-crm03')
		}
		'ErmReports' = @{
			'ServerUrls' = @('http://uk-rpt/ReportServer')
			'ReportsFolder' = '/MSCRM'
			'Region' = 'RU'
		}
	}

	'Test.01' = FromTemplate-Test.Russia.CRM '01'
	'Test.02' = FromTemplate-Test.Russia.CRM '02'
	'Test.03' = FromTemplate-Test.Russia.CRM '03'
	'Test.04' = FromTemplate-Test.Russia.CRM '04'
	'Test.05' = FromTemplate-Test.Russia.CRM '05'
	'Test.06' = FromTemplate-Test.Russia.CRM '06'

	'Test.07' = FromTemplate-Test.Russia '07'
	'Test.08' = FromTemplate-Test.Russia '08'
	'Test.09' = FromTemplate-Test.Russia '09'
	'Test.10' = FromTemplate-Test.Russia '10'
	'Test.11' = FromTemplate-Test.Russia '11'
	'Test.12' = FromTemplate-Test.Russia '12'
	'Test.13' = FromTemplate-Test.Russia '13'
	'Test.14' = FromTemplate-Test.Russia '14'
	'Test.15' = FromTemplate-Test.Russia '15'
	'Test.16' = FromTemplate-Test.Russia '16'
	'Test.17' = FromTemplate-Test.Russia '17'
	'Test.18' = FromTemplate-Test.Russia '18'
	'Test.19' = FromTemplate-Test.Russia '19'
	'Test.20' = FromTemplate-Test.Russia '20'
	'Test.21' = FromTemplate-Test.Russia '21'
	'Test.22' = FromTemplate-Test.Russia '22'
	'Test.23' = FromTemplate-Test.Russia '23'
	'Test.24' = FromTemplate-Test.Russia '24'
	'Test.25' = FromTemplate-Test.Russia '25'
	
	'Test.101' = FromTemplate-Test 'Cyprus' '101'
	'Test.102' = FromTemplate-Test 'Cyprus' '102'
	'Test.108' = FromTemplate-Test 'Cyprus' '108'

	'Test.201' = FromTemplate-Test 'Czech' '201'
	'Test.202' = FromTemplate-Test 'Czech' '202'
	'Test.208' = FromTemplate-Test 'Czech' '208'

	'Test.301' = FromTemplate-Test 'Chile' '301'
	'Test.302' = FromTemplate-Test 'Chile' '302'
	'Test.308' = FromTemplate-Test 'Chile' '308'
	'Test.320' = FromTemplate-Test 'Chile' '320'
	
	'Test.401' = FromTemplate-Test 'Ukraine' '401'
	'Test.402' = FromTemplate-Test 'Ukraine' '402'
	'Test.408' = FromTemplate-Test 'Ukraine' '408'

	'Test.501' = FromTemplate-Test 'Emirates' '501'
	'Test.502' = FromTemplate-Test 'Emirates' '502'
	'Test.503' = FromTemplate-Test 'Emirates' '503'
	'Test.508' = FromTemplate-Test 'Emirates' '508'
}

function Get-EntryPointMetadata([ValidateSet(
'ConfigTransform',
'2Gis.Erm.UI.Web.Mvc',
'2Gis.Erm.API.WCF.Operations',
'2Gis.Erm.API.WCF.MoDi',
'2Gis.Erm.API.WCF.Metadata',
'2Gis.Erm.API.WCF.OrderValidation',
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
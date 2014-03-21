Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

$AllMetadata = @{

	'AllEmpty' = @{
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

	'Edu.Chile' = @{
		'2Gis.Erm.UI.Web.Mvc' = @{
			'IisAppPath' = 'web-app.edu.erm.chile'
			'TargetHosts' = @('uk-erm-edu01')
		}
		'2Gis.Erm.API.WCF.Operations' = @{
			'IisAppPath' = 'basic-operations.api.edu.erm.chile'
			'TargetHosts' = @('uk-erm-edu01')
		}
		'2Gis.Erm.API.WCF.Metadata' = @{
			'IisAppPath' = 'metadata.api.edu.erm.chile'
			'TargetHosts' = @('uk-erm-edu01')
		}
		'2Gis.Erm.API.WCF.OrderValidation' = @{
			'IisAppPath' = 'order-validation.api.edu.erm.chile'
			'TargetHosts' = @('uk-erm-edu01')
		}
		'2Gis.Erm.API.WCF.Releasing' = @{
			'IisAppPath' = 'releasing.api.edu.erm.chile'
			'TargetHosts' = @('uk-erm-edu01')
		}
		'2Gis.Erm.TaskService.Installer' = @{
			'TargetHosts' = @('uk-erm-edu01')
		}
		'ErmReports' = @{
			'ServerUrls' = @('http://uk-erm-edu01/ReportServer')
			'ReportsFolder' = '/Edu.Chile'
			'Region' = 'CL'
		}
	}

	'Edu.Cyprus' = @{
		'2Gis.Erm.UI.Web.Mvc' = @{
			'IisAppPath' = 'web-app.edu.erm.cyprus'
			'TargetHosts' = @('uk-erm-edu01')
		}
		'2Gis.Erm.API.WCF.Operations' = @{
			'IisAppPath' = 'basic-operations.api.edu.erm.cyprus'
			'TargetHosts' = @('uk-erm-edu01')
		}
		'2Gis.Erm.API.WCF.Metadata' = @{
			'IisAppPath' = 'metadata.api.edu.erm.cyprus'
			'TargetHosts' = @('uk-erm-edu01')
		}
		'2Gis.Erm.API.WCF.OrderValidation' = @{
			'IisAppPath' = 'order-validation.api.edu.erm.cyprus'
			'TargetHosts' = @('uk-erm-edu01')
		}
		'2Gis.Erm.API.WCF.Releasing' = @{
			'IisAppPath' = 'releasing.api.edu.erm.cyprus'
			'TargetHosts' = @('uk-erm-edu01')
		}
		'2Gis.Erm.TaskService.Installer' = @{
			'TargetHosts' = @('uk-erm-edu01')
		}
		'ErmReports' = @{
			'ServerUrls' = @('http://uk-erm-edu01/ReportServer')
			'ReportsFolder' = '/Edu.Cyprus'
			'Region' = 'CY'
		}
	}

	'Edu.Czech' = @{
		'2Gis.Erm.UI.Web.Mvc' = @{
			'IisAppPath' = 'web-app.edu.erm.czech'
			'TargetHosts' = @('uk-erm-edu01')
		}
		'2Gis.Erm.API.WCF.Operations' = @{
			'IisAppPath' = 'basic-operations.api.edu.erm.czech'
			'TargetHosts' = @('uk-erm-edu01')
		}
		'2Gis.Erm.API.WCF.Metadata' = @{
			'IisAppPath' = 'metadata.api.edu.erm.czech'
			'TargetHosts' = @('uk-erm-edu01')
		}
		'2Gis.Erm.API.WCF.OrderValidation' = @{
			'IisAppPath' = 'order-validation.api.edu.erm.czech'
			'TargetHosts' = @('uk-erm-edu01')
		}
		'2Gis.Erm.API.WCF.Releasing' = @{
			'IisAppPath' = 'releasing.api.edu.erm.czech'
			'TargetHosts' = @('uk-erm-edu01')
		}
		'2Gis.Erm.TaskService.Installer' = @{
			'TargetHosts' = @('uk-erm-edu01')
		}
		'ErmReports' = @{
			'ServerUrls' = @('http://uk-erm-edu01/ReportServer')
			'ReportsFolder' = '/Edu.Czech'
			'Region' = 'CZ'
		}
	}

	'Edu.Russia' = @{
		'2Gis.Erm.UI.Web.Mvc' = @{
			'IisAppPath' = 'web-app.edu.erm.russia'
			'TargetHosts' = @('uk-erm-edu01')
		}
		'2Gis.Erm.API.WCF.Operations' = @{
			'IisAppPath' = 'basic-operations.api.edu.erm.russia'
			'TargetHosts' = @('uk-erm-edu01')
		}
		'2Gis.Erm.API.WCF.MoDi' = @{
			'IisAppPath' = 'money-distribution.api.edu.erm.russia'
			'TargetHosts' = @('uk-erm-edu01')
		}
		'2Gis.Erm.API.WCF.Metadata' = @{
			'IisAppPath' = 'metadata.api.edu.erm.russia'
			'TargetHosts' = @('uk-erm-edu01')
		}
		'2Gis.Erm.API.WCF.OrderValidation' = @{
			'IisAppPath' = 'order-validation.api.edu.erm.russia'
			'TargetHosts' = @('uk-erm-edu01')
		}
		'2Gis.Erm.API.WCF.Operations.Special' = @{
			'IisAppPath' = 'financial-operations.api.edu.erm.russia'
			'TargetHosts' = @('uk-erm-edu01')
		}
		'2Gis.Erm.API.WCF.Releasing' = @{
			'IisAppPath' = 'releasing.api.edu.erm.russia'
			'TargetHosts' = @('uk-erm-edu01')
		}
		'2Gis.Erm.TaskService.Installer' = @{
			'TargetHosts' = @('uk-erm-edu01')
		}
		'ErmReports' = @{
			'ServerUrls' = @('http://uk-erm-edu01/ReportServer')
			'ReportsFolder' = '/Edu.Russia'
			'Region' = 'RU'
		}
	}

	'Expl.Cyprus' = @{
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

	'Production.Chile' = @{
		'2Gis.Erm.UI.Web.Mvc' = @{
			'IisAppPath' = 'web-app.prod.erm.chile'
			'TargetHosts' = @('uk-erm-iis01', 'uk-erm-iis02')
		}
		'2Gis.Erm.API.WCF.Operations' = @{
			'IisAppPath' = 'basic-operations.api.prod.erm.chile'
			'TargetHosts' = @('uk-erm-iis01', 'uk-erm-iis02')
		}
		'2Gis.Erm.API.WCF.Metadata' = @{
			'IisAppPath' = 'metadata.api.prod.erm.chile'
			'TargetHosts' = @('uk-erm-iis01', 'uk-erm-iis02')
		}
		'2Gis.Erm.API.WCF.OrderValidation' = @{
			'IisAppPath' = 'order-validation.api.prod.erm.chile'
			'TargetHosts' = @('uk-erm-iis01', 'uk-erm-iis02')
		}
		'2Gis.Erm.API.WCF.Releasing' = @{
			'IisAppPath' = 'releasing.api.prod.erm.chile'
			'TargetHosts' = @('uk-erm-iis01', 'uk-erm-iis02')
		}
		'2Gis.Erm.TaskService.Installer' = @{
			'TargetHosts' = @('uk-erm-iis01', 'uk-erm-iis02')
		}
		'ErmReports' = @{
			'ServerUrls' = @('http://uk-rpt/ReportServer')
			'ReportsFolder' = '/ERM_ENG'
			'Region' = 'CL'
		}
	}

	'Production.Cyprus' = @{
		'2Gis.Erm.UI.Web.Mvc' = @{
			'IisAppPath' = 'web-app.prod.erm.cyprus'
			'TargetHosts' = @('uk-erm-iis01', 'uk-erm-iis02')
		}
		'2Gis.Erm.API.WCF.Operations' = @{
			'IisAppPath' = 'basic-operations.api.prod.erm.cyprus'
			'TargetHosts' = @('uk-erm-iis01', 'uk-erm-iis02')
		}
		'2Gis.Erm.API.WCF.Metadata' = @{
			'IisAppPath' = 'metadata.api.prod.erm.cyprus'
			'TargetHosts' = @('uk-erm-iis01', 'uk-erm-iis02')
		}
		'2Gis.Erm.API.WCF.OrderValidation' = @{
			'IisAppPath' = 'order-validation.api.prod.erm.cyprus'
			'TargetHosts' = @('uk-erm-iis01', 'uk-erm-iis02')
		}
		'2Gis.Erm.API.WCF.Releasing' = @{
			'IisAppPath' = 'releasing.api.prod.erm.cyprus'
			'TargetHosts' = @('uk-erm-iis01', 'uk-erm-iis02')
		}
		'2Gis.Erm.TaskService.Installer' = @{
			'TargetHosts' = @('uk-erm-iis01', 'uk-erm-iis02')
		}
		'ErmReports' = @{
			'ServerUrls' = @('http://uk-rpt/ReportServer')
			'ReportsFolder' = '/ERM_ENG'
			'Region' = 'CY'
		}
	}

	'Production.Czech' = @{
		'2Gis.Erm.UI.Web.Mvc' = @{
			'IisAppPath' = 'web-app.prod.erm.czech'
			'TargetHosts' = @('uk-erm-iis01', 'uk-erm-iis02')
		}
		'2Gis.Erm.API.WCF.Operations' = @{
			'IisAppPath' = 'basic-operations.api.prod.erm.czech'
			'TargetHosts' = @('uk-erm-iis01', 'uk-erm-iis02')
		}
		'2Gis.Erm.API.WCF.Metadata' = @{
			'IisAppPath' = 'metadata.api.prod.erm.czech'
			'TargetHosts' = @('uk-erm-iis01', 'uk-erm-iis02')
		}
		'2Gis.Erm.API.WCF.OrderValidation' = @{
			'IisAppPath' = 'order-validation.api.prod.erm.czech'
			'TargetHosts' = @('uk-erm-iis01', 'uk-erm-iis02')
		}
		'2Gis.Erm.API.WCF.Releasing' = @{
			'IisAppPath' = 'releasing.api.prod.erm.czech'
			'TargetHosts' = @('uk-erm-iis01', 'uk-erm-iis02')
		}
		'2Gis.Erm.TaskService.Installer' = @{
			'TargetHosts' = @('uk-erm-iis01', 'uk-erm-iis02')
		}
		'ErmReports' = @{
			'ServerUrls' = @('http://uk-rpt/ReportServer')
			'ReportsFolder' = '/ERM_ENG'
			'Region' = 'CZ'
		}
	}

	'Production.Russia' = @{
		'2Gis.Erm.UI.Web.Mvc' = @{
			'IisAppPath' = 'web-app.prod.erm.russia'
			'TargetHosts' = @('uk-crm01', 'uk-crm03')
		}
		'2Gis.Erm.API.WCF.Operations' = @{
			'IisAppPath' = 'basic-operations.api.prod.erm.russia'
			'TargetHosts' = @('uk-crm01', 'uk-crm03')
		}
		'2Gis.Erm.API.WCF.MoDi' = @{
			'IisAppPath' = 'money-distribution.api.prod.erm.russia'
			'TargetHosts' = @('uk-crm01', 'uk-crm03')
		}
		'2Gis.Erm.API.WCF.Metadata' = @{
			'IisAppPath' = 'metadata.api.prod.erm.russia'
			'TargetHosts' = @('uk-crm01', 'uk-crm03')
		}
		'2Gis.Erm.API.WCF.OrderValidation' = @{
			'IisAppPath' = 'order-validation.api.prod.erm.russia'
			'TargetHosts' = @('uk-crm01', 'uk-crm03')
		}
		'2Gis.Erm.API.WCF.Operations.Special' = @{
			'IisAppPath' = 'financial-operations.api.prod.erm.russia'
			'TargetHosts' = @('uk-crm01', 'uk-crm03')
		}
		# releasing на отдёльном серваке
		'2Gis.Erm.API.WCF.Releasing' = @{
			'IisAppPath' = 'releasing.api.prod.erm.russia'
			'TargetHosts' = @('uk-crm01')
		}
		'2Gis.Erm.TaskService.Installer' = @{
			'TargetHosts' = @('uk-crm01', 'uk-crm03')
		}
		'ErmReports' = @{
			'ServerUrls' = @('http://uk-rpt/ReportServer')
			'ReportsFolder' = '/MSCRM'
			'Region' = 'RU'
		}
	}

	'Test.01' = @{
		'2Gis.Erm.UI.Web.Mvc' = @{
			'IisAppPath' = 'web-app01.test.erm.russia'
			'TargetHosts' = @('uk-erm-test01')
		}
		'2Gis.Erm.API.WCF.Operations' = @{
			'IisAppPath' = 'basic-operations01.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test01')
		}
		'2Gis.Erm.API.WCF.MoDi' = @{
			'IisAppPath' = 'money-distribution01.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test01')
		}
		'2Gis.Erm.API.WCF.Metadata' = @{
			'IisAppPath' = 'metadata01.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test01')
		}
		'2Gis.Erm.API.WCF.OrderValidation' = @{
			'IisAppPath' = 'order-validation01.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test01')
		}
		'2Gis.Erm.API.WCF.Operations.Special' = @{
			'IisAppPath' = 'financial-operations01.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test01')
		}
		'2Gis.Erm.API.WCF.Releasing' = @{
			'IisAppPath' = 'releasing01.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test01')
		}
		'2Gis.Erm.TaskService.Installer' = @{
			'TargetHosts' = @('uk-erm-test01')
		}
		'ErmReports' = @{
			'ServerUrls' = @('http://uk-sql01/ReportServer')
			'ReportsFolder' = '/Test.01'
			'Region' = 'RU'
		}
	}

	'Test.02' = @{
		'2Gis.Erm.UI.Web.Mvc' = @{
			'IisAppPath' = 'web-app02.test.erm.russia'
			'TargetHosts' = @('uk-erm-test02')
		}
		'2Gis.Erm.API.WCF.Operations' = @{
			'IisAppPath' = 'basic-operations02.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test02')
		}
		'2Gis.Erm.API.WCF.MoDi' = @{
			'IisAppPath' = 'money-distribution02.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test02')
		}
		'2Gis.Erm.API.WCF.Metadata' = @{
			'IisAppPath' = 'metadata02.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test02')
		}
		'2Gis.Erm.API.WCF.OrderValidation' = @{
			'IisAppPath' = 'order-validation02.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test02')
		}
		'2Gis.Erm.API.WCF.Operations.Special' = @{
			'IisAppPath' = 'financial-operations02.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test02')
		}
		'2Gis.Erm.API.WCF.Releasing' = @{
			'IisAppPath' = 'releasing02.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test02')
		}
		'2Gis.Erm.TaskService.Installer' = @{
			'TargetHosts' = @('uk-erm-test02')
		}
		'ErmReports' = @{
			'ServerUrls' = @('http://uk-sql01/ReportServer')
			'ReportsFolder' = '/Test.02'
			'Region' = 'RU'
		}
	}

	'Test.03' = @{
		'2Gis.Erm.UI.Web.Mvc' = @{
			'IisAppPath' = 'web-app03.test.erm.russia'
			'TargetHosts' = @('uk-erm-test03')
		}
		'2Gis.Erm.API.WCF.Operations' = @{
			'IisAppPath' = 'basic-operations03.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test03')
		}
		'2Gis.Erm.API.WCF.MoDi' = @{
			'IisAppPath' = 'money-distribution03.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test03')
		}
		'2Gis.Erm.API.WCF.Metadata' = @{
			'IisAppPath' = 'metadata03.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test03')
		}
		'2Gis.Erm.API.WCF.OrderValidation' = @{
			'IisAppPath' = 'order-validation03.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test03')
		}
		'2Gis.Erm.API.WCF.Operations.Special' = @{
			'IisAppPath' = 'financial-operations03.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test03')
		}
		'2Gis.Erm.API.WCF.Releasing' = @{
			'IisAppPath' = 'releasing03.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test03')
		}
		'2Gis.Erm.TaskService.Installer' = @{
			'TargetHosts' = @('uk-erm-test03')
		}
		'2Gis.Erm.UI.Desktop.WPF' = @{
			'IisAppPath' = 'wpf-app03.test.erm.russia'
			'TargetHosts' = @('uk-erm-test03')
		}
		'ErmReports' = @{
			'ServerUrls' = @('http://uk-sql01/ReportServer')
			'ReportsFolder' = '/Test.03'
			'Region' = 'RU'
		}
	}

	'Test.04' = @{
		'2Gis.Erm.UI.Web.Mvc' = @{
			'IisAppPath' = 'web-app04.test.erm.russia'
			'TargetHosts' = @('uk-erm-test04')
		}
		'2Gis.Erm.API.WCF.Operations' = @{
			'IisAppPath' = 'basic-operations04.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test04')
		}
		'2Gis.Erm.API.WCF.MoDi' = @{
			'IisAppPath' = 'money-distribution04.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test04')
		}
		'2Gis.Erm.API.WCF.Metadata' = @{
			'IisAppPath' = 'metadata04.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test04')
		}
		'2Gis.Erm.API.WCF.OrderValidation' = @{
			'IisAppPath' = 'order-validation04.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test04')
		}
		'2Gis.Erm.API.WCF.Operations.Special' = @{
			'IisAppPath' = 'financial-operations04.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test04')
		}
		'2Gis.Erm.API.WCF.Releasing' = @{
			'IisAppPath' = 'releasing04.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test04')
		}
		'2Gis.Erm.TaskService.Installer' = @{
			'TargetHosts' = @('uk-erm-test04')
		}
		'ErmReports' = @{
			'ServerUrls' = @('http://uk-sql01/ReportServer')
			'ReportsFolder' = '/Test.04'
			'Region' = 'RU'
		}
	}

	'Test.05' = @{
		'2Gis.Erm.UI.Web.Mvc' = @{
			'IisAppPath' = 'web-app05.test.erm.russia'
			'TargetHosts' = @('uk-erm-test05')
		}
		'2Gis.Erm.API.WCF.Operations' = @{
			'IisAppPath' = 'basic-operations05.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test05')
		}
		'2Gis.Erm.API.WCF.MoDi' = @{
			'IisAppPath' = 'money-distribution05.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test05')
		}
		'2Gis.Erm.API.WCF.Metadata' = @{
			'IisAppPath' = 'metadata05.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test05')
		}
		'2Gis.Erm.API.WCF.OrderValidation' = @{
			'IisAppPath' = 'order-validation05.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test05')
		}
		'2Gis.Erm.API.WCF.Operations.Special' = @{
			'IisAppPath' = 'financial-operations05.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test05')
		}
		'2Gis.Erm.API.WCF.Releasing' = @{
			'IisAppPath' = 'releasing05.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test05')
		}
		'2Gis.Erm.TaskService.Installer' = @{
			'TargetHosts' = @('uk-erm-test05')
		}
		'2Gis.Erm.UI.Desktop.WPF' = @{
			'IisAppPath' = 'wpf-app05.test.erm.russia'
			'TargetHosts' = @('uk-erm-test05')
		}
		'ErmReports' = @{
			'ServerUrls' = @('http://uk-sql01/ReportServer')
			'ReportsFolder' = '/Test.05'
			'Region' = 'RU'
		}
	}

	'Test.06' = @{
		'2Gis.Erm.UI.Web.Mvc' = @{
			'IisAppPath' = 'web-app06.test.erm.russia'
			'TargetHosts' = @('uk-erm-test06')
		}
		'2Gis.Erm.API.WCF.Operations' = @{
			'IisAppPath' = 'basic-operations06.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test06')
		}
		'2Gis.Erm.API.WCF.MoDi' = @{
			'IisAppPath' = 'money-distribution06.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test06')
		}
		'2Gis.Erm.API.WCF.Metadata' = @{
			'IisAppPath' = 'metadata06.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test06')
		}
		'2Gis.Erm.API.WCF.OrderValidation' = @{
			'IisAppPath' = 'order-validation06.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test06')
		}
		'2Gis.Erm.API.WCF.Operations.Special' = @{
			'IisAppPath' = 'financial-operations06.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test06')
		}
		'2Gis.Erm.API.WCF.Releasing' = @{
			'IisAppPath' = 'releasing06.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test06')
		}
		'2Gis.Erm.TaskService.Installer' = @{
			'TargetHosts' = @('uk-erm-test06')
		}
		'ErmReports' = @{
			'ServerUrls' = @('http://uk-sql01/ReportServer')
			'ReportsFolder' = '/Test.06'
			'Region' = 'RU'
		}
	}

	'Test.07' = @{
		'2Gis.Erm.UI.Web.Mvc' = @{
			'IisAppPath' = 'web-app07.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Operations' = @{
			'IisAppPath' = 'basic-operations07.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.MoDi' = @{
			'IisAppPath' = 'money-distribution07.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Metadata' = @{
			'IisAppPath' = 'metadata07.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.OrderValidation' = @{
			'IisAppPath' = 'order-validation07.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Operations.Special' = @{
			'IisAppPath' = 'financial-operations07.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Releasing' = @{
			'IisAppPath' = 'releasing07.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.TaskService.Installer' = @{
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.UI.Desktop.WPF' = @{
			'IisAppPath' = 'wpf-app07.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'ErmReports' = @{
			'ServerUrls' = @('http://uk-sql01/ReportServer')
			'ReportsFolder' = '/Test.07'
			'Region' = 'RU'
		}
	}

	'Test.08' = @{
		'2Gis.Erm.UI.Web.Mvc' = @{
			'IisAppPath' = 'web-app08.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Operations' = @{
			'IisAppPath' = 'basic-operations08.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.MoDi' = @{
			'IisAppPath' = 'money-distribution08.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Metadata' = @{
			'IisAppPath' = 'metadata08.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.OrderValidation' = @{
			'IisAppPath' = 'order-validation08.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Operations.Special' = @{
			'IisAppPath' = 'financial-operations08.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Releasing' = @{
			'IisAppPath' = 'releasing08.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.TaskService.Installer' = @{
			'TargetHosts' = @('uk-erm-test07')
		}
		'ErmReports' = @{
			'ServerUrls' = @('http://uk-sql01/ReportServer')
			'ReportsFolder' = '/Test.08'
			'Region' = 'RU'
		}
	}

	'Test.09' = @{
		'2Gis.Erm.UI.Web.Mvc' = @{
			'IisAppPath' = 'web-app09.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Operations' = @{
			'IisAppPath' = 'basic-operations09.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.MoDi' = @{
			'IisAppPath' = 'money-distribution09.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Metadata' = @{
			'IisAppPath' = 'metadata09.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.OrderValidation' = @{
			'IisAppPath' = 'order-validation09.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Operations.Special' = @{
			'IisAppPath' = 'financial-operations09.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Releasing' = @{
			'IisAppPath' = 'releasing09.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.TaskService.Installer' = @{
			'TargetHosts' = @('uk-erm-test07')
		}
		'ErmReports' = @{
			'ServerUrls' = @('http://uk-sql01/ReportServer')
			'ReportsFolder' = '/Test.09'
			'Region' = 'RU'
		}
	}
	
	'Test.10' = @{
		'2Gis.Erm.UI.Web.Mvc' = @{
			'IisAppPath' = 'web-app10.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Operations' = @{
			'IisAppPath' = 'basic-operations10.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.MoDi' = @{
			'IisAppPath' = 'money-distribution10.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Metadata' = @{
			'IisAppPath' = 'metadata10.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.OrderValidation' = @{
			'IisAppPath' = 'order-validation10.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Operations.Special' = @{
			'IisAppPath' = 'financial-operations10.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Releasing' = @{
			'IisAppPath' = 'releasing10.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.TaskService.Installer' = @{
			'TargetHosts' = @('uk-erm-test07')
		}
		'ErmReports' = @{
			'ServerUrls' = @('http://uk-sql01/ReportServer')
			'ReportsFolder' = '/Test.10'
			'Region' = 'RU'
		}
	}
	
	'Test.101' = @{
		'2Gis.Erm.UI.Web.Mvc' = @{
			'IisAppPath' = 'web-app101.test.erm.cyprus'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Operations' = @{
			'IisAppPath' = 'basic-operations101.api.test.erm.cyprus'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Metadata' = @{
			'IisAppPath' = 'metadata101.api.test.erm.cyprus'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.OrderValidation' = @{
			'IisAppPath' = 'order-validation101.api.test.erm.cyprus'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Releasing' = @{
			'IisAppPath' = 'releasing101.api.test.erm.cyprus'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.TaskService.Installer' = @{
			'TargetHosts' = @('uk-erm-test07')
		}
		'ErmReports' = @{
			'ServerUrls' = @('http://uk-sql01/ReportServer')
			'ReportsFolder' = '/Test.101'
			'Region' = 'CY'
		}
	}

	'Test.102' = @{
		'2Gis.Erm.UI.Web.Mvc' = @{
			'IisAppPath' = 'web-app102.test.erm.cyprus'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Operations' = @{
			'IisAppPath' = 'basic-operations102.api.test.erm.cyprus'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Metadata' = @{
			'IisAppPath' = 'metadata102.api.test.erm.cyprus'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.OrderValidation' = @{
			'IisAppPath' = 'order-validation102.api.test.erm.cyprus'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Releasing' = @{
			'IisAppPath' = 'releasing102.api.test.erm.cyprus'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.TaskService.Installer' = @{
			'TargetHosts' = @('uk-erm-test07')
		}
		'ErmReports' = @{
			'ServerUrls' = @('http://uk-sql01/ReportServer')
			'ReportsFolder' = '/Test.102'
			'Region' = 'CY'
		}
	}

	'Test.11' = @{
		'2Gis.Erm.UI.Web.Mvc' = @{
			'IisAppPath' = 'web-app11.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Operations' = @{
			'IisAppPath' = 'basic-operations11.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.MoDi' = @{
			'IisAppPath' = 'money-distribution11.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Metadata' = @{
			'IisAppPath' = 'metadata11.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.OrderValidation' = @{
			'IisAppPath' = 'order-validation11.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Operations.Special' = @{
			'IisAppPath' = 'financial-operations11.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Releasing' = @{
			'IisAppPath' = 'releasing11.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.TaskService.Installer' = @{
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.UI.Desktop.WPF' = @{
			'IisAppPath' = 'wpf-app11.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'ErmReports' = @{
			'ServerUrls' = @('http://uk-sql01/ReportServer')
			'ReportsFolder' = '/Test.11'
			'Region' = 'RU'
		}
	}

	'Test.12' = @{
		'2Gis.Erm.UI.Web.Mvc' = @{
			'IisAppPath' = 'web-app12.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Operations' = @{
			'IisAppPath' = 'basic-operations12.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.MoDi' = @{
			'IisAppPath' = 'money-distribution12.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Metadata' = @{
			'IisAppPath' = 'metadata12.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.OrderValidation' = @{
			'IisAppPath' = 'order-validation12.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Operations.Special' = @{
			'IisAppPath' = 'financial-operations12.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Releasing' = @{
			'IisAppPath' = 'releasing12.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.TaskService.Installer' = @{
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.UI.Desktop.WPF' = @{
			'IisAppPath' = 'wpf-app12.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'ErmReports' = @{
			'ServerUrls' = @('http://uk-sql01/ReportServer')
			'ReportsFolder' = '/Test.12'
			'Region' = 'RU'
		}
	}

	'Test.13' = @{
		'2Gis.Erm.UI.Web.Mvc' = @{
			'IisAppPath' = 'web-app13.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Operations' = @{
			'IisAppPath' = 'basic-operations13.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.MoDi' = @{
			'IisAppPath' = 'money-distribution13.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Metadata' = @{
			'IisAppPath' = 'metadata13.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.OrderValidation' = @{
			'IisAppPath' = 'order-validation13.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Operations.Special' = @{
			'IisAppPath' = 'financial-operations13.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Releasing' = @{
			'IisAppPath' = 'releasing13.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.TaskService.Installer' = @{
			'TargetHosts' = @('uk-erm-test07')
		}
		'ErmReports' = @{
			'ServerUrls' = @('http://uk-sql01/ReportServer')
			'ReportsFolder' = '/Test.13'
			'Region' = 'RU'
		}
	}
	
	'Test.14' = @{
		'2Gis.Erm.UI.Web.Mvc' = @{
			'IisAppPath' = 'web-app14.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Operations' = @{
			'IisAppPath' = 'basic-operations14.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.MoDi' = @{
			'IisAppPath' = 'money-distribution14.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Metadata' = @{
			'IisAppPath' = 'metadata14.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.OrderValidation' = @{
			'IisAppPath' = 'order-validation14.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Operations.Special' = @{
			'IisAppPath' = 'financial-operations14.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Releasing' = @{
			'IisAppPath' = 'releasing14.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.TaskService.Installer' = @{
			'TargetHosts' = @('uk-erm-test07')
		}
		'ErmReports' = @{
			'ServerUrls' = @('http://uk-sql01/ReportServer')
			'ReportsFolder' = '/Test.14'
			'Region' = 'RU'
		}
	}

	'Test.15' = @{
		'2Gis.Erm.UI.Web.Mvc' = @{
			'IisAppPath' = 'web-app15.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Operations' = @{
			'IisAppPath' = 'basic-operations15.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.MoDi' = @{
			'IisAppPath' = 'money-distribution15.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Metadata' = @{
			'IisAppPath' = 'metadata15.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.OrderValidation' = @{
			'IisAppPath' = 'order-validation15.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Operations.Special' = @{
			'IisAppPath' = 'financial-operations15.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Releasing' = @{
			'IisAppPath' = 'releasing15.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.TaskService.Installer' = @{
			'TargetHosts' = @('uk-erm-test07')
		}
		'ErmReports' = @{
			'ServerUrls' = @('http://uk-sql01/ReportServer')
			'ReportsFolder' = '/Test.15'
			'Region' = 'RU'
		}
	}

	'Test.16' = @{
		'2Gis.Erm.UI.Web.Mvc' = @{
			'IisAppPath' = 'web-app16.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Operations' = @{
			'IisAppPath' = 'basic-operations16.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.MoDi' = @{
			'IisAppPath' = 'money-distribution16.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Metadata' = @{
			'IisAppPath' = 'metadata16.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.OrderValidation' = @{
			'IisAppPath' = 'order-validation16.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Operations.Special' = @{
			'IisAppPath' = 'financial-operations16.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Releasing' = @{
			'IisAppPath' = 'releasing16.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.TaskService.Installer' = @{
			'TargetHosts' = @('uk-erm-test07')
		}
		'ErmReports' = @{
			'ServerUrls' = @('http://uk-sql01/ReportServer')
			'ReportsFolder' = '/Test.16'
			'Region' = 'RU'
		}
	}

	'Test.17' = @{
		'2Gis.Erm.UI.Web.Mvc' = @{
			'IisAppPath' = 'web-app17.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Operations' = @{
			'IisAppPath' = 'basic-operations17.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.MoDi' = @{
			'IisAppPath' = 'money-distribution17.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Metadata' = @{
			'IisAppPath' = 'metadata17.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.OrderValidation' = @{
			'IisAppPath' = 'order-validation17.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Operations.Special' = @{
			'IisAppPath' = 'financial-operations17.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Releasing' = @{
			'IisAppPath' = 'releasing17.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.TaskService.Installer' = @{
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.UI.Desktop.WPF' = @{
			'IisAppPath' = 'wpf-app17.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'ErmReports' = @{
			'ServerUrls' = @('http://uk-sql01/ReportServer')
			'ReportsFolder' = '/Test.17'
			'Region' = 'RU'
		}
	}

	'Test.18' = @{
		'2Gis.Erm.UI.Web.Mvc' = @{
			'IisAppPath' = 'web-app18.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Operations' = @{
			'IisAppPath' = 'basic-operations18.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.MoDi' = @{
			'IisAppPath' = 'money-distribution18.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Metadata' = @{
			'IisAppPath' = 'metadata18.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.OrderValidation' = @{
			'IisAppPath' = 'order-validation18.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Operations.Special' = @{
			'IisAppPath' = 'financial-operations18.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Releasing' = @{
			'IisAppPath' = 'releasing18.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.TaskService.Installer' = @{
			'TargetHosts' = @('uk-erm-test07')
		}
		'ErmReports' = @{
			'ServerUrls' = @('http://uk-sql01/ReportServer')
			'ReportsFolder' = '/Test.18'
			'Region' = 'RU'
		}
	}

	'Test.19' = @{
		'2Gis.Erm.UI.Web.Mvc' = @{
			'IisAppPath' = 'web-app19.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Operations' = @{
			'IisAppPath' = 'basic-operations19.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.MoDi' = @{
			'IisAppPath' = 'money-distribution19.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Metadata' = @{
			'IisAppPath' = 'metadata19.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.OrderValidation' = @{
			'IisAppPath' = 'order-validation19.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Operations.Special' = @{
			'IisAppPath' = 'financial-operations19.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Releasing' = @{
			'IisAppPath' = 'releasing19.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.TaskService.Installer' = @{
			'TargetHosts' = @('uk-erm-test07')
		}
		'ErmReports' = @{
			'ServerUrls' = @('http://uk-sql01/ReportServer')
			'ReportsFolder' = '/Test.19'
			'Region' = 'RU'
		}
	}

	'Test.20' = @{
		'2Gis.Erm.UI.Web.Mvc' = @{
			'IisAppPath' = 'web-app20.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Operations' = @{
			'IisAppPath' = 'basic-operations20.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.MoDi' = @{
			'IisAppPath' = 'money-distribution20.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Metadata' = @{
			'IisAppPath' = 'metadata20.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.OrderValidation' = @{
			'IisAppPath' = 'order-validation20.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Operations.Special' = @{
			'IisAppPath' = 'financial-operations20.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Releasing' = @{
			'IisAppPath' = 'releasing20.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.TaskService.Installer' = @{
			'TargetHosts' = @('uk-erm-test07')
		}
		'ErmReports' = @{
			'ServerUrls' = @('http://uk-sql01/ReportServer')
			'ReportsFolder' = '/Test.20'
			'Region' = 'RU'
		}
	}

	'Test.201' = @{
		'2Gis.Erm.UI.Web.Mvc' = @{
			'IisAppPath' = 'web-app201.test.erm.czech'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Operations' = @{
			'IisAppPath' = 'basic-operations201.api.test.erm.czech'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Metadata' = @{
			'IisAppPath' = 'metadata201.api.test.erm.czech'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.OrderValidation' = @{
			'IisAppPath' = 'order-validation201.api.test.erm.czech'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Releasing' = @{
			'IisAppPath' = 'releasing201.api.test.erm.czech'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.TaskService.Installer' = @{
			'TargetHosts' = @('uk-erm-test07')
		}
		'ErmReports' = @{
			'ServerUrls' = @('http://uk-sql01/ReportServer')
			'ReportsFolder' = '/Test.201'
			'Region' = 'CZ'
		}
	}

	'Test.202' = @{
		'2Gis.Erm.UI.Web.Mvc' = @{
			'IisAppPath' = 'web-app202.test.erm.czech'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Operations' = @{
			'IisAppPath' = 'basic-operations202.api.test.erm.czech'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Metadata' = @{
			'IisAppPath' = 'metadata202.api.test.erm.czech'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.OrderValidation' = @{
			'IisAppPath' = 'order-validation202.api.test.erm.czech'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Releasing' = @{
			'IisAppPath' = 'releasing202.api.test.erm.czech'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.TaskService.Installer' = @{
			'TargetHosts' = @('uk-erm-test07')
		}
		'ErmReports' = @{
			'ServerUrls' = @('http://uk-sql01/ReportServer')
			'ReportsFolder' = '/Test.202'
			'Region' = 'CZ'
		}
	}

	'Test.21' = @{
		'2Gis.Erm.UI.Web.Mvc' = @{
			'IisAppPath' = 'web-app21.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Operations' = @{
			'IisAppPath' = 'basic-operations21.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.MoDi' = @{
			'IisAppPath' = 'money-distribution21.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Metadata' = @{
			'IisAppPath' = 'metadata21.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.OrderValidation' = @{
			'IisAppPath' = 'order-validation21.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Operations.Special' = @{
			'IisAppPath' = 'financial-operations21.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Releasing' = @{
			'IisAppPath' = 'releasing21.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.TaskService.Installer' = @{
			'TargetHosts' = @('uk-erm-test07')
		}
		'ErmReports' = @{
			'ServerUrls' = @('http://uk-sql01/ReportServer')
			'ReportsFolder' = '/Test.21'
			'Region' = 'RU'
		}
	}

	'Test.22' = @{
		'2Gis.Erm.UI.Web.Mvc' = @{
			'IisAppPath' = 'web-app22.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Operations' = @{
			'IisAppPath' = 'basic-operations22.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.MoDi' = @{
			'IisAppPath' = 'money-distribution22.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test06')
		}
		'2Gis.Erm.API.WCF.Metadata' = @{
			'IisAppPath' = 'metadata22.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.OrderValidation' = @{
			'IisAppPath' = 'order-validation22.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Operations.Special' = @{
			'IisAppPath' = 'financial-operations22.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Releasing' = @{
			'IisAppPath' = 'releasing22.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.TaskService.Installer' = @{
			'TargetHosts' = @('uk-erm-test07')
		}
		'ErmReports' = @{
			'ServerUrls' = @('http://uk-sql01/ReportServer')
			'ReportsFolder' = '/Test.22'
			'Region' = 'RU'
		}
	}
	
	'Test.23' = @{
		'2Gis.Erm.UI.Web.Mvc' = @{
			'IisAppPath' = 'web-app23.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Operations' = @{
			'IisAppPath' = 'basic-operations23.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.MoDi' = @{
			'IisAppPath' = 'money-distribution23.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test06')
		}
		'2Gis.Erm.API.WCF.Metadata' = @{
			'IisAppPath' = 'metadata23.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.OrderValidation' = @{
			'IisAppPath' = 'order-validation23.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Operations.Special' = @{
			'IisAppPath' = 'financial-operations23.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Releasing' = @{
			'IisAppPath' = 'releasing23.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.TaskService.Installer' = @{
			'TargetHosts' = @('uk-erm-test07')
		}
		'ErmReports' = @{
			'ServerUrls' = @('http://uk-sql01/ReportServer')
			'ReportsFolder' = '/Test.23'
			'Region' = 'RU'
		}
	}
	
	'Test.24' = @{
		'2Gis.Erm.UI.Web.Mvc' = @{
			'IisAppPath' = 'web-app24.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Operations' = @{
			'IisAppPath' = 'basic-operations24.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.MoDi' = @{
			'IisAppPath' = 'money-distribution24.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Metadata' = @{
			'IisAppPath' = 'metadata24.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.OrderValidation' = @{
			'IisAppPath' = 'order-validation24.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Operations.Special' = @{
			'IisAppPath' = 'financial-operations24.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Releasing' = @{
			'IisAppPath' = 'releasing24.api.test.erm.russia'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.TaskService.Installer' = @{
			'TargetHosts' = @('uk-erm-test07')
		}
		'ErmReports' = @{
			'ServerUrls' = @('http://uk-sql01/ReportServer')
			'ReportsFolder' = '/Test.24'
			'Region' = 'RU'
		}
	}

	'Test.301' = @{
		'2Gis.Erm.UI.Web.Mvc' = @{
			'IisAppPath' = 'web-app301.test.erm.chile'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Operations' = @{
			'IisAppPath' = 'basic-operations301.api.test.erm.chile'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Metadata' = @{
			'IisAppPath' = 'metadata301.api.test.erm.chile'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.OrderValidation' = @{
			'IisAppPath' = 'order-validation301.api.test.erm.chile'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Releasing' = @{
			'IisAppPath' = 'releasing301.api.test.erm.chile'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.TaskService.Installer' = @{
			'TargetHosts' = @('uk-erm-test07')
		}
		'ErmReports' = @{
			'ServerUrls' = @('http://uk-sql01/ReportServer')
			'ReportsFolder' = '/Test.301'
			'Region' = 'CL'
		}
	}

	'Test.302' = @{
		'2Gis.Erm.UI.Web.Mvc' = @{
			'IisAppPath' = 'web-app302.test.erm.chile'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Operations' = @{
			'IisAppPath' = 'basic-operations302.api.test.erm.chile'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Metadata' = @{
			'IisAppPath' = 'metadata302.api.test.erm.chile'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.OrderValidation' = @{
			'IisAppPath' = 'order-validation302.api.test.erm.chile'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.API.WCF.Releasing' = @{
			'IisAppPath' = 'releasing302.api.test.erm.chile'
			'TargetHosts' = @('uk-erm-test07')
		}
		'2Gis.Erm.TaskService.Installer' = @{
			'TargetHosts' = @('uk-erm-test07')
		}
		'ErmReports' = @{
			'ServerUrls' = @('http://uk-sql01/ReportServer')
			'ReportsFolder' = '/Test.302'
			'Region' = 'CL'
		}
	}
}

function Get-EntryPointMetadata([ValidateSet(
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
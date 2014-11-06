Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

function Get-ReportsMetadata ($EnvType, $Country, $Index){

	switch ($EnvType){
		'Test' {
			switch ($Country) {
				'Russia' {
					return @{
						'ServerUrls' = @('http://uk-sql01/ReportServer')
						'ReportsFolder' = "/Test.$Index"
					}
				}
				default {
					return @{
						'ServerUrls' = @('http://uk-sql02/ReportServer')
						'ReportsFolder' = "/Test.$Index"
					}
				}
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
		default {
			throw "Unknown EnvType '$EnvType'"
		}
	}
}

Export-ModuleMember -Function Get-ReportsMetadata
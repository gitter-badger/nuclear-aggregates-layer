Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

function Get-ReportsMetadata ($EnvType, $Country, $Index){

	switch ($EnvType){
		'Test' {
			return @{
				'ReportsFolder' = "/Test.$Index"
			}
		}
		'Production' {
			switch ($Country) {
				'Russia' {
					return @{
						'ReportsFolder' = '/MSCRM'
					}
				}
				default {
					return @{
						'ReportsFolder' = '/ERM_ENG'
					}
				}
			}
		}
		{ @('Edu', 'Int', 'Load') -contains $_ } {
			return @{
				'ReportsFolder' = "/$EnvType.$Country"
			}
		}
		default {
			throw "Unknown EnvType '$EnvType'"
		}
	}
}

Export-ModuleMember -Function Get-ReportsMetadata
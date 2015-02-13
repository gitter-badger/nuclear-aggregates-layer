Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

function Get-OptionDynamicsMetadata ($EnvType, $Country, $Index) {

	switch ($Country){
		'Russia' {
			switch ($EnvType) {
				'Test' {
					$optionDynamics = [int]$Index -le 6
					return @{ 'OptionDynamics' = $optionDynamics }
				}
				default {
					return @{ 'OptionDynamics' = $true }
				}
			}
		}
		default {
			return @{ 'OptionDynamics' = $false }
		}
	}
}

function Get-CrmHostsMetadata ($EnvType, $Index) {

	switch ($EnvType) {
		'Production'{
			return @{ 'CrmHosts' = @('uk-crm01') }
		}
		'Edu' {
			return @{ 'CrmHosts' = @('uk-erm-edu01') }
		}
		'Business' {
			return @{ 'CrmHosts' = @('uk-erm-edu02') }
		}
		'Load' {
			return @{ 'CrmHosts' = @('uk-crm10') }
		}
		'Test' {
			return @{ 'CrmHosts' = @("uk-crm-test$Index") }
		}
		'Int' {
			return @{ 'CrmHosts' = @('uk-test-int02') }
		}
		default {
			throw "Unknown EnvType '$EnvType'"
		}
	}
}

function Get-DynamicsMetadata ($EnvType, $Country, $Index) {

	$metadata = @{}
	$metadata += Get-OptionDynamicsMetadata $EnvType $Country $Index
	
	if ($metadata.OptionDynamics){
		$metadata += Get-CrmHostsMetadata $EnvType $Index
	}
	
	return $metadata
}

Export-ModuleMember -Function Get-DynamicsMetadata
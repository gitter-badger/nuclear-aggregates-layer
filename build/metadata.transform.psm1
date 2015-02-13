Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module "$PSScriptRoot\metadata.dynamics.psm1" -DisableNameChecking
Import-Module "$PSScriptRoot\metadata.servicebus.psm1" -DisableNameChecking

function Get-XdtTransformMetadata($EnvType, $Country, $Index){
	$xdt = @(
		'Common\log4net.Release.config'
		'Common\Erm.Release.config'
	)

	switch ($EnvType){
		'Test' {
			switch ($Country){
				'Russia' {
					
					$dynamicsMetadata = Get-DynamicsMetadata $EnvType $Country $Index
					if ($dynamicsMetadata.OptionDynamics){
						$xdt += @('Templates\Erm.Test.Russia.CRM.config')
					} else {
						$xdt += @("Templates\Erm.Test.$Country.config")
					}
				}
				default {
					$xdt += @("Templates\Erm.Test.$Country.config")
				}
			}
		}
		default {}
	}

	return $xdt
}

function Get-RegexTransformMetadata($EnvName, $Index){

	$regex = @{}
	$regex += @{ '{EnvNum}' = "$Index" }
	$regex += Get-SharedAccessKeyMetadata $EnvName

	return $regex
}

function Get-TransformMetadata ($EnvName, $EnvType, $Country, $Index){
	
	$metadata = @{
		'Xdt' = Get-XdtTransformMetadata $EnvType $Country $Index
		'Regex' = Get-RegexTransformMetadata $EnvName $Index
	}

	return $metadata
}

Export-ModuleMember -Function Get-TransformMetadata
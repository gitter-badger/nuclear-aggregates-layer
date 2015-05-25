﻿Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module "$PSScriptRoot\metadata.dynamics.psm1" -DisableNameChecking
Import-Module "$PSScriptRoot\metadata.web.psm1" -DisableNameChecking

function Get-QuartzConfigMetadata ($EnvType, $Country, $Index){

	switch ($EnvType){
		'Test' {
			switch ($Country){
				'Russia' {
					$quartzConfigs = @('Templates\quartz.Test.Russia.config')
					
					$dynamicsMetadata = Get-DynamicsMetadata $EnvType $Country $Index
					if ($dynamicsMetadata.OptionDynamics){
						$quartzConfigs += @('Templates\quartz.Test.Russia.CRM.config')
					}
					$alterQuartzConfigs = @()
				}
				'Emirates' {
					$quartzConfigs = @('Templates\quartz.Test.Emirates.config')
					$alterQuartzConfigs = @()
				}
				default {
					$quartzConfigs = @('Templates\quartz.Test.MultiCulture.config')
					$alterQuartzConfigs = @()
				}
			}
		}
		'Production' {
			switch ($Country){
				'Russia' {
					$quartzConfigs = @('quartz.Production.Russia.config')
					$alterQuartzConfigs = @()
				}
				'Emirates' {
					$quartzConfigs = @('quartz.Production.Emirates.config')
					$alterQuartzConfigs = @()
				}
				default {
					$quartzConfigs = @('quartz.Production.MultiCulture.config')
					$alterQuartzConfigs = @()
				}
			}
		}
		default {
			switch ($Country){
				'Russia' {
					$quartzConfigs = @("quartz.$EnvType.Russia.config")
					$alterQuartzConfigs = @('Templates\quartz.Test.Russia.config')
					
					$dynamicsMetadata = Get-DynamicsMetadata $EnvType $Country $Index
					if ($dynamicsMetadata.OptionDynamics){
						$alterQuartzConfigs += @('Templates\quartz.Test.Russia.CRM.config')
					}
				}
				'Emirates' {
					$quartzConfigs = @("quartz.$EnvType.Emirates.config")
					$alterQuartzConfigs = @('Templates\quartz.Test.Emirates.config')
				}
				default {
					$quartzConfigs = @("quartz.$EnvType.MultiCulture.config")
					$alterQuartzConfigs = @('Templates\quartz.Test.MultiCulture.config')
				}
			}
		}
	}

	return @{
		'QuartzConfigs' =  $quartzConfigs
		'AlterQuartzConfigs' = $alterQuartzConfigs
	}
}

function Get-TargetHostsMetadata ($EnvType, $Country, $Index){

	$webMetadata = Get-WebMetadata $EnvType $Country '2Gis.Erm.TaskService.Installer' $Index

	switch ($EnvType) {
		'Production' {
			return @{ 'TargetHosts' = @('uk-erm-iis04', 'uk-erm-iis02', 'uk-erm-iis03') }
		}
		'Load' {
			return @{ 'TargetHosts' = @('uk-erm-iis10', 'uk-erm-iis11', 'uk-erm-iis12') }
		}
		default {
			return @{'TargetHosts' = $webMetadata.TargetHosts}
		}
	}
}

function Get-TaskServiceMetadata ($EnvType, $Country, $Index) {

	$metadata = @{}
	$metadata += Get-TargetHostsMetadata $EnvType $Country $Index
	$metadata += Get-QuartzConfigMetadata $EnvType $Country $Index
	
	$metadata += @{
		'ServiceName' = 'ERM'
		'ServiceDisplayName' = '2GIS ERM Task Service'
		
		'EntrypointType' = 'Desktop'
	}
	
	return $metadata
}

Export-ModuleMember -Function Get-TaskServiceMetadata
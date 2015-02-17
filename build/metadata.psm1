Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module "$PSScriptRoot\metadata.dynamics.psm1" -DisableNameChecking
Import-Module "$PSScriptRoot\metadata.web.psm1" -DisableNameChecking
Import-Module "$PSScriptRoot\metadata.taskservice.psm1" -DisableNameChecking
Import-Module "$PSScriptRoot\metadata.transform.psm1" -DisableNameChecking

function Get-MigrationsMetadata ($Country){
	switch ($Country){
		'Russia' {
			return @{ 'RunElasticsearchMigrations' = $true }
		}
		default {
			return @{ 'RunElasticsearchMigrations' = $false }
	}
}
}

function Get-EnvironmentMetadata ($EnvName, [ValidateSet('Test', 'Int', 'Load', 'Production', 'Edu', 'Business')]$EnvType, $Country, $Index) {

	return @{
		'Transform' = Get-TransformMetadata $EnvName $EnvType $Country $Index
		'2Gis.Erm.UI.Web.Mvc' = Get-WebMetadata $EnvType $Country '2Gis.Erm.UI.Web.Mvc' $Index
		'2Gis.Erm.API.WCF.Operations' = Get-WebMetadata $EnvType $Country '2Gis.Erm.API.WCF.Operations' $Index
		'2Gis.Erm.API.WCF.MoDi' = Get-WebMetadata $EnvType $Country '2Gis.Erm.API.WCF.MoDi' $Index
		'2Gis.Erm.API.WCF.Metadata' = Get-WebMetadata $EnvType $Country '2Gis.Erm.API.WCF.Metadata' $Index
		'2Gis.Erm.API.WCF.OrderValidation' = Get-WebMetadata $EnvType $Country '2Gis.Erm.API.WCF.OrderValidation' $Index
		'2Gis.Erm.API.WCF.Operations.Special' = Get-WebMetadata $EnvType $Country '2Gis.Erm.API.WCF.Operations.Special' $Index
		'2Gis.Erm.API.WCF.Releasing' = Get-WebMetadata $EnvType $Country '2Gis.Erm.API.WCF.Releasing' $Index
		'2Gis.Erm.UI.Desktop.WPF' = Get-WebMetadata $EnvType $Country '2Gis.Erm.UI.Desktop.WPF' $Index
		'2Gis.Erm.TaskService.Installer' = Get-TaskServiceMetadata $EnvName $EnvType $Country $Index
		'Migrations' = Get-MigrationsMetadata $Country
		'Dynamics' = Get-DynamicsMetadata $EnvType $Country $Index
	}
}

$EnvironmentMetadata = @{

	'Edu.Chile' = Get-EnvironmentMetadata 'Edu.Chile' 'Edu' 'Chile'
	'Edu.Cyprus' = Get-EnvironmentMetadata 'Edu.Cyprus' 'Edu' 'Cyprus'
	'Edu.Czech' = Get-EnvironmentMetadata 'Edu.Czech' 'Edu' 'Czech'
	'Edu.Emirates' = Get-EnvironmentMetadata 'Edu.Emirates' 'Edu' 'Emirates'
	'Edu.Kazakhstan' = Get-EnvironmentMetadata 'Edu.Kazakhstan' 'Edu' 'Kazakhstan'
	'Edu.Russia' = Get-EnvironmentMetadata 'Edu.Russia' 'Edu' 'Russia'
	'Edu.Ukraine' = Get-EnvironmentMetadata 'Edu.Ukraine' 'Edu' 'Ukraine'
	'Business.Russia' = Get-EnvironmentMetadata 'Business.Russia' 'Business' 'Russia'

	'Int.Chile' = Get-EnvironmentMetadata 'Int.Chile' 'Int' 'Chile'
	'Int.Cyprus' = Get-EnvironmentMetadata 'Int.Cyprus' 'Int' 'Cyprus'
	'Int.Czech' = Get-EnvironmentMetadata 'Int.Czech' 'Int' 'Czech'
	'Int.Emirates' = Get-EnvironmentMetadata 'Int.Emirates' 'Int' 'Emirates'
	'Int.Kazakhstan' = Get-EnvironmentMetadata 'Int.Kazakhstan' 'Int' 'Kazakhstan'
	'Int.Russia' = Get-EnvironmentMetadata 'Int.Russia' 'Int' 'Russia'
	'Int.Ukraine' = Get-EnvironmentMetadata 'Int.Ukraine' 'Int' 'Ukraine'
	
	'Load.Russia' = Get-EnvironmentMetadata 'Load.Russia' 'Load' 'Russia'
	'Load.Cyprus' = Get-EnvironmentMetadata 'Load.Cyprus' 'Load' 'Cyprus'
	'Load.Czech' = Get-EnvironmentMetadata 'Load.Czech' 'Load' 'Czech'
	'Load.Ukraine' = Get-EnvironmentMetadata 'Load.Ukraine' 'Load' 'Ukraine'

	'Production.Chile' = Get-EnvironmentMetadata 'Production.Chile' 'Production' 'Chile'
	'Production.Cyprus' = Get-EnvironmentMetadata 'Production.Cyprus' 'Production' 'Cyprus'
	'Production.Czech' = Get-EnvironmentMetadata 'Production.Czech' 'Production' 'Czech'
	'Production.Emirates' = Get-EnvironmentMetadata 'Production.Emirates' 'Production' 'Emirates'
	'Production.Kazakhstan' = Get-EnvironmentMetadata 'Production.Kazakhstan' 'Production' 'Kazakhstan'
	'Production.Russia' = Get-EnvironmentMetadata 'Production.Russia' 'Production' 'Russia'
	'Production.Ukraine' = Get-EnvironmentMetadata 'Production.Ukraine' 'Production' 'Ukraine'

	'Test.Russia.Crm.01' = Get-EnvironmentMetadata 'Test.Russia.Crm.01' 'Test' 'Russia' '01'
	'Test.Russia.Crm.02' = Get-EnvironmentMetadata 'Test.Russia.Crm.02' 'Test' 'Russia' '02'
	'Test.Russia.Crm.03' = Get-EnvironmentMetadata 'Test.Russia.Crm.03' 'Test' 'Russia' '03'
	'Test.Russia.Crm.04' = Get-EnvironmentMetadata 'Test.Russia.Crm.04' 'Test' 'Russia' '04'
	'Test.Russia.Crm.05' = Get-EnvironmentMetadata 'Test.Russia.Crm.05' 'Test' 'Russia' '05'
	'Test.Russia.Crm.06' = Get-EnvironmentMetadata 'Test.Russia.Crm.06' 'Test' 'Russia' '06'

	'Test.Russia.07' = Get-EnvironmentMetadata 'Test.Russia.07' 'Test' 'Russia' '07'
	'Test.Russia.08' = Get-EnvironmentMetadata 'Test.Russia.08' 'Test' 'Russia' '08'
	'Test.Russia.09' = Get-EnvironmentMetadata 'Test.Russia.09' 'Test' 'Russia' '09'
	'Test.Russia.10' = Get-EnvironmentMetadata 'Test.Russia.10' 'Test' 'Russia' '10'
	'Test.Russia.11' = Get-EnvironmentMetadata 'Test.Russia.11' 'Test' 'Russia' '11'
	'Test.Russia.12' = Get-EnvironmentMetadata 'Test.Russia.12' 'Test' 'Russia' '12'
	'Test.Russia.13' = Get-EnvironmentMetadata 'Test.Russia.13' 'Test' 'Russia' '13'
	'Test.Russia.14' = Get-EnvironmentMetadata 'Test.Russia.14' 'Test' 'Russia' '14'
	'Test.Russia.15' = Get-EnvironmentMetadata 'Test.Russia.15' 'Test' 'Russia' '15'
	'Test.Russia.16' = Get-EnvironmentMetadata 'Test.Russia.16' 'Test' 'Russia' '16'
	'Test.Russia.17' = Get-EnvironmentMetadata 'Test.Russia.17' 'Test' 'Russia' '17'
	'Test.Russia.18' = Get-EnvironmentMetadata 'Test.Russia.18' 'Test' 'Russia' '18'
	'Test.Russia.19' = Get-EnvironmentMetadata 'Test.Russia.19' 'Test' 'Russia' '19'
	'Test.Russia.20' = Get-EnvironmentMetadata 'Test.Russia.20' 'Test' 'Russia' '20'
	'Test.Russia.21' = Get-EnvironmentMetadata 'Test.Russia.21' 'Test' 'Russia' '21'
	'Test.Russia.22' = Get-EnvironmentMetadata 'Test.Russia.22' 'Test' 'Russia' '22'
	'Test.Russia.23' = Get-EnvironmentMetadata 'Test.Russia.23' 'Test' 'Russia' '23'
	'Test.Russia.24' = Get-EnvironmentMetadata 'Test.Russia.24' 'Test' 'Russia' '24'
	'Test.Russia.25' = Get-EnvironmentMetadata 'Test.Russia.25' 'Test' 'Russia' '25'
	
	'Test.Cyprus.101' = Get-EnvironmentMetadata 'Test.Cyprus.101' 'Test' 'Cyprus' '101'
	'Test.Cyprus.102' = Get-EnvironmentMetadata 'Test.Cyprus.102' 'Test' 'Cyprus' '102'
	'Test.Cyprus.103' = Get-EnvironmentMetadata 'Test.Cyprus.103' 'Test' 'Cyprus' '103'
	'Test.Cyprus.108' = Get-EnvironmentMetadata 'Test.Cyprus.108' 'Test' 'Cyprus' '108'

	'Test.Czech.201' = Get-EnvironmentMetadata 'Test.Czech.201' 'Test' 'Czech' '201'
	'Test.Czech.202' = Get-EnvironmentMetadata 'Test.Czech.202' 'Test' 'Czech' '202'
	'Test.Czech.203' = Get-EnvironmentMetadata 'Test.Czech.203' 'Test' 'Czech' '203'
	'Test.Czech.208' = Get-EnvironmentMetadata 'Test.Czech.208' 'Test' 'Czech' '208'

	'Test.Chile.301' = Get-EnvironmentMetadata 'Test.Chile.301' 'Test' 'Chile' '301'
	'Test.Chile.302' = Get-EnvironmentMetadata 'Test.Chile.302' 'Test' 'Chile' '302'
	'Test.Chile.303' = Get-EnvironmentMetadata 'Test.Chile.303' 'Test' 'Chile' '303'
	'Test.Chile.304' = Get-EnvironmentMetadata 'Test.Chile.304' 'Test' 'Chile' '304'
	'Test.Chile.308' = Get-EnvironmentMetadata 'Test.Chile.308' 'Test' 'Chile' '308'
	'Test.Chile.320' = Get-EnvironmentMetadata 'Test.Chile.320' 'Test' 'Chile' '320'
	
	'Test.Ukraine.401' = Get-EnvironmentMetadata 'Test.Ukraine.401' 'Test' 'Ukraine' '401'
	'Test.Ukraine.402' = Get-EnvironmentMetadata 'Test.Ukraine.402' 'Test' 'Ukraine' '402'
    'Test.Ukraine.403' = Get-EnvironmentMetadata 'Test.Ukraine.403' 'Test' 'Ukraine' '403'
	'Test.Ukraine.404' = Get-EnvironmentMetadata 'Test.Ukraine.404' 'Test' 'Ukraine' '404'
	'Test.Ukraine.408' = Get-EnvironmentMetadata 'Test.Ukraine.408' 'Test' 'Ukraine' '408'

	'Test.Emirates.501' = Get-EnvironmentMetadata 'Test.Emirates.501' 'Test' 'Emirates' '501'
	'Test.Emirates.502' = Get-EnvironmentMetadata 'Test.Emirates.502' 'Test' 'Emirates' '502'
	'Test.Emirates.503' = Get-EnvironmentMetadata 'Test.Emirates.503' 'Test' 'Emirates' '503'
	'Test.Emirates.508' = Get-EnvironmentMetadata 'Test.Emirates.508' 'Test' 'Emirates' '508'

	'Test.Kazakhstan.601' = Get-EnvironmentMetadata 'Test.Kazakhstan.601' 'Test' 'Kazakhstan' '601'
	'Test.Kazakhstan.602' = Get-EnvironmentMetadata 'Test.Kazakhstan.602' 'Test' 'Kazakhstan' '602'
	'Test.Kazakhstan.603' = Get-EnvironmentMetadata 'Test.Kazakhstan.603' 'Test' 'Kazakhstan' '603'
    'Test.Kazakhstan.604' = Get-EnvironmentMetadata 'Test.Kazakhstan.604' 'Test' 'Kazakhstan' '604'
    'Test.Kazakhstan.605' = Get-EnvironmentMetadata 'Test.Kazakhstan.605' 'Test' 'Kazakhstan' '605'
	'Test.Kazakhstan.608' = Get-EnvironmentMetadata 'Test.Kazakhstan.608' 'Test' 'Kazakhstan' '608'
}

Export-ModuleMember -Variable EnvironmentMetadata
Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module "$PSScriptRoot\metadata.dynamics.psm1" -DisableNameChecking
Import-Module "$PSScriptRoot\metadata.web.psm1" -DisableNameChecking

$UpgradeCodes = @{
	'Edu.Chile' = 'CF135E98-2D85-4BC2-8FDE-011BB7F6CF6A'
	'Edu.Cyprus' = '33B02E9D-2860-452F-A2D6-049577715930'
	'Edu.Czech' = 'A3548458-DCEF-58D5-B4B5-2D37232525D1'
	'Edu.Emirates' = '33B09E1D-3600-452F-A2D6-089577715930'
	'Edu.Kazakhstan' = 'F0B14F6F-EC42-4DCA-9F06-E8351C76266C'
	'Edu.Russia' = 'CAE92B57-6FF7-4257-82C9-8C42AE17C106'
	'Edu.Ukraine' = 'B58DF457-52D6-47B9-9F0F-D5C8C8BD2494'
	'Business.Russia' = 'C9D29527-E442-49E0-B25D-AE1F482BD942'
	
	'Int.Chile' = 'B049AF18-7D2C-4A82-A0F4-6DD1BCBBD619' 
	'Int.Cyprus' = 'B7F0FD87-84D0-48F6-AF39-725BF6DEBB8D'
	'Int.Czech' = 'A3548458-DCEF-47D7-B4B5-2D37232525D1' 
	'Int.Emirates'	= '38A16324-ED69-48C7-8240-2DEAAECBD48E'
    'Int.Kazakhstan'	= 'C84704CA-4E9C-4161-822D-C95BCF4F60B4'
	'Int.Russia'	= 'CAE92B67-6FF7-4257-82E9-8C42AE17C106'
	'Int.Ukraine'	= '7A68446A-F389-409E-8740-DE463D8FAAC9'
   
	'Load.Russia' = '7563b08b-ea51-4115-b894-43a136af8801' 
    'Load.Cyprus' = '56CB90B1-66DF-4BFF-B074-EE04C84673BE' 
	'Load.Czech' = 'F24ED3A7-65FF-4099-B350-F62093C21829' 
    'Load.Ukraine' = '61B7783B-A8C2-4F7F-9772-D269F89B1B5C'
	
	'Production.Chile' =  'BB622DAF-9686-4428-9061-42B1F9B643D0'
	'Production.Cyprus' = 'AF99F81A-715D-4687-A170-52D7E88B4DDF'
	'Production.Czech' = 'A3548845-DCEF-49D5-B4B5-2D37238325D1'
	'Production.Emirates' = 'AF99F81A-780D-4687-A170-52D7E88B4DDF'
	'Production.Kazakhstan' = '14F0EE15-5044-4EDB-921D-5B553A625636'
	'Production.Russia' = '7563b07b-ea51-4115-b894-43a136af8801'
	'Production.Ukraine' = '3CC2E766-5DA5-4459-9C8A-0BC5138CFAE4'
	
	'Test.01' = '06CEDA4F-722D-4D1E-A3F9-C36A8DDA026F'
	'Test.02' = '0CEBBB45-C539-4846-875E-6552B5FE937D'
	'Test.03' = '93C848C6-F228-4F89-BCD9-D4E2585F15C2'
	'Test.04' = '5862AB78-F9CC-4992-BC80-B032EE541396'
	'Test.05' = '5C24AB83-0BF3-4DC5-9A44-1CCF7313A583'
    'Test.06' = 'CC6794C1-F798-4532-AE1E-23D2B9CC282D'

	'Test.07' = '6DABC3D9-6DE4-4CA6-89F0-357FEE60C16F'
	'Test.08' = '3BF90797-6F3D-4181-834A-882211A87294'
	'Test.09' = '2C412D6C-7E70-4F08-B0B2-5D643E1FC3D1'
	'Test.10' = '2C412D6C-1E70-4F08-B0B7-5D643E1FC3D1'
	'Test.11' = '0D424C6C-3875-4F6C-82BC-50F769E1F72A'
	'Test.12' = 'C0DEDD27-B2F4-4EE0-98F8-0AFA56533FF6'
	'Test.13' = 'C6ABE531-8B67-4AAF-82A1-B46A8911B21A'
	'Test.14' = 'A3548458-BCEF-49D5-B4B0-2D37238325D7'
	'Test.15' = 'A3548458-DCEF-50D5-B4B5-2D37238325D7'
	'Test.16' = 'A3548458-DCCF-49D5-B4B5-2D37238325D7'
	'Test.17' = 'A3548458-DCEF-49D5-B4B2-2D37238325D7'
	'Test.18' = 'A3548458-DCEF-49D5-B9B5-2D37238325D7'
	'Test.19' = 'A3548458-DCEF-49D5-B4B5-2D97238325D7'
	'Test.20' = 'A3548458-DCEF-49D5-B4B5-2D37238325D1'
	'Test.21' = 'A3548458-BCEF-91D5-B4B0-2D37238325D7'
	'Test.22' = '59B49313-00DE-4BFB-8662-4D8F311B7D4D'
	'Test.23' = 'A3548458-DCEF-49D5-B4B5-2D37232525D1'
	'Test.24' = 'DCE72E82-64A6-4B16-8764-EF883CF6A83E'
	'Test.25' = '10CAF954-6D6D-4B5A-84B9-A272F3E17F5B'
	'Test.88' = 'DA8270F7-CE1E-442D-9AE6-199F20D70997'

	'Test.101' = '59F62505-F286-4681-A4CB-A74024F754C8'
	'Test.102' = '0DF48E0B-48B4-4131-A0DB-CCCD68FDB5D0'
	'Test.103' = 'CABCC420-A334-417E-999A-1A95D7C05392'
	'Test.108' = 'E9DC1036-4647-4439-ABEF-F40BA7D9B241'
				 
	'Test.201' = '082BA5D7-AE8E-43E1-B172-1BFD865AD89E'
	'Test.202' = '919D03F3-2114-4B86-B1E0-840691262B8B'
	'Test.203' = '53C922E7-D58A-4CC3-9848-329519B9DED5'
	'Test.208' = 'B4A8B900-BBCA-4449-8920-572C40933FF7'

	'Test.301' = '01FC3A5A-50BE-43CF-B993-86AF9EBD8678'
	'Test.302' = '1C029286-1862-4CCE-982A-8D0C69519B59'
	'Test.303' = 'CA2615DB-1D7A-416B-A74F-D8F70E771279'
	'Test.304' = 'AAC01554-7216-43DE-926F-770D55A0D357'
	'Test.308' = 'D0E15669-68CF-4E52-8E7F-AAC560FED7A9'
	'Test.320' = '0EE40877-4B9A-4A4D-A77C-650C0586F52B'
	
	'Test.401' = '87BB7A3D-1D98-4202-9583-2C37BFAD8725'
	'Test.402' = '71D6E609-2139-43DB-8B10-2DE10E65A86A'
	'Test.403' = 'E14C7252-680E-43DD-90D3-EAE107389945'
	'Test.404' = 'F3CCA264-8428-48A4-9A28-5B5FF9F90C1E'
	'Test.408' = '8D79DA63-8A78-4273-9A76-749E97A1EA89'
				 
	'Test.501' = 'E1D4AF62-5442-4BBA-AB62-A3B30663B7AD'
	'Test.502' = '9A3555B7-7CF9-4C05-9859-8D36FC83EBF9'
	'Test.503' = '3F7EBD0E-6E9C-418D-ADD9-B6C5CF5C414A'
	'Test.508' = '44B17827-9F9D-49F9-8471-E07E7002BCF0'

	'Test.601' = '5827FA5E-BDAE-4C5F-ABA9-4E54B296D837'
	'Test.602' = '529FD598-3E9A-4032-8FB2-03700CDC81A4'
	'Test.603' = '29E1A1A3-6CF1-45C1-AFE4-CCDF15784EA2'
    'Test.604' = '4381BAA0-4679-420B-AF27-93B7F78C113A'
    'Test.605' = '88A54597-26D4-42BD-808F-46B788FEBC4F'
	'Test.608' = '33F7DF3A-E7A7-4510-94F1-B062B3003F56'
}

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

function Get-AutoStartMetadata ($EnvType){
	return @{ 'AutoStart' = $true}
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

function Get-UpgradeCodeMetadata($EnvName){

	if (!$UpgradeCodes.ContainsKey($EnvName)){
		throw "Can't found UpgradeCode for EnvName '$EnvName'"
	}

	return @{'UpgradeCode' = $UpgradeCodes[$EnvName] }
}

function Get-TaskServiceMetadata ($EnvName, $EnvType, $Country, $Index) {

	$metadata = @{}
	$metadata += Get-TargetHostsMetadata $EnvType $Country $Index
	$metadata += Get-AutoStartMetadata $EnvType
	$metadata += Get-QuartzConfigMetadata $EnvType $Country $Index
	$metadata += Get-UpgradeCodeMetadata $EnvName
	
	return $metadata
}

Export-ModuleMember -Function Get-TaskServiceMetadata
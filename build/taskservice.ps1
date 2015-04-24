Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module "$BuildToolsRoot\modules\metadata.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\winrm.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\winservice.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\entrypoint.psm1" -DisableNameChecking

Properties { $OptionTaskService = $true }

Task Build-TaskService -Precondition { $OptionTaskService } -Depends Update-AssemblyInfo {

	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.TaskService'
	$entryPointMetadata = Get-Metadata '2Gis.Erm.TaskService'

	Build-WinService $projectFileName $entryPointMetadata
}

Task Deploy-TaskService -Depends Import-WinServiceModule, Take-TaskServiceOffline -Precondition { $OptionTaskService } {

	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.TaskService'
	$entryPointMetadata = Get-Metadata '2Gis.Erm.TaskService'

	Deploy-WinService $projectFileName $entryPointMetadata
}

Task Take-TaskServiceOffline -Depends Import-WinServiceModule -Precondition { $OptionTaskService } {

	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.TaskService'
	$entryPointMetadata = Get-Metadata '2Gis.Erm.TaskService'

	Take-WinServiceOffline $projectFileName $entryPointMetadata
}

Task Import-WinServiceModule {

	$module = Get-Module 'winservice'

	$entryPointMetadata = Get-Metadata '2Gis.Erm.TaskService'
	foreach($targetHost in $entryPointMetadata.TargetHosts){
		$session = Get-CachedSession $targetHost
		Import-ModuleToSession $session $module
	}
}
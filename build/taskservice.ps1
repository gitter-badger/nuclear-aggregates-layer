Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module "$BuildToolsRoot\modules\msbuild.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\entrypoint.psm1" -DisableNameChecking

Properties { $OptionTaskService = $true }

Task Build-TaskService -Precondition { $OptionTaskService } -Depends Update-AssemblyInfo {

	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.TaskService'
	Build-WinService $projectFileName '2Gis.Erm.TaskService'
}

Task Deploy-TaskService -Depends Import-WinServiceModule, Take-TaskServiceOffline -Precondition { $OptionTaskService } {
	Deploy-WinService '2Gis.Erm.TaskService'
}

Task Take-TaskServiceOffline -Depends Import-WinServiceModule -Precondition { $OptionTaskService } {
	Take-WinServiceOffline '2Gis.Erm.TaskService'
}

Task Import-WinServiceModule {
	Load-WinServiceModule '2Gis.Erm.TaskService'
}
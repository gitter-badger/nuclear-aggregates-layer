Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module .\modules\transform.psm1 -DisableNameChecking
Import-Module .\modules\msbuild.psm1 -DisableNameChecking
Import-Module .\modules\msdeploy.psm1 -DisableNameChecking
Import-Module .\modules\metadata.psm1 -DisableNameChecking

Properties{ $OptionTaskService=$false }
Task Build-TaskService -Precondition { return $OptionTaskService } -Depends Update-AssemblyInfo {

	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.TaskService.Installer' '.wixproj'
	
	$environmentName = $global:Context.EnvironmentName
	$version = $global:Context.Version.ToString(3)
	$compilerArguments = @("-dProductVersion=$version", "-dEnvironmentName=$environmentName") 

	# hack: в production имя сервиса не содержит версию
	if ($environmentName -match 'Production'){
		$compilerArguments += "-dServiceName=$environmentName"
	}
	$compilerOptionsStr = [string]::Join(' ', $compilerArguments)

	Transform-TaskServiceConfigs
	try {
		Invoke-MSBuild @(
		"""$projectFileName"""
		"""/p:CompilerAdditionalOptions=$compilerOptionsStr"""
		)
	}
	finally {
		Restore-TaskServiceConfigs
	}

	$convensionalArtifactName = Join-Path (Split-Path $projectFileName) 'bin\x64\Release\2Gis.Erm.TaskService.Installer.msi'
	Publish-Artifacts $convensionalArtifactName
}

function Transform-TaskServiceConfigs {

	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.TaskService'
	Transform-Log4NetConfig $projectFileName
	Transform-AppConfig $projectFileName
}

function Restore-TaskServiceConfigs {

	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.TaskService'
	Restore-Log4NetConfig $projectFileName
	Restore-AppConfig $projectFileName
}

Task Deploy-TaskService -Precondition { return $OptionTaskService } -Depends Build-TaskService {
	
	$remoteScriptBlock = {
		param($artifactFileName)
		
		Set-StrictMode -Version 3.0
		$ErrorActionPreference = 'Stop'
		#------------------------------

		$artifactName = "C:\Windows\Temp\$artifactFileName"
		cmd.exe /c msiexec.exe -i $artifactName -quiet -norestart
        if ($LastExitCode -ne 0) {
          throw "Command failed with exit code $LastExitCode"
        }
		Remove-Item $artifactName -Force
	}
	
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.TaskService.Installer' '.wixproj'
	$artifactName = Get-Artifacts '' '2Gis.Erm.TaskService.Installer.msi'
	$artifactFileName = Split-Path $artifactName -Leaf

	$entryPointMetadata = Get-EntryPointMetadata '2Gis.Erm.TaskService.Installer'
	foreach($targetHost in $entryPointMetadata.TargetHosts){
		# copy to remote host
		Invoke-MSDeploy `
		-Source "filePath=""$artifactName""" `
		-Dest "filePath=""C:\Windows\Temp\$artifactFileName""" `
		-HostName $targetHost
		
		# install on remote host
		Invoke-Command `
		-ComputerName $targetHost `
		-Args $artifactFileName `
		-ScriptBlock $remoteScriptBlock
	}
}

Task Take-TaskServiceOffline -Precondition { return $OptionTaskService } {

	$remoteScriptBlock = {
		param($EnvironmentName)
		
		Set-StrictMode -Version 3.0
		$ErrorActionPreference = 'Stop'
		#------------------------------

		$service = Get-WmiObject Win32_Service -Filter "(name='$EnvironmentName' or name like '%$($EnvironmentName)[_]%') and startmode!='disabled' and state='running'"
		if ($service -eq $null){
			# fresh install, nothing to stop
			return
		}
		if ($service -is [System.Array] -and $service.Length -ne 1){
			throw "Found more than one service with name similar to $EnvironmentName"
		}
		$serviceResult = $service.stopService()
		if ($serviceResult.returnvalue -ne 0){
			throw "Can't stop service $EnvironmentName, error code $($serviceResult.returnvalue)"
		}
	}

	$environmentName = $global:Context.EnvironmentName

	$entryPointMetadata = Get-EntryPointMetadata '2Gis.Erm.TaskService.Installer'
	foreach($targetHost in $entryPointMetadata.TargetHosts){

		# stop service on remote host
		Invoke-Command `
		-ComputerName $targetHost `
		-Args $environmentName `
		-ScriptBlock $remoteScriptBlock
        }
}

Task Take-TaskServiceOnline -Precondition { return $OptionTaskService -and $global:Context.EnvironmentName -notmatch 'Production' } {

	$remoteScriptBlock = {
		param($EnvironmentName)

		Set-StrictMode -Version 3.0
		$ErrorActionPreference = 'Stop'
		#------------------------------

			$service = Get-WmiObject Win32_Service -Filter "(name='$EnvironmentName' or name like '%$($EnvironmentName)[_]%') and startmode!='disabled' and state='stopped'"
			if ($service -eq $null){
				throw "Cannot found just installed service $EnvironmentName"
			}
			if ($service -is [System.Array] -and $service.Length -ne 1){
				throw "Found more than one service $EnvironmentName"
			}
		$serviceResult = $service.startService()
		if ($serviceResult.returnvalue -ne 0){
			throw "Can't start service $EnvironmentName, error code $($serviceResult.returnvalue)"
        }
	}
	
	$environmentName = $global:Context.EnvironmentName
	
	$entryPointMetadata = Get-EntryPointMetadata '2Gis.Erm.TaskService.Installer'
	foreach($targetHost in $entryPointMetadata.TargetHosts){
		
		# start service on remote host
		Invoke-Command `
		-ComputerName $targetHost `
		-Args $environmentName `
		-ScriptBlock $remoteScriptBlock
	}
}
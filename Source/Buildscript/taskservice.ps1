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
	
	$publishProfileName = $global:Context.EnvironmentName
	$version = $global:Context.Version.ToString(3)
	$constantsOverrides = "ProductVersion=$version;PublishProfileName=$publishProfileName"

	Transform-TaskServiceConfigs
	try {
		Invoke-MSBuild @(
		"""$projectFileName"""
		"/p:DefineConstantsOverrides=""$constantsOverrides"""
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
	
	# script block will be called on remote host
	$remoteScriptBlock = {
		param($artifactFileName, $EnvironmentName)
		
		Set-StrictMode -Version 3.0
		$ErrorActionPreference = 'Stop'
		#------------------------------

		$service = Get-WmiObject Win32_Service -Filter "name like '%$EnvironmentName%' and startmode!='disabled' and state='running'"
		if ($service -ne $null){
			if ($service -is [System.Array] -and $service.Length -ne 1){
				throw "Found more than one service with name similar to $EnvironmentName"
			}
			$service.stopService()
		}

		$artifactName = "C:\Windows\Temp\$artifactFileName"
		cmd.exe /c msiexec.exe -i $artifactName -quiet -norestart
        if ($LastExitCode -ne 0) {
          throw "Command failed with exit code $LastExitCode"
        }
		Remove-Item $artifactName -Force

		# hack for Test.08
        if ($EnvironmentName -eq 'Test.08'){
			$service = Get-WmiObject Win32_Service -Filter "name like '%$EnvironmentName%' and startmode!='disabled' and state='stopped'"
			if ($service -eq $null){
				throw "Cannot found just installed service $EnvironmentName"
			}
			if ($service -is [System.Array] -and $service.Length -ne 1){
				throw "Found more than one service $EnvironmentName"
			}
			$service.startService()
        }
	}
	
	$environmentName = $global:Context.EnvironmentName
	
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.TaskService.Installer' '.wixproj'
	$artifactName = Get-Artifacts '' '2Gis.Erm.TaskService.Installer.msi'
	$artifactFileName = Split-Path $artifactName -Leaf

	$entryPointMetadata = Get-EntryPointMetadata '2Gis.Erm.TaskService.Installer'
	
	foreach($targetHost in $EntryPointMetadata.TargetHosts){
		# copy to remote host
		Invoke-MSDeploy `
		-Source "filePath=""$artifactName""" `
		-Dest "filePath=""C:\Windows\Temp\$artifactFileName""" `
		-HostName $targetHost
		
		# install on remote host
		Invoke-Command `
		-ComputerName $targetHost `
		-Args $artifactFileName, $environmentName `
		-ScriptBlock $remoteScriptBlock
	}
}
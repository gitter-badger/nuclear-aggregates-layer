Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module .\modules\transform.psm1 -DisableNameChecking
Import-Module .\modules\msbuild.psm1 -DisableNameChecking
Import-Module .\modules\msdeploy.psm1 -DisableNameChecking

Properties{ $OptionTaskService=$false }
Task Build-TaskService -Precondition { return $OptionTaskService } -Depends Update-AssemblyInfo {

	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.TaskService.Installer' '.wixproj'
	
	$publishProfileName = $global:Context.EnvironmentName
	$version = $global:Context.Version.ToString(3)
	$constantsOverrides = "ProductVersion=$version;PublishProfileName=$publishProfileName"

	try {
		Transform-TaskServiceConfigs

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
	$hostName = $global:Context.TargetHostName
	$publishProfileName = $global:Context.EnvironmentName
	
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.TaskService.Installer' '.wixproj'
	$artifactName = Get-Artifacts '' '2Gis.Erm.TaskService.Installer.msi'
	$artifactFileName = Split-Path $artifactName -Leaf
	
	# copy to remote host
	Sync-MSDeploy `
	-Source "filePath=""$artifactName""" `
	-Dest "filePath=""C:\Windows\Temp\$artifactFileName""" `
	-HostName $hostName
	
	# install on remote host
	Invoke-Command `
	-ComputerName $hostName `
	-Args $artifactFileName, $publishProfileName `
	-ScriptBlock {
		param($artifactFileName, $PublishProfileName)
		
		Set-StrictMode -Version 3.0
		$ErrorActionPreference = 'Stop'
		#------------------------------

        $service = Get-Service -Include ('*' + $PublishProfileName + '*')
        if ($service -ne $null -and $service.Status -eq "Running"){
          $service.Stop()
        }

		$artifactName = "C:\Windows\Temp\$artifactFileName"
		cmd.exe /c msiexec.exe -i $artifactName -quiet -norestart
        if ($LastExitCode -ne 0) {
          throw "Command failed with exit code $LastExitCode"
        }
		Remove-Item $artifactName -Force

		# hack
		$service = Get-Service -Include $PublishProfileName
        if ($service -ne $null -and $PublishProfileName -eq 'Test.08'){
          $service.Start()
        }
	}
}
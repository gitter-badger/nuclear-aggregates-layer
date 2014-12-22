Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module .\modules\transform.psm1 -DisableNameChecking
Import-Module .\modules\msbuild.psm1 -DisableNameChecking
Import-Module .\modules\msdeploy.psm1 -DisableNameChecking
Import-Module .\modules\metadata.psm1 -DisableNameChecking
Import-Module .\modules\versioning.psm1 -DisableNameChecking

Properties{ $OptionTaskService=$false }
Task Build-TaskService -Precondition { return $OptionTaskService } -Depends Update-AssemblyInfo {

	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.TaskService.Installer' '.wixproj'
	
	$compilerOptions = @()
	$variablesIncludeFileName = Create-VariablesIncludeFile
	$compilerOptions += "-dVariablesIncludeFileName=$variablesIncludeFileName"
	$quartzIncludeFileName = Create-QuartzIncludeFile
	$compilerOptions += "-dQuartzIncludeFileName=$quartzIncludeFileName"
	$compilerOptionsStr = [string]::Join(' ', $compilerOptions)
	
	$linkerOptions = @()
	$linkerOptions += "-b PackagesDir=$($global:Context.Dir.Solution)\packages"
	$linkerOptionsStr = [string]::Join(' ', $linkerOptions)

	Transform-TaskServiceConfigs
	try {
		Invoke-MSBuild @(
		"""$projectFileName"""
		"""/p:CompilerAdditionalOptions=$compilerOptionsStr"""
		"""/p:LinkerAdditionalOptions=$linkerOptionsStr"""
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
	$projectDir = Split-Path $ProjectFileName

	$configFileName1 = Join-Path $projectDir 'log4net.config'
	$content1 = Transform-Config $configFileName1
	Backup-Config $configFileName1 $content1

	$configFileName2 = Join-Path $projectDir 'app.config'
	$content2 = Transform-Config $configFileName2
	Backup-Config $configFileName2 $content2
}

function Restore-TaskServiceConfigs {
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.TaskService'
	$projectDir = Split-Path $ProjectFileName

	$configFileName1 = Join-Path $projectDir 'log4net.config'
	Restore-Config $configFileName1
	
	$configFileName2 = Join-Path $projectDir 'app.config'
	Restore-Config $configFileName2
}

Task Deploy-TaskService -Precondition { return $OptionTaskService } -Depends Build-TaskService {
	
	$remoteScriptBlock = {
		param($artifactFileName)
		
		Set-StrictMode -Version Latest
		$ErrorActionPreference = 'Stop'
		#------------------------------
		
		$timeout = '00:01:00'
		$mutex = New-Object System.Threading.Mutex($false, 'Global\ErmTaskService')
		$hasHandle = $false
		
		try{
			$hasHandle = $mutex.WaitOne($timeout)
			if(!$hasHandle){
				throw "Can't install task service, msiexec is locked by another installation"
			}
			
			$artifactName = "C:\Windows\Temp\$artifactFileName"
			cmd.exe /c msiexec.exe -i $artifactName -quiet -norestart -lv "C:\Windows\Temp\2Gis.Erm.TaskService.Installer.log"
	        if ($LastExitCode -ne 0) {
	          throw "Command failed with exit code $LastExitCode"
	        }
			Remove-Item $artifactName -Force
		}
		finally {
			if ($hasHandle){
				$mutex.ReleaseMutex()
			}
		}
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
		
		Set-StrictMode -Version Latest
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

Task Take-TaskServiceOnline -Precondition { return $OptionTaskService -and (Get-EntryPointMetadata '2Gis.Erm.TaskService.Installer').AutoStart } {

	$remoteScriptBlock = {
		param($EnvironmentName)

		Set-StrictMode -Version Latest
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

function Create-VariablesIncludeFile {

	$version = Get-Version
	$environmentName = $global:Context.EnvironmentName
	
	$entryPointMetadata = Get-EntryPointMetadata '2Gis.Erm.TaskService.Installer'
	$upgradeCode = $entryPointMetadata['UpgradeCode']
	
	$autoStart = $entryPointMetadata['AutoStart']
	if ($autoStart){
		$serviceStartType = 'auto'
	} else {
		$serviceStartType = 'demand'
	}

	$content = @"
<?xml version="1.0" encoding="utf-8"?>
<Include>
	<?define ProductVersion = "$($version.NumericVersion)" ?>
	<?define SemanticVersion = "$($version.SemanticVersion)" ?>
	<?define EnvironmentName = "$environmentName" ?>
	<?define UpgradeCode = "$upgradeCode" ?>
	<?define ServiceStartType = "$serviceStartType" ?>
</Include>
"@

	$variablesIncludeFileName = Join-Path $global:Context.Dir.Temp 'Variables.wxi' 
	Out-File $variablesIncludeFileName -InputObject $content -Encoding UTF8 -Force
	return $variablesIncludeFileName
}

function Create-QuartzIncludeFile {

	$quartzConfigs = @()

	$baseDir = Join-Path $global:Context.Dir.Solution 'Environments'
	$entryPointMetadata = Get-EntryPointMetadata '2Gis.Erm.TaskService.Installer'
	foreach($quartzConfig in $entryPointMetadata.QuartzConfigs){
		
		$quartzConfigFileName = Join-Path $baseDir $quartzConfig
		
		if (Test-Path $quartzConfigFileName){
			$quartzConfigs += $quartzConfigFileName
		} elseif ($entryPointMetadata.AlterQuartzConfigs.Count -gt 0) {
			foreach($alterQuartzConfig in $entryPointMetadata.AlterQuartzConfigs){
				$quartzConfigs += Join-Path $baseDir $alterQuartzConfig
			}
			break
		} else {
			throw "Can't find config $quartzConfigFileName "
		}
	}
	
	# если нашли именной quartz.config файл то плюём на всё и используем только его
	$environmentName = $global:Context.EnvironmentName
	$environmentQuartzConfig = Join-Path $baseDir "quartz.$environmentName.config"
	if (Test-Path $environmentQuartzConfig){
		if ($quartzConfigs -notcontains $environmentQuartzConfig){
			$quartzConfigs = @($environmentQuartzConfig)
		}
	}
	
	[xml]$xml = @"
<?xml version="1.0" encoding="utf-8"?>
<Include>
</Include>
"@
	$includeNode = $xml['Include']

	
	foreach ($quartzConfig in $quartzConfigs){
	
		$fileNode = $xml.CreateElement('File')
		$fileNode.SetAttribute('Name', [System.IO.Path]::GetFileName($quartzConfig))
		$fileNode.SetAttribute('Source', $quartzConfig)
		
		$componentNode = $xml.CreateElement('Component')
		[void]$componentNode.AppendChild($fileNode)
		[void]$includeNode.AppendChild($componentNode)
	}

	$quartzIncludeFileName = Join-Path $global:Context.Dir.Temp 'Quartz.wxi'
	$xml.Save($quartzIncludeFileName)
	return $quartzIncludeFileName
}
Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module "$BuildToolsRoot\modules\transform.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\msbuild.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\msdeploy.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\metadata.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\versioning.psm1" -DisableNameChecking

Properties { $OptionTaskService = $true }

Task Build-TaskService -Precondition { $OptionTaskService } -Depends Update-AssemblyInfo {

	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.TaskService'

	$projectDir = Split-Path $ProjectFileName

	$configFileName1 = Join-Path $projectDir 'log4net.config'
	$customXml1 = Transform-Config $configFileName1
	$configFileName2 = Join-Path $projectDir 'app.config'
	$customXml2 = Transform-Config $configFileName2
	$configFileName3 = Join-Path $projectDir 'quartz.config'
	$customXml3 = Transform-QuartzConfig $configFileName3
	$configXmls = @($customXml1, $customXml2, $customXml3)
	
	$buildFileName = Create-BuildFile $ProjectFileName -Properties @{ 'AppConfig' = 'app.transformed.config' } -CustomXmls $configXmls
	Invoke-MSBuild $buildFileName
	
	$convensionalArtifactName = Join-Path (Split-Path $projectFileName) 'bin\Release'
	Publish-Artifacts $convensionalArtifactName 'Task Service'
}

# TODO: обобщить с Transform-Config в модуле transform.ps1
function Transform-QuartzConfig ($ConfigFileName){

	$quartzConfigs = Get-QuartzConfigs
	$newConfigFileName = [string]::Join(';', $quartzConfigs)

	$fileName = [System.IO.Path]::GetFileName($ConfigFileName)
	$targetName = "TransformConfig-$(Get-Random)"

	$customXml = @"
<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
	<!-- Трансформация $fileName -->
	<PropertyGroup>
		<CoreBuildDependsOn>
			$targetName;
			`$(CoreBuildDependsOn)
		</CoreBuildDependsOn>
	</PropertyGroup>
	<Target Name="$targetName">
		<ItemGroup Condition="Exists('$fileName')">
		
			<!-- primary -->
		    <NoneToRemove Include="@(None)" Condition=" '%(None.Identity)' == '$fileName' " />
		    <None Remove="@(NoneToRemove)" />
		    <None Include="$newConfigFileName" Condition=" '@(NoneToRemove)' != '' ">
				<!-- <Link>$fileName</Link> -->
				<CopyToOutputDirectory>Always</CopyToOutputDirectory>
			</None>

			<!-- secondary -->
		    <ContentToRemove Include="@(Content)" Condition=" '%(Content.Identity)' == '$fileName' " />
		    <Content Remove="@(ContentToRemove)" />
		    <Content Include="$newConfigFileName" Condition=" '@(ContentToRemove)' != '' ">
				<!-- <Link>$fileName</Link> -->
			</Content>

		</ItemGroup>
	</Target>
</Project>
"@

	return $customXml
}

function Get-QuartzConfigs {

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
	
	return $quartzConfigs
}

Task Deploy-TaskService -Precondition { $OptionTaskService } {
	
	$remoteScriptBlock = {
		param($artifactFileName)
		
		Set-StrictMode -Version Latest
		$ErrorActionPreference = 'Stop'
		#------------------------------

		$timeout = [timespan]'00:01:00'
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
	
	$artifactName = Get-Artifacts '' '2Gis.Erm.TaskService.Installer.msi'
	$artifactFileName = [System.IO.Path]::GetFileName($artifactName)

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

Task Take-TaskServiceOffline -Precondition { $OptionTaskService } {

	$remoteScriptBlock = {
		param($EnvironmentName)
		
		Set-StrictMode -Version Latest
		$ErrorActionPreference = 'Stop'
		#------------------------------

		$service = Get-WmiObject Win32_Service -Filter "(name='$EnvironmentName' or name like '%$($EnvironmentName)[_]%') and startmode!='disabled'"
		if ($service -is [System.Array] -and $service.Length -ne 1){
			throw "Found more than one service with name similar to $EnvironmentName"
		}
		if ($service -eq $null){
			# fresh install, nothing to stop
			return
		}

		if ($service.state -eq 'running'){
			$serviceResult = $service.stopService()
			if ($serviceResult.returnvalue -ne 0){
				throw "Can't stop service $EnvironmentName, error code $($serviceResult.returnvalue)"
			}
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

Task Take-TaskServiceOnline -Precondition { $OptionTaskService -and (Get-EntryPointMetadata '2Gis.Erm.TaskService.Installer').AutoStart } {

	$remoteScriptBlock = {
		param($EnvironmentName)

		Set-StrictMode -Version Latest
		$ErrorActionPreference = 'Stop'
		#------------------------------

		$WaitAttempts = 5
		for ($i = 0; $i -lt $WaitAttempts; $i++){
		
			$service = Get-WmiObject Win32_Service -Filter "(name='$EnvironmentName' or name like '%$($EnvironmentName)[_]%') and startmode!='disabled'"
			if ($service -is [System.Array] -and $service.Length -ne 1){
				throw "Found more than one service $EnvironmentName"
			}

			if ($service -ne $null){
				break
			} else {
				Write-Host "Cannot found just installed service '$EnvironmentName' (attempt $($i + 1))"
				Start-Sleep -Second 5
			}
		}
		
		if ($i -eq $WaitAttempts){
			throw "Cannot found just installed service '$EnvironmentName' after $WaitAttempts attempts"
		}

		if ($service.state -eq 'stopped'){
			$serviceResult = $service.startService()
			if ($serviceResult.returnvalue -ne 0){
				throw "Can't start service $EnvironmentName, error code $($serviceResult.returnvalue)"
	        }
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
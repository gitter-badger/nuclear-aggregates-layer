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
	Publish-Artifacts $convensionalArtifactName '2GIS ERM Task Service'
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
	$entryPointMetadata = Get-EntryPointMetadata '2Gis.Erm.TaskService'
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

Task Deploy-TaskService -Depends Take-TaskServiceOffline -Precondition { $OptionTaskService } {

	$removeServiceScriptBlock = {
		param($environmentName)
		
		Set-StrictMode -Version Latest
		$ErrorActionPreference = 'Stop'
		#------------------------------

		$service = Get-WmiObject Win32_Service -Filter "name like '%$environmentName[-_]%' and startmode!='disabled'"
		if ($service -is [System.Array] -and $service.Length -ne 1){
			throw "Found more than one service with name similar to $environmentName"
		}
		if ($service -eq $null){
			# fresh install, nothing to remove
			return
		}

		if ($service.state -ne 'stopped'){
			throw "Can't delete service $($service.name), service is running"
		}

		$serviceResult = $service.delete()
		if ($serviceResult.returnvalue -ne 0){
			throw "Can't delete service $($service.Name), error code $($serviceResult.returnvalue)"
		}
		
		$serviceDir = Split-Path $service.pathname.Trim('"')
		rd $serviceDir -Recurse -Force | Out-Null
	}

	$createServiceScriptBlock = {
		param($name, $displayName, $servicePath)
		
		Set-StrictMode -Version Latest
		$ErrorActionPreference = 'Stop'
		#------------------------------

		$emptyPassword = New-Object System.Security.SecureString
		$credential = New-Object System.Management.Automation.PSCredential('NT AUTHORITY\NETWORK SERVICE', $emptyPassword)

		$service = New-Service `
			-Name $name `
			-DisplayName $displayName `
			-BinaryPathName $servicePath `
			-StartupType 'Automatic' `
			-Credential $credential
	}
	
	$sourceDirPath = Get-Artifacts '2GIS ERM Task Service'
	$serviceExeName = Get-ChildItem $sourceDirPath -Filter '*.exe'
	
	$destDirName = Split-Path $sourceDirPath -Leaf
	$destDirPath = Join-Path 'C:\Program Files' $destDirName
	$destServicePath = Join-Path $destDirPath $serviceExeName

	$environmentName = $global:Context.EnvironmentName
	$semanticVersion = (Get-Version).SemanticVersion

	$serviceName = "ERM-$environmentName-$semanticVersion"
	$serviceDisplayName = $destDirName

	$entryPointMetadata = Get-EntryPointMetadata '2Gis.Erm.TaskService'
	foreach($targetHost in $entryPointMetadata.TargetHosts){

		# remove from remote host
		Invoke-Command `
		-ComputerName $targetHost `
		-Args $environmentName `
		-ScriptBlock $removeServiceScriptBlock

		# copy to remote host
		Invoke-MSDeploy `
		-Source "dirPath=""$sourceDirPath""" `
		-Dest "dirPath=""$destDirPath""" `
		-HostName $targetHost
		
		# install on remote host
		Invoke-Command `
		-ComputerName $targetHost `
		-Args $serviceName, $serviceDisplayName, $destServicePath `
		-ScriptBlock $createServiceScriptBlock
	}
}

Task Take-TaskServiceOffline -Precondition { $OptionTaskService } {

	$remoteScriptBlock = {
		param($EnvironmentName)
		
		Set-StrictMode -Version Latest
		$ErrorActionPreference = 'Stop'
		#------------------------------

		$service = Get-WmiObject Win32_Service -Filter "name like '%$EnvironmentName[-_]%' and startmode!='disabled'"
		if ($service -is [System.Array] -and $service.Length -ne 1){
			throw "Found more than one service with name similar to $EnvironmentName"
		}
		if ($service -eq $null){
			# fresh install, nothing to stop
			return
		}

		if ($service.state -eq 'stopped'){
			# already stopped
			return
		}

		$serviceController = Get-Service $service.name
		$timeout = New-TimeSpan -Seconds 30
		try{
			$serviceController.Stop()
        	$serviceController.WaitForStatus('Stopped', $timeout)
		}
		catch [System.ServiceProcess.TimeoutException] {

			Write-Host "Can't stop service $($service.name), timeout $timeout, killing process by pid $($service.ProcessId)"
			Stop-Process -Id $service.ProcessId -Force
		}
	}

	$environmentName = $global:Context.EnvironmentName

	$entryPointMetadata = Get-EntryPointMetadata '2Gis.Erm.TaskService'
	foreach($targetHost in $entryPointMetadata.TargetHosts){

		# stop service on remote host
		Invoke-Command `
		-ComputerName $targetHost `
		-Args $environmentName `
		-ScriptBlock $remoteScriptBlock
	}
}

Task Take-TaskServiceOnline -Precondition { $OptionTaskService } {

	$remoteScriptBlock = {
		param($EnvironmentName)

		Set-StrictMode -Version Latest
		$ErrorActionPreference = 'Stop'
		#------------------------------

		$WaitAttempts = 5
		for ($i = 0; $i -lt $WaitAttempts; $i++){
		
			$service = Get-WmiObject Win32_Service -Filter "name like '%$EnvironmentName[-_]%' and startmode!='disabled'"
			if ($service -is [System.Array] -and $service.Length -ne 1){
				throw "Found more than one service $EnvironmentName"
			}

			if ($service -ne $null){
				break
			} else {
				Write-Host "Cannot found just installed service with name similar to $EnvironmentName (attempt $($i + 1))"
				Start-Sleep -Second 5
			}
		}
		
		if ($i -eq $WaitAttempts){
			throw "Cannot found just installed service with name similar to $EnvironmentName after $WaitAttempts attempts"
		}

		if ($service.state -eq 'stopped'){
			$serviceResult = $service.startService()
			if ($serviceResult.returnvalue -ne 0){
				throw "Can't start service $($service.name), error code $($serviceResult.returnvalue)"
	        }
		}
	}
	
	$environmentName = $global:Context.EnvironmentName
	$entryPointMetadata = Get-EntryPointMetadata '2Gis.Erm.TaskService'
	foreach($targetHost in $entryPointMetadata.TargetHosts){
		
		# start service on remote host
		Invoke-Command `
		-ComputerName $targetHost `
		-Args $environmentName `
		-ScriptBlock $remoteScriptBlock
	}
}
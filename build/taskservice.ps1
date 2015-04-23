Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module "$BuildToolsRoot\modules\transform.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\msbuild.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\msdeploy.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\metadata.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\versioning.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\winrm.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\winservice.psm1" -DisableNameChecking

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

Task Deploy-TaskService -Depends Import-WinServiceModule, Take-TaskServiceOffline, Remove-TaskService -Precondition { $OptionTaskService } {

	$sourceDirPath = Get-Artifacts '2GIS ERM Task Service'
	$serviceExeName = Get-ChildItem $sourceDirPath -Filter '*.exe'
	
	$environmentName = $global:Context.EnvironmentName
	
	$destDirName = "2GIS ERM Task Service-$environmentName"
	$destDirPath = "%ProgramFiles%\$destDirName"
	$destServicePath = "`"`${Env:ProgramFiles}\$destDirName\$serviceExeName`""
	
	$semanticVersion = (Get-Version).SemanticVersion
	$serviceName = "ERM-$environmentName-$semanticVersion"
	$serviceDisplayName = "2GIS ERM Task Service-$environmentName-$semanticVersion"

	$entryPointMetadata = Get-EntryPointMetadata '2Gis.Erm.TaskService'
	foreach($targetHost in $entryPointMetadata.TargetHosts){

		# copy to remote host
		Invoke-MSDeploy `
		-Source "dirPath=""$sourceDirPath""" `
		-Dest "dirPath=""$destDirPath""" `
		-HostName $targetHost

		$session = Get-CachedSession $targetHost
		Invoke-Command $session { Create-WindowsService $using:serviceName $using:serviceDisplayName $using:destServicePath }
	}
}

Task Remove-TaskService -Depends Import-WinServiceModule -Precondition { $OptionTaskService } {

	$serviceName = $global:Context.EnvironmentName

	$entryPointMetadata = Get-EntryPointMetadata '2Gis.Erm.TaskService'
	foreach($targetHost in $entryPointMetadata.TargetHosts){
		
		$session = Get-CachedSession $targetHost
		Invoke-Command $session { Delete-WindowsService $using:serviceName }
	}
}

Task Take-TaskServiceOffline -Depends Import-WinServiceModule -Precondition { $OptionTaskService } {

	$serviceName = $global:Context.EnvironmentName

	$entryPointMetadata = Get-EntryPointMetadata '2Gis.Erm.TaskService'
	foreach($targetHost in $entryPointMetadata.TargetHosts){
		
		$session = Get-CachedSession $targetHost
		Invoke-Command $session { Stop-WindowsService $using:serviceName }
	}
}

Task Take-TaskServiceOnline -Depends Import-WinServiceModule -Precondition { $OptionTaskService } {

	$serviceName = $global:Context.EnvironmentName

	$entryPointMetadata = Get-EntryPointMetadata '2Gis.Erm.TaskService'
	foreach($targetHost in $entryPointMetadata.TargetHosts){
		
		$session = Get-CachedSession $targetHost
		Invoke-Command $session { Start-WindowsService $using:serviceName }
	}
}

Task Import-WinServiceModule {

	$module = Get-Module 'winservice'

	$entryPointMetadata = Get-EntryPointMetadata '2Gis.Erm.TaskService'
	foreach($targetHost in $entryPointMetadata.TargetHosts){
		$session = Get-CachedSession $targetHost
		Import-ModuleToSession $session $module
	}
}
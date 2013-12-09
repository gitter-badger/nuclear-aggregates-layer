Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module .\modules\msbuild.psm1 -DisableNameChecking
Import-Module .\modules\msdeploy.psm1 -DisableNameChecking

Properties{ $OptionTaskService=$false }
Task Build-TaskService -Precondition { return $OptionTaskService } -Depends Update-AssemblyInfo {
	
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.TaskService.Installer' '.wixproj'

	$outDir = $global:Context.Dir.Temp
	$publishProfileName = $global:Context.EnvironmentName
	$version = $global:Context.Version.ToString(3)
	$constantsOverrides = "ProductVersion=$version;PublishProfileName=$publishProfileName"

	Invoke-MSBuild @(
	"""$projectFileName"""
	"/p:OutDir=$outDir\"
	"/p:OutputName=dummy"
	"/p:PublishProfileName=$publishProfileName"
	"/p:DefineConstantsOverrides=""$constantsOverrides"""
	)
	
	# copy dummy to artifacts
	$dummyFileName = Join-Path $outDir 'dummy.msi'
	$packageFileName = Get-PackageFileName $projectFileName 'msi'
	Copy-Item $dummyFileName $packageFileName
	
	Write-Output "##teamcity[publishArtifacts '$packageFileName => Task service']"
}

Task Deploy-TaskService -Precondition { return $OptionTaskService } -Depends Build-TaskService {
	$hostName = $global:Context.TargetHostName
	$publishProfileName = $global:Context.EnvironmentName
	
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.TaskService.Installer' '.wixproj'
	$packageFileName = Get-PackageFileName $projectFileName 'msi'
	$packageName = Split-Path $packageFileName -Leaf
	
	# copy to remote host
	Sync-MSDeploy `
	-Source "filePath=""$packageFileName""" `
	-Dest "filePath=""C:\Windows\Temp\$packageName""" `
	-HostName $hostName
	
	# install on remote host
	Invoke-Command `
	-ComputerName $hostName `
	-Args $packageName, $publishProfileName `
	-ScriptBlock {
		param($PackageName, $PublishProfileName)
		
		Set-StrictMode -Version 3.0
		$ErrorActionPreference = 'Stop'
		#------------------------------

        $service = Get-Service -Include $PublishProfileName
        if ($service -ne $null -and $service.Status -eq "Running"){
          $service.Stop()
        }

		$packageFileName = Join-Path 'C:\Windows\Temp' $packageName
		cmd.exe /c msiexec.exe -i $packageFileName -quiet -norestart
        if ($LastExitCode -ne 0) {
          throw "Command failed with exit code $LastExitCode"
        }
		Remove-Item $packageFileName -Force

		# hack
		$service = Get-Service -Include $PublishProfileName
        if ($service -ne $null -and $PublishProfileName -eq 'Test.08'){
          $service.Start()
        }
	}
}
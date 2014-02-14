Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module .\modules\transform.psm1 -DisableNameChecking
Import-Module .\modules\msbuild.psm1 -DisableNameChecking
Import-Module .\modules\msdeploy.psm1 -DisableNameChecking

function Build-WebPackage($ProjectFileName, $EntryPointMetadata, $MsBuildPlatform = 'x64'){

	try {
		Transform-Log4NetConfig $ProjectFileName
		Transform-WebConfig $ProjectFileName

		$versionFileName = Get-VersionFileName
		$branchFileName = Get-BranchFileName
		
		$packageLocation = "DeployPackages\$($global:Context.EnvironmentName)\Package.zip"
		
		Invoke-MSBuild -MsBuildPlatform $MsBuildPlatform -Arguments @(
		"""$ProjectFileName"""
		"/t:WebPackagePublish"
		"/p:PackageLocation=$packageLocation"
		"/p:DeployIisAppPath=$($EntryPointMetadata.IisAppPath)"
		"/p:ProjectConfigTransformFileName=web.$($global:Context.EnvironmentName).config"
		"/p:GenerateSampleDeployScript=False"
		"/p:VersionFilePath=""$versionFileName"""
		"/p:BranchFilePath=""$branchFileName"""
		)
	}
	finally {
		Restore-Log4NetConfig $ProjectFileName
		Restore-WebConfig $ProjectFileName
	}
	
	$convensionalArtifactName = Join-Path (Split-Path $projectFileName) $packageLocation
	$artifactFileName = Join-Path $global:Context.Dir.Temp ([System.IO.Path]::GetFileNameWithoutExtension($ProjectFileName) + '.zip')
	
	Copy-Item $convensionalArtifactName $artifactFileName
	Publish-Artifacts $artifactFileName
}

function Deploy-WebPackage ($ProjectFileName, $EntryPointMetadata){

	$artifactFileName = [System.IO.Path]::GetFileNameWithoutExtension($ProjectFileName) + '.zip'
	$artifactName = Get-Artifacts '' $artifactFileName

	foreach($targetHost in $EntryPointMetadata.TargetHosts){
		Create-RemoteWebsite $targetHost $EntryPointMetadata.IisAppPath
		
		Sync-MSDeploy `
		-Source "package=""$artifactName""" `
		-HostName $targetHost
	}
}

function Create-RemoteWebsite($HostName, $WebsiteName){

	Invoke-Command `
	-ComputerName $HostName `
	-Args $WebsiteName `
	-ScriptBlock {
		param($WebsiteName)
		
		function Create-LocalWebSite($WebsiteName){
		
			# проверяем существование сайта
			$website = Get-WebSite | Where { $_.Name -eq $WebsiteName }
            if($website -ne $null){
                return
            }

            # создаём папку для сайта
            $websitePhysicalPath = "${Env:SystemDrive}\inetpub\$WebsiteName"
            if (Test-Path $websitePhysicalPath -PathType Container){
              rd $websitePhysicalPath -Recurse -Force | Out-Null
            }
            md $websitePhysicalPath | Out-Null

			# todo: создавать ErmAppPool
			
            $website = New-Website -Name $WebsiteName -HostHeader $WebsiteName -ApplicationPool "ErmAppPool" -PhysicalPath $websitePhysicalPath

            # добавляем https к созданному сайту
            New-WebBinding -Name $WebsiteName -Protocol https -HostHeader $WebsiteName

            # добавляем аутентификацию Windows
            Set-WebConfigurationProperty -filter "/system.WebServer/security/authentication/windowsAuthentication" -name "enabled" -value "true" -location $WebsiteName
		}
		
		Set-StrictMode -Version 3.0
		$ErrorActionPreference = 'Stop'
		#------------------------------

		Import-Module WebAdministration
		Create-LocalWebSite $WebsiteName
	}
}

function Get-VersionFileName {
 	$version = $global:Context.Version
	
	$fileName = ('version_{0}_{1}_{2}_{3}' -f $version.Major, $version.Minor, $version.Build, $version.Revision)
	$filePath = Join-Path $global:Context.Dir.Temp $fileName
	
	Set-Content -Path $filePath -Value $version.ToString(4) -Encoding UTF8
	
	return $filePath
}

function Get-BranchFileName {
	$branch = $global:Context.Branch

	$safeBranchName = $branch.ToLowerInvariant() -replace '[\/:*?"<>|.]', '_'
	
	$fileName = ('branch_{0}' -f $safeBranchName)
	$filePath = Join-Path $global:Context.Dir.Temp $fileName
	
	Set-Content -Path $filePath -Value $branch -Encoding UTF8
	
	return $filePath
}

function Validate-WebSite($EntryPointMetadata, $UriPath){

	$uriBuilder = New-Object System.UriBuilder('https', $EntryPointMetadata.IisAppPath, -1, $UriPath)
	
	try{
		Invoke-WebRequest $uriBuilder.Uri -UseDefaultCredentials -UseBasicParsing -TimeoutSec 300 | Out-Null
	}
	catch{
		Write-Host "Error then calling '$($uriBuilder.Uri)'"
		throw
	}
}
 
Export-ModuleMember -Function Build-WebPackage, Deploy-WebPackage, Validate-WebSite, Create-RemoteWebsite
Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module .\modules\transform.psm1 -DisableNameChecking
Import-Module .\modules\versioning.psm1 -DisableNameChecking
Import-Module .\modules\msbuild.psm1 -DisableNameChecking
Import-Module .\modules\msdeploy.psm1 -DisableNameChecking

function Build-WebPackage($ProjectFileName, $MsBuildPlatform = 'x64'){

	Transform-Log4NetConfig $ProjectFileName

	$publishProfile = Get-PublishProfile $ProjectFileName
	$versionFileName = Get-VersionFileName $global:Context.Dir.Temp $global:Context.Version
	
	Invoke-MSBuild -MsBuildPlatform $MsBuildPlatform -Arguments @(
	"""$ProjectFileName"""
	"/p:DeployOnBuild=True"
	"/p:GenerateSampleDeployScript=False"
	"/p:VersionFilePath=""$versionFileName"""
	"/p:PublishProfile=$($publishProfile.FileName)"
	)
	
	$convensionalArtifactName = Join-Path (Split-Path $projectFileName) "DeployPackages\$($global:Context.EnvironmentName)\Package.zip"
	$artifactFileName = Join-Path $global:Context.Dir.Temp ([System.IO.Path]::GetFileNameWithoutExtension($ProjectFileName) + '.zip')
	
	Copy-Item $convensionalArtifactName $artifactFileName
	Publish-Artifacts $artifactFileName
}

function Deploy-WebPackage ($ProjectFileName){
	$hostName = $global:Context.TargetHostName
	
	$publishProfile = Get-PublishProfile $ProjectFileName
	$websiteName = $publishProfile.UriBuilder.Host
	Create-RemoteWebsite $websiteName $hostName
	
	$artifactFileName = [System.IO.Path]::GetFileNameWithoutExtension($ProjectFileName) + '.zip'
	$artifactName = Get-Artifacts '' $artifactFileName
	
	Sync-MSDeploy `
	-Source "package=""$artifactName""" `
	-HostName $hostName
}

function Get-PublishProfile($ProjectFileName){
	$projectDir = Split-Path $ProjectFileName -Parent
	$publishProfileDir = Join-Path $projectDir 'Properties\PublishProfiles'
	$publishProfileName = $global:Context.EnvironmentName + '.pubxml'
	$publishProfileFileName = Join-Path $publishProfileDir $publishProfileName
	
	$publishProfileXml = [xml](Get-Content -Path $publishProfileFileName)
	$uriBuilder = New-Object System.UriBuilder('https', $publishProfileXml.Project.PropertyGroup.DeployIisAppPath)
	
	return @{
		'FileName' = $publishProfileName
		'UriBuilder' = $uriBuilder
	}
}

# TODO у некоторых проектов есть жёсткая связь с web app, зарефакторить
function Get-WebAppPublishProfile {
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.UI.Web.Mvc'
	$publishProfile = Get-PublishProfile $ProjectFileName

	return $publishProfile
}

function Create-RemoteWebsite($WebsiteName, $HostName){

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

function Validate-WebSite($ProjectFileName, $UriPath){

	$publishProfile = Get-PublishProfile $ProjectFileName
	$publishProfile.UriBuilder.Path = $UriPath
	
	try{
		Invoke-WebRequest $publishProfile.UriBuilder.Uri -UseDefaultCredentials -UseBasicParsing -TimeoutSec 300 | Out-Null
	}
	catch{
		Write-Host "Error then calling '$($uriBuilder.Uri)'"
		throw
	}
}

Export-ModuleMember -Function Build-WebPackage, Deploy-WebPackage, Validate-WebSite, Get-WebAppPublishProfile, Create-RemoteWebsite
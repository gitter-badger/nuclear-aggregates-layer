Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module .\modules\versioning.psm1 -DisableNameChecking
Import-Module .\modules\msbuild.psm1 -DisableNameChecking
Import-Module .\modules\msdeploy.psm1 -DisableNameChecking

function Build-WebPackage($ProjectFileName, $MsBuildPlatform = 'x64'){

	$publishProfile = Get-PublishProfile $ProjectFileName
	$versionFileName = Get-VersionFileName $global:Context.Dir.Temp $global:Context.Version
	$packageFileName = Get-PackageFileName $ProjectFileName 'zip'
	
	Invoke-MSBuild -MsBuildPlatform $MsBuildPlatform -Arguments @(
	"""$ProjectFileName"""
	"/p:DeployOnBuild=True"
	"/p:GenerateSampleDeployScript=False"
	"/p:VersionFilePath=""$versionFileName"""
	"/p:PublishProfile=$($publishProfile.FileName)"
	"/p:PackageFileName=$packageFileName"
	)
	
	Write-Output "##teamcity[publishArtifacts '$packageFileName => Web']"
}

function Deploy-WebPackage ($ProjectFileName){
	$hostName = $global:Context.TargetHostName
	
	Create-RemoteWebsite $ProjectFileName -HostName $hostName
	
	$packageFileName = Get-PackageFileName $ProjectFileName 'zip'
	
	Sync-MSDeploy `
	-Source "package=""$packageFileName""" `
	-HostName $hostName
}

function Get-PublishProfile($ProjectFileName){
	$projectDir = Split-Path $ProjectFileName -Parent
	$publishProfileDir = Join-Path $projectDir 'Properties\PublishProfiles'
	$publishProfileName = $global:Context.EnvironmentName + '.pubxml'
	$publishProfileFileName = Join-Path $publishProfileDir $publishProfileName
	
	$publishProfileXml = [xml](Get-Content -Path $publishProfileFileName)
	$uri = New-Object System.Uri('https://' + $publishProfileXml.Project.PropertyGroup.DeployIisAppPath)
	
	return @{
		'FileName' = $publishProfileName
		'Uri' = $uri
	}
}

# TODO у некоторых проектов есть жёсткая связь с web app, зарефакторить
function Get-WebAppInfo {
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.UI.Web.Mvc'
	
	$projectDir = Split-Path $projectFileName -Parent
	$configFileName = Join-Path $projectDir 'web.config'
	$transformFileName = Join-Path $projectDir "web.$($global:Context.EnvironmentName).config"
	$transformedConfigFileName = Transform-Config $configFileName $transformFileName

	$publishProfile = Get-PublishProfile $ProjectFileName

	return @{
		'Uri' = $publishProfile.Uri
		'TransformedConfigFileName' = $transformedConfigFileName
	}
}

function Transform-Config ($configFileName, $transformFileName){
	$XdtSdkPath = Join-Path $global:Context.Dir.Solution 'packages\Microsoft.Web.Xdt.1.0.0\lib\net40\Microsoft.Web.XmlTransform.dll'
	Add-Type -Path $XdtSdkPath
	
	$configXml = New-Object Microsoft.Web.XmlTransform.XmlTransformableDocument
    $configXml.PreserveWhitespace = $true
    $configXml.Load($configFileName);
	
	$xmlTransformation = New-Object Microsoft.Web.XmlTransform.XmlTransformation($transformFileName)
	$success = $xmlTransformation.Apply($configXml)
	if (!$success){
		throw "Failed to transform $configFileName by $transformFileName"
	}
	
	$transformedFileName = Join-Path $global:Context.Dir.Temp ( "$(Get-Random)" + '.config')
	
	$configXml.Save($transformedFileName)
	return $transformedFileName
}

function Create-RemoteWebsite($ProjectFileName, $HostName){

	$publishProfile = Get-PublishProfile $ProjectFileName
	$websiteName = $publishProfile.Uri.Host

	Invoke-Command `
	-ComputerName $HostName `
	-Args $websiteName `
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
	$uriBuilder = New-Object System.UriBuilder($publishProfile.Uri)
	$uriBuilder.Path = $UriPath

	Invoke-WebRequest $uriBuilder.Uri -UseDefaultCredentials -TimeoutSec 300 | Out-Null
}

Export-ModuleMember -Function Build-WebPackage, Deploy-WebPackage, Validate-WebSite, Get-WebAppInfo
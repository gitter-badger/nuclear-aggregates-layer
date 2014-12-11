﻿Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module .\modules\transform.psm1 -DisableNameChecking
Import-Module .\modules\msbuild.psm1 -DisableNameChecking
Import-Module .\modules\msdeploy.psm1 -DisableNameChecking
Import-Module .\modules\versioning.psm1 -DisableNameChecking

function Build-WebPackage($ProjectFileName, $EntryPointMetadata, $MsBuildPlatform = 'x64'){

	$projectDir = Split-Path $ProjectFileName

	$configFileName1 = Join-Path $projectDir 'log4net.config'
	$content1 = Transform-Config $configFileName1
	Backup-Config $configFileName1 $content1
	$configFileName2 = Join-Path $projectDir 'web.config'
	$content2 = Transform-Config $configFileName2
	Backup-Config $configFileName2 $content2
	try {
		$customXmls = @(Get-VersionFileXml)
		
		$packageLocation = "Packages\$($global:Context.EnvironmentName)\Package.zip"
		
		Invoke-MSBuild $ProjectFileName -Targets 'Package' -Properties @{
			'PackageLocation' = $packageLocation
			'DeployIisAppPath' = $EntryPointMetadata.IisAppPath
			'GenerateSampleDeployScript' = $false
		} -CustomXmls $customXmls -MsBuildPlatform $MsBuildPlatform
	}
	finally {
		Restore-Config $configFileName1
		Restore-Config $configFileName2
	}
	
	$convensionalArtifactName = Join-Path (Split-Path $projectFileName) $packageLocation
	$artifactFileName = Join-Path $global:Context.Dir.Temp ([System.IO.Path]::GetFileNameWithoutExtension($ProjectFileName) + '.zip')
	
	Copy-Item $convensionalArtifactName $artifactFileName
	Publish-Artifacts $artifactFileName
}

function Deploy-WebPackage ($ProjectFileName, $EntryPointMetadata, $Arguments = @()){

	$artifactFileName = [System.IO.Path]::GetFileNameWithoutExtension($ProjectFileName) + '.zip'
	$artifactName = Get-Artifacts '' $artifactFileName

	foreach($targetHost in $EntryPointMetadata.TargetHosts){
		Create-RemoteWebsite $targetHost $EntryPointMetadata.IisAppPath
		
		Invoke-MSDeploy `
		-Source "package=""$artifactName""" `
		-HostName $targetHost `
		-Arguments $Arguments
	}
}

function Create-RemoteWebsite($TargetHost, $WebsiteName){

	Invoke-Command `
	-ComputerName $TargetHost `
	-Args $WebsiteName `
	-ScriptBlock {
		param($WebsiteName)
		
		function Create-LocalWebSite($WebsiteName){
		
			# Get-WebSite bug, workaround using where
			$website = Get-WebSite | where { $_.Name -eq $WebsiteName }
            if($website -ne $null){
				Start-Website $WebsiteName
                return
            }

            # создаём папку для сайта
            $websitePhysicalPath = "${Env:SystemDrive}\inetpub\$WebsiteName"
            if (Test-Path $websitePhysicalPath){
              rd $websitePhysicalPath -Recurse -Force | Out-Null
            }
            md $websitePhysicalPath | Out-Null

			# todo: создавать ErmAppPool
			
            $website = New-Website -Name $WebsiteName -HostHeader $WebsiteName -ApplicationPool "ErmAppPool" -PhysicalPath $websitePhysicalPath

            # добавляем https к созданному сайту
            New-WebBinding -Name $WebsiteName -Protocol https -HostHeader $WebsiteName

            # добавляем аутентификацию Windows
            Set-WebConfigurationProperty -filter "/system.WebServer/security/authentication/windowsAuthentication" -name "enabled" -value "true" -location $WebsiteName
			Start-Website $WebsiteName
		}
		
		Set-StrictMode -Version Latest
		$ErrorActionPreference = 'Stop'
		#------------------------------

		Import-Module WebAdministration
		Create-LocalWebSite $WebsiteName
	}
}

function Get-VersionFileXml {

	$versionFileName = Get-VersionFileName
	$branchFileName = Get-BranchFileName

	[xml]$xml = @"
<Project>
    <ItemGroup>
      <Content Include="$versionFileName">
        <Link>$([System.IO.Path]::GetFileName($versionFileName))</Link>
      </Content>
    </ItemGroup>
    <ItemGroup>
      <Content Include="$branchFileName">
        <Link>$([System.IO.Path]::GetFileName($branchFileName))</Link>
      </Content>
    </ItemGroup>
</Project>
"@
	return $xml
}

function Get-VersionFileName {
 	$version = Get-Version
	
	$fileName = '.version_' + $version.SemanticVersion
	$filePath = Join-Path $global:Context.Dir.Temp $fileName
	
	Set-Content -Path $filePath -Value $version.SemanticVersion -Encoding UTF8
	
	return $filePath
}

function Get-BranchFileName {
	$branch = Get-Branch
	$fileName = '.branch_' + $branch
	$filePath = Join-Path $global:Context.Dir.Temp $fileName
	
	Set-Content -Path $filePath -Value $branch -Encoding UTF8
	
	return $filePath
}

function Validate-WebSite($EntryPointMetadata, $UriPath){

	if (!$EntryPointMetadata.ValidateWebsite){
		return
	}

	$uriBuilder = New-Object System.UriBuilder('https', $EntryPointMetadata.IisAppPath, -1, $UriPath)
	
	try{
		Invoke-WebRequest $uriBuilder.Uri -UseDefaultCredentials -UseBasicParsing -TimeoutSec 300 | Out-Null
	}
	catch{
		Write-Host "Error then calling '$($uriBuilder.Uri)'"
		throw
	}
}

function Take-WebsiteOffline ($TargetHosts, $IisAppPath) {
	$content = Get-AppOfflineFileContent
	
	foreach($targetHost in $TargetHosts){

		Invoke-Command `
		-ComputerName $targetHost `
		-Args $IisAppPath, $content `
		-ScriptBlock {
			param($WebsiteName, $Content)

			function Create-AppOfflineFile ($WebsiteName, $Content){
				$website = Get-WebSite  | where { $_.Name -eq $WebsiteName }
	            if($website -eq $null){
	                return
	            }
				
				$filePath = Join-Path $website.PhysicalPath 'App_offline.htm'
				Set-Content -Path $filePath -Value $Content -Encoding UTF8
			}

			Set-StrictMode -Version Latest
			$ErrorActionPreference = 'Stop'
			#------------------------------

			Import-Module WebAdministration
			Create-AppOfflineFile $WebsiteName $Content
		}
	}
}

function Take-WebsiteOnline ($TargetHosts, $IisAppPath) {
	
	foreach($targetHost in $TargetHosts){

		Invoke-Command `
		-ComputerName $targetHost `
		-Args $IisAppPath `
		-ScriptBlock {
			param($WebsiteName)

			function Delete-AppOfflineFile ($WebsiteName){
				$website = Get-WebSite | where { $_.Name -eq $WebsiteName }
	            if($website -eq $null){
	                return
	            }
				
				$filePath = Join-Path $website.PhysicalPath 'App_offline.htm'
				if (Test-Path $filePath){
					Remove-Item $filePath				
				}
			}

			Set-StrictMode -Version Latest
			$ErrorActionPreference = 'Stop'
			#------------------------------

			Import-Module WebAdministration
			Delete-AppOfflineFile $WebsiteName
		}
	}
}

function Get-AppOfflineFileContent (){
	$version = Get-Version

	$content = @"
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
    <title>Приложение обновляется</title>
    <style  type="text/css">

    div {
        background-color:#ffffcc;
        padding-top:10px;
        padding-bottom:10px;
        padding-left:10px;
        padding-right:10px;    
        border-style:solid;
        border-color:Black;
        border-width:1px;
    }

    </style>
</head>
<body>
    <div>
		<p>Application is updating to version <strong>$($version.SemanticVersion)</strong>.</p>
		<p>Reload page after a few minutes.</p>
    </div>
    <div>
		<p>Приложение обновляется до версии <strong>$($version.SemanticVersion)</strong>.</p>
		<p>Обновите страницу через несколько минут.</p>
    </div>
</body>
</html>
"@

	return $content
}

Export-ModuleMember -Function Build-WebPackage, Deploy-WebPackage, Validate-WebSite, Create-RemoteWebsite, Take-WebsiteOffline, Take-WebsiteOnline
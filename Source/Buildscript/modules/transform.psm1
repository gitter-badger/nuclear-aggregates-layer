Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module .\modules\nuget.psm1 -DisableNameChecking
Import-Module .\modules\metadata.psm1 -DisableNameChecking
Import-Module .\modules\msbuild.psm1 -DisableNameChecking

$PackageInfo = Get-PackageInfo 'Microsoft.Web.Xdt'
Add-Type -Path (Join-Path $PackageInfo.VersionedDir 'lib\net40\Microsoft.Web.XmlTransform.dll')

function Transform-Config ($ConfigFileName) {

	$configTransforms = Get-ConfigTransforms
	$configFileContent = Apply-XdtTransform $ConfigFileName $configTransforms.Xdt
	$configFileContent = Apply-RegexTransform $configFileContent $configTransforms.Regex

	return $configFileContent
}

function Apply-XdtTransform ($ConfigFileName, $TransformFileNames) {
	$configXml = New-Object Microsoft.Web.XmlTransform.XmlTransformableDocument
    $configXml.PreserveWhitespace = $true
    $configXml.Load($ConfigFileName)
	
	foreach($TransformFileName in $TransformFileNames){
		$xmlTransformation = New-Object Microsoft.Web.XmlTransform.XmlTransformation($TransformFileName)
		$success = $xmlTransformation.Apply($configXml)
		if (!$success){
			throw "Failed to transform $configFileName by $transformFileName"
		}
	}
	
	return $configXml.OuterXml
}

function Apply-RegexTransform ($ConfigFileContent, $Regexes){

	foreach ($regex in $Regexes.Keys){
		$replacement = $Regexes[$regex]
		$ConfigFileContent = $ConfigFileContent -replace $regex, $replacement
	}
	
	return $ConfigFileContent
}

function Backup-Config ($fileName, $newContent) {
	$backupFileName = Get-TempFileName $fileName
	Copy-Item $fileName $backupFileName -Force
	Set-Content $fileName $newContent -Encoding UTF8 -Force
}

function Restore-Config ($fileName) {
	$backupFileName = Get-TempFileName $fileName
	if (Test-Path $backupFileName){
		Copy-Item $backupFileName $fileName -Force
	}
}

function Get-TempFileName ($ConfigFileName) {
	$projectDir = Split-Path $ConfigFileName
	$projectFolderName = Split-Path $projectDir -Leaf
	$fileName = Split-Path $ConfigFileName -Leaf

	$tempFileName = "$projectFolderName.$fileName"

	return Join-Path $global:Context.Dir.Temp $tempFileName
}

function Get-ConfigTransforms {

	$configTransforms = @{
		'Xdt' = @()
		'Regex' = @{}
	}
	
	$baseDir = Join-Path $global:Context.Dir.Solution 'Environments'
	
	$entryPointMetadata = Get-EntryPointMetadata 'Transform'
	foreach($xdt in $entryPointMetadata.Xdt){
		$configTransforms.Xdt +=  Join-Path $baseDir $xdt
	}
	$configTransforms.Regex += $entryPointMetadata.Regex
	
	# соглашение
	$environmentName = $global:Context.EnvironmentName
	$environmentTransformTemplate = Join-Path $baseDir "Erm.$environmentName.config"
	if (Test-Path $environmentTransformTemplate){
		$configTransforms.Xdt += $environmentTransformTemplate
	}
	
	return $configTransforms
}

function Get-ConnectionString ($ConnectionStringName) {
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.UI.Web.Mvc'
	$projectDir = Split-Path $ProjectFileName
	$configFileName = Join-Path $projectDir 'web.config'
	[xml]$configFileContent = Transform-Config $configFileName

	$xmlNode = $configFileContent.SelectNodes("configuration/connectionStrings/add[@name = '$ConnectionStringName']")
	if ($xmlNode -eq $null){
		throw "Could not find connection string $ConnectionStringName in config file of project [$projectFileName]"
	}

	return $xmlNode.connectionString
}

function Get-ServiceUriString ($ServiceName) {
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.UI.Web.Mvc'
	$projectDir = Split-Path $ProjectFileName
	$configFileName = Join-Path $projectDir 'web.config'
	[xml]$configFileContent = Transform-Config $configFileName

	$xmlNode = $configFileContent.SelectNodes("configuration/ermServicesSettings/ermServices/ermService[@name = '$ServiceName']")
	if ($xmlNode -eq $null){
		throw "Could not find service $ServiceName in config file of project [$projectFileName]"
	}

	return $xmlNode.baseUrl
}

Export-ModuleMember -Function Transform-Config, Backup-Config, Restore-Config, Get-ConnectionString, Get-ServiceUriString
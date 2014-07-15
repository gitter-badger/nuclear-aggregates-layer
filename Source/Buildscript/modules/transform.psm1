Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module .\modules\nuget.psm1 -DisableNameChecking
Import-Module .\modules\metadata.psm1 -DisableNameChecking
Import-Module .\modules\msbuild.psm1 -DisableNameChecking

$PackageInfo = Get-PackageInfo 'Microsoft.Web.Xdt'
Add-Type -Path (Join-Path $PackageInfo.VersionedDir 'lib\net40\Microsoft.Web.XmlTransform.dll')

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

function Transform-Log4NetConfig ($ProjectFileName) {
	$projectDir = Split-Path $ProjectFileName
	$configFileName = Join-Path $projectDir 'log4net.config'

	Backup-File $configFileName

	$configTransforms = Get-ConfigTransforms $configFileName
	$content = Transform-Config $configFileName $configTransforms
	Out-File $configFileName -InputObject $content -Encoding UTF8 -Force
}

function Restore-Log4NetConfig ($ProjectFileName) {
	$projectDir = Split-Path $ProjectFileName
	$configFileName = Join-Path $projectDir 'log4net.config'

	Restore-File $configFileName
}

function Transform-AppConfig ($ProjectFileName) {
	$projectDir = Split-Path $ProjectFileName
	$configFileName = Join-Path $projectDir 'app.config'

	Backup-File $configFileName

	$configTransforms = Get-ConfigTransforms $configFileName
	$content = Transform-Config $configFileName $configTransforms
	Out-File $configFileName -InputObject $content -Encoding UTF8 -Force
}

function Restore-AppConfig ($ProjectFileName) {
	$projectDir = Split-Path $ProjectFileName
	$configFileName = Join-Path $projectDir 'app.config'

	Restore-File $configFileName
}

function Transform-WebConfig ($ProjectFileName) {
	$projectDir = Split-Path $ProjectFileName
	$configFileName = Join-Path $projectDir 'web.config'

	Backup-File $configFileName

	$configTransforms = Get-ConfigTransforms $configFileName
	$content = Transform-Config $configFileName $configTransforms
	Out-File $configFileName -InputObject $content -Encoding UTF8 -Force
}

function Restore-WebConfig ($ProjectFileName) {
	$projectDir = Split-Path $ProjectFileName
	$configFileName = Join-Path $projectDir 'web.config'

	Restore-File $configFileName
}

function Transform-Config ($ConfigFileName, $ConfigTransforms) {

	$configFileContent = Apply-XdtTransform $ConfigFileName $configTransforms.Xdt
	$configFileContent = Apply-RegexTransform $configFileContent $configTransforms.Regex

	return $configFileContent
}

function Backup-File ($fileName) {
	$backupFileName = Get-TempFileName $fileName
	Copy-Item $fileName $backupFileName -Force
}

function Restore-File ($fileName) {
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

function Get-ConfigTransforms ($configFileName) {

	$configTransforms = @{
		'Xdt' = @()
		'Regex' = @{}
	}
	
	$baseDir = Join-Path $global:Context.Dir.Solution 'Environments'
	
	$entryPointMetadata = Get-EntryPointMetadata 'ConfigTransform'
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

	$configTransforms = Get-ConfigTransforms $configFileName
	$transformedConfigFileContent = Transform-Config $configFileName $configTransforms

	[xml]$configFileContent = $transformedConfigFileContent

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

	$configTransforms = Get-ConfigTransforms $configFileName
	$transformedConfigFileContent = Transform-Config $configFileName $configTransforms

	[xml]$configFileContent = $transformedConfigFileContent

	$xmlNode = $configFileContent.SelectNodes("configuration/ermServicesSettings/ermServices/ermService[@name = '$ServiceName']")
	if ($xmlNode -eq $null){
		throw "Could not find service $ServiceName in config file of project [$projectFileName]"
	}

	return $xmlNode.baseUrl
}

Export-ModuleMember -Function Transform-Log4NetConfig, Restore-Log4NetConfig, Transform-AppConfig, Restore-AppConfig, Transform-WebConfig, Restore-WebConfig, Get-ConnectionString, Get-ServiceUriString
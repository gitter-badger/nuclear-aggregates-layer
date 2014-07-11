Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module .\modules\nuget.psm1 -DisableNameChecking
Import-Module .\modules\metadata.psm1 -DisableNameChecking

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
	
	$configTransforms = @{
		'Xdt' = @(Join-Path $projectDir 'log4net.Release.config')
		'Regex' = @{}
	}

	$transformedConfigFileContent = Transform-Config $configFileName $configTransforms

	# save copy of original config to temp
	$configOriginalCopyFileName = Get-TempFileName $configFileName
	Copy-Item $configFileName $configOriginalCopyFileName -Force
	
	# replace original config with transformed one
	$transformedConfigFileContent | Out-File $configFileName -Encoding UTF8 -Force
}

function Restore-Log4NetConfig ($ProjectFileName) {
	$projectDir = Split-Path $ProjectFileName
	$configFileName = Join-Path $projectDir 'log4net.config'

	$configOriginalCopyFileName = Get-TempFileName $configFileName

	Copy-Item $configOriginalCopyFileName $configFileName -Force
}

function Transform-AppConfig ($ProjectFileName) {
	$projectDir = Split-Path $ProjectFileName
	$configFileName = Join-Path $projectDir 'app.config'

	$configTransforms = Get-ConfigTransforms
	$transformedConfigFileContent = Transform-Config $configFileName $configTransforms
	
	# save copy of original config to temp
	$configOriginalCopyFileName = Get-TempFileName $configFileName
	Copy-Item $configFileName $configOriginalCopyFileName -Force

	# replace original config with transformed one
	$transformedConfigFileContent | Out-File $configFileName -Encoding UTF8 -Force
}

function Restore-AppConfig ($ProjectFileName) {
	$projectDir = Split-Path $ProjectFileName
	$configFileName = Join-Path $projectDir 'app.config'

	$configOriginalCopyFileName = Get-TempFileName $configFileName

	Copy-Item $configOriginalCopyFileName $configFileName -Force
}

function Transform-WebConfig ($ProjectFileName) {
	$projectDir = Split-Path $ProjectFileName
	$configFileName = Join-Path $projectDir 'web.config'

	$configTransforms = Get-ConfigTransforms
	$transformedConfigFileContent = Transform-Config $configFileName $configTransforms

	# save copy of original config to temp
	$configOriginalCopyFileName = Get-TempFileName $configFileName
	Copy-Item $configFileName $configOriginalCopyFileName -Force

	# replace original config with transformed one
	$transformedConfigFileContent | Out-File $configFileName -Encoding UTF8 -Force
}

function Restore-WebConfig ($ProjectFileName) {
	$projectDir = Split-Path $ProjectFileName
	$configFileName = Join-Path $projectDir 'web.config'

	$configOriginalCopyFileName = Get-TempFileName $configFileName

	Copy-Item $configOriginalCopyFileName $configFileName -Force
}

function Transform-Config ($ConfigFileName, $ConfigTransforms) {

	$configFileContent = Apply-XdtTransform $ConfigFileName $configTransforms.Xdt
	$configFileContent = Apply-RegexTransform $configFileContent $configTransforms.Regex

	return $configFileContent
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
	
	$configurationTransformTemplate = Join-Path $baseDir 'Common\Erm.Release.config'
	$configTransforms.Xdt += $configurationTransformTemplate
	
	$entryPointMetadata = Get-EntryPointMetadata 'ConfigTransform'
	foreach($xdt in $entryPointMetadata.Xdt){
		$configTransforms.Xdt +=  Join-Path $baseDir "Templates\$xdt"
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

Export-ModuleMember -Function Transform-Log4NetConfig, Restore-Log4NetConfig, Transform-AppConfig, Restore-AppConfig, Transform-WebConfig, Restore-WebConfig
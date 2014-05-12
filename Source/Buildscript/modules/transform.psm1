Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

$XdtVersion = '1.0.0'

$ThisDir = Split-Path $MyInvocation.MyCommand.Path
$XdtDir = Join-Path $ThisDir "..\..\packages\Microsoft.Web.Xdt.$XdtVersion\lib\net40"
Add-Type -Path (Join-Path $XdtDir 'Microsoft.Web.XmlTransform.dll')

Import-Module .\modules\envsettings.psm1 -DisableNameChecking

# returns content of transformed config 
function Transform-Config ($ConfigFileContent, $TransformFileName) {
	$configXml = New-Object Microsoft.Web.XmlTransform.XmlTransformableDocument
    $configXml.PreserveWhitespace = $true
    $configXml.LoadXml($ConfigFileContent -Join [System.Environment]::NewLine)
	
	$xmlTransformation = New-Object Microsoft.Web.XmlTransform.XmlTransformation($TransformFileName)
	$success = $xmlTransformation.Apply($configXml)
	if (!$success){
		throw "Failed to transform $configFileName by $transformFileName"
	}
	
	return $configXml.OuterXml
}

function Transform-Log4NetConfig ($ProjectFileName) {
	$projectDir = Split-Path $ProjectFileName -Parent
	
	$configFileName = Join-Path $projectDir 'log4net.config'
	$transformFileName = Join-Path $projectDir 'log4net.Release.config'
	$configFileContent = Get-Content $configFileName -Encoding UTF8
	
	$transformedConfigFileContent = Transform-Config $configFileContent $transformFileName

	# save copy of original config to temp
	$configOriginalCopyFileName = Get-TempFileName $configFileName
	Copy-Item $configFileName $configOriginalCopyFileName -Force
	
	# replace original config with transformed one
	$transformedConfigFileContent | Out-File $configFileName -Encoding UTF8 -Force
}

function Restore-Log4NetConfig ($ProjectFileName) {
	$projectDir = Split-Path $ProjectFileName -Parent
	$configFileName = Join-Path $projectDir 'log4net.config'

	$configOriginalCopyFileName = Get-TempFileName $configFileName

	Copy-Item $configOriginalCopyFileName $configFileName -Force
}

function Transform-AppConfig ($ProjectFileName) {
	$projectDir = Split-Path $ProjectFileName -Parent
	$configFileName = Join-Path $projectDir 'app.config'
	$configFileContent = Get-Content $configFileName -Encoding UTF8

	$transformations = @(Get-EnvironmentTransformations 'erm')
	
	$transformFileName = Join-Path $projectDir "app.$($global:Context.EnvironmentName).config"
	if (Test-Path $transformFileName) {
		$transformations += $transformFileName
	}

	$transformedConfigFileContent = Apply-Transformations $configFileContent $transformations (Get-EnvironmentSettings)
	
	# save copy of original config to temp
	$configOriginalCopyFileName = Get-TempFileName $configFileName
	Copy-Item $configFileName $configOriginalCopyFileName -Force

	# replace original config with transformed one
	$transformedConfigFileContent | Out-File $configFileName -Encoding UTF8 -Force
}

function Restore-AppConfig ($ProjectFileName) {
	$projectDir = Split-Path $ProjectFileName -Parent
	$configFileName = Join-Path $projectDir 'app.config'

	$configOriginalCopyFileName = Get-TempFileName $configFileName

	Copy-Item $configOriginalCopyFileName $configFileName -Force
}

function Transform-WebConfig ($ProjectFileName) {
	$projectDir = Split-Path $ProjectFileName -Parent
	$configFileName = Join-Path $projectDir 'web.config'
	$configFileContent = Get-Content $configFileName -Encoding UTF8

	$transformations = Get-EnvironmentTransformations 'erm' 
	$transformedConfigFileContent = Apply-Transformations $configFileContent $transformations (Get-EnvironmentSettings)

	# save copy of original config to temp
	$configOriginalCopyFileName = Get-TempFileName $configFileName
	Copy-Item $configFileName $configOriginalCopyFileName -Force

	# replace original config with transformed one
	$transformedConfigFileContent | Out-File $configFileName -Encoding UTF8 -Force
}

function Restore-WebConfig ($ProjectFileName) {
	$projectDir = Split-Path $ProjectFileName -Parent
	$configFileName = Join-Path $projectDir 'web.config'

	$configOriginalCopyFileName = Get-TempFileName $configFileName

	Copy-Item $configOriginalCopyFileName $configFileName -Force
}

function Evaluate-Config ($ProjectFileName, $ConfigFilePrefix) {
	$projectDir = Split-Path $ProjectFileName -Parent
	$configFileName = Join-Path $projectDir "$ConfigFilePrefix.config"
	$configFileContent = Get-Content $configFileName -Encoding UTF8

	$transformations = @(Get-EnvironmentTransformations 'erm')

	$transformFileName = Join-Path $projectDir "$ConfigFilePrefix.$($global:Context.EnvironmentName).config"
	if (Test-Path $transformFileName) {
		$transformations += $transformFileName
	}

	return Apply-Transformations $configFileContent $transformations (Get-EnvironmentSettings)
}

function Apply-Transformations ($ConfigFileContent, $TransformFileNames, $EnvironmentParameters) {
	$projectDirName = Split-Path (Split-Path $ConfigFileName -Parent) -Leaf

	# apply transformations
	$transformedConfigFileContent = $ConfigFileContent
	foreach ($transformFileName in $TransformFileNames) {
		$transformedConfigFileContent = Transform-Config $transformedConfigFileContent $transformFileName
	}

	# apply environment parameters
	foreach ($p in $EnvironmentParameters.GetEnumerator()) {
		$transformedConfigFileContent = $transformedConfigFileContent -replace "{$($p.Name)}", $p.Value
	}

	return $transformedConfigFileContent;
}

function Get-TempFileName ($ConfigFileName) {
	$projectDir = Split-Path $ConfigFileName -Parent
	$projectFolderName = Split-Path $projectDir -Leaf
	$fileName = Split-Path $ConfigFileName -Leaf

	$tempFileName = "$projectFolderName.$fileName"

	return Join-Path $global:Context.Dir.Temp $tempFileName
}

Export-ModuleMember -Function Transform-Log4NetConfig, Restore-Log4NetConfig, Transform-AppConfig, Restore-AppConfig, Transform-WebConfig, Restore-WebConfig, Evaluate-Config
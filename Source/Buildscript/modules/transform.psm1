Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

function Load-Sdk {
	$XdtSdk_1_0_0 = Join-Path $global:Context.Dir.Solution 'packages\Microsoft.Web.Xdt.1.0.0\lib\net40'
	Add-Type -Path (Join-Path $XdtSdk_1_0_0 'Microsoft.Web.XmlTransform.dll')
}

# returns path to transformed config in Temp folder
function Transform-Config ($configFileName, $transformFileName){
	Load-Sdk
	
	$configXml = New-Object Microsoft.Web.XmlTransform.XmlTransformableDocument
    $configXml.PreserveWhitespace = $true
    $configXml.Load($configFileName);
	
	$xmlTransformation = New-Object Microsoft.Web.XmlTransform.XmlTransformation($transformFileName)
	$success = $xmlTransformation.Apply($configXml)
	if (!$success){
		throw "Failed to transform $configFileName by $transformFileName"
	}
	
	$transformedConfigFileName = Join-Path $global:Context.Dir.Temp ( "$(Get-Random)" + '.config')
	
	$configXml.Save($transformedConfigFileName)
	return $transformedConfigFileName
}

function Transform-Log4NetConfig ($ProjectFileName) {
	$projectDir = Split-Path $ProjectFileName -Parent
	
	$configFileName = Join-Path $projectDir 'log4net.config'
	$transformFileName = Join-Path $projectDir 'log4net.Release.config'
	$transformedConfigFileName = Transform-Config $configFileName $transformFileName
	
	# replace original config with transformed one
	Copy-Item $transformedConfigFileName $configFileName -Force
}

function Transform-AppConfig ($ProjectFileName) {
	$projectDir = Split-Path $projectFileName -Parent
	
	$configFileName = Join-Path $projectDir 'app.config'
	$transformFileName = Join-Path $projectDir "app.$($global:Context.EnvironmentName).config"
	$transformedConfigFileName = Transform-Config $configFileName $transformFileName
	
	# replace original config with transformed one
	Copy-Item $transformedConfigFileName $configFileName -Force
}

function Get-TransformedWebConfig ($ProjectFileName) {
	$projectDir = Split-Path $projectFileName -Parent
	
	$configFileName = Join-Path $projectDir 'web.config'
	$transformFileName = Join-Path $projectDir "web.$($global:Context.EnvironmentName).config"
	$transformedConfigFileName = Transform-Config $configFileName $transformFileName

	# return transformed config, do not touch original one
	return $transformedConfigFileName
}

Export-ModuleMember -Function Transform-Log4NetConfig, Transform-AppConfig, Get-TransformedWebConfig
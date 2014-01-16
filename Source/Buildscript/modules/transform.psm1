Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

$ThisDir = Split-Path $MyInvocation.MyCommand.Path -Parent

$XdtSdk_1_0_0 = Join-Path $ThisDir '..\..\packages\Microsoft.Web.Xdt.1.0.0\lib\net40'
Add-Type -Path (Join-Path $XdtSdk_1_0_0 'Microsoft.Web.XmlTransform.dll')

# returns path to transformed config in Temp folder
function Transform-Config ($configFileName, $transformFileName){

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
	
	# save copy of original config to temp
	$projectName = Split-Path $ProjectFileName -Leaf
	$configOriginalCopyFileName = Join-Path $global:Context.Dir.Temp "log4net.$projectName.config"
	Copy-Item $configFileName $configOriginalCopyFileName -Force
	
	# replace original config with transformed one
	Copy-Item $transformedConfigFileName $configFileName -Force
}

function Restore-Log4NetConfig ($ProjectFileName) {
	$projectDir = Split-Path $ProjectFileName -Parent
	$configFileName = Join-Path $projectDir 'log4net.config'

	$projectName = Split-Path $ProjectFileName -Leaf
	$configOriginalCopyFileName = Join-Path $global:Context.Dir.Temp "log4net.$projectName.config"

	Copy-Item $configOriginalCopyFileName $configFileName -Force
}

function Transform-AppConfig ($ProjectFileName) {
	$projectDir = Split-Path $ProjectFileName -Parent
	
	$configFileName = Join-Path $projectDir 'app.config'
	$transformFileName = Join-Path $projectDir "app.$($global:Context.EnvironmentName).config"
	$transformedConfigFileName = Transform-Config $configFileName $transformFileName
	
	# save copy of original config to temp
	$projectName = Split-Path $projectFileName -Leaf
	$configOriginalCopyFileName = Join-Path $global:Context.Dir.Temp "app.$projectName.config"
	Copy-Item $configFileName $configOriginalCopyFileName -Force

	# replace original config with transformed one
	Copy-Item $transformedConfigFileName $configFileName -Force
}

function Restore-AppConfig ($ProjectFileName) {
	$projectDir = Split-Path $ProjectFileName -Parent
	$configFileName = Join-Path $projectDir 'app.config'

	$projectName = Split-Path $projectFileName -Leaf
	$configOriginalCopyFileName = Join-Path $global:Context.Dir.Temp "app.$projectName.config"

	Copy-Item $configOriginalCopyFileName $configFileName -Force
}

Export-ModuleMember -Function Transform-Log4NetConfig, Restore-Log4NetConfig, Transform-AppConfig, Restore-AppConfig
# To install elasticsearch, run
# Import-Module 'PATH-TO\elasticsearch.psm1' ; Install-Elasticsearch 'D:\'
# To uninstall elasticsearch, run
# Import-Module 'PATH-TO\elasticsearch.psm1' ; Uninstall-Elasticsearch 'D:\'


Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

$ThisDir = Split-Path $MyInvocation.MyCommand.Path
Add-Type -Path (Join-Path $ThisDir 'tar-cs.dll')
Add-Type -Assembly System.IO.Compression
Add-Type -Assembly System.IO.Compression.FileSystem

$ServerJreUrl = 'http://download.oracle.com/otn-pub/java/jdk/8u20-b26/server-jre-8u20-windows-x64.tar.gz'
$ElasticsearchUrl = 'https://download.elasticsearch.org/elasticsearch/elasticsearch/elasticsearch-1.3.2.zip'
$ElasticsearchPlugins = @(
	'elasticsearch/elasticsearch-transport-thrift/2.3.0'
	'elasticsearch/elasticsearch-analysis-icu/2.3.0'
	'mobz/elasticsearch-head'
	'royrusso/elasticsearch-HQ'
	'polyfractal/elasticsearch-inquisitor'
)

function Download-ElasticSearch($InstallFolder){

	$ElasticsearchDir = Join-Path $InstallFolder 'elasticsearch*'
	if (Test-Path $ElasticsearchDir){
		return (Get-Item $ElasticsearchDir)
	}

	$request = [System.Net.WebRequest]::Create($ElasticsearchUrl)
	$request.UseDefaultCredentials = $true
	$proxy = $request.Proxy
	if ($proxy -ne $null){
		$proxy.Credentials = [System.Net.CredentialCache]::DefaultCredentials
	}
	$response = $request.GetResponse()
	$responseStream = $response.GetResponseStream()

	$zipArchive = New-Object System.IO.Compression.ZipArchive($responseStream, 'Read')
	[System.IO.Compression.ZipFileExtensions]::ExtractToDirectory($zipArchive, $InstallFolder)
	$zipArchive.Dispose()

	return (Get-Item $ElasticsearchDir)
}

function Download-ServerJre($InstallFolder){
	$ServerJreDir = Join-Path $InstallFolder 'jdk*'
	if (Test-Path $ServerJreDir){
		return (Get-Item $ServerJreDir)
	}
	
	$request = [System.Net.WebRequest]::Create($ServerJreUrl)
	$request.UseDefaultCredentials = $true
	$proxy = $request.Proxy
	if ($proxy -ne $null){
		$proxy.Credentials = [System.Net.CredentialCache]::DefaultCredentials
	}
	$cookie = New-Object System.Net.Cookie('oraclelicense', 'accept-securebackup-cookie')
	$request.CookieContainer = New-Object System.Net.CookieContainer
	$request.CookieContainer.Add('http://download.oracle.com', $cookie)
	$request.CookieContainer.Add('http://edelivery.oracle.com', $cookie)
	$response = $request.GetResponse()
	$responseStream = $response.GetResponseStream()

	$gZipStream = New-Object System.IO.Compression.GZipStream($responseStream, [System.IO.Compression.CompressionMode]'Decompress')
	$tarReader = New-Object tar_cs.TarReader($gZipStream)
	$tarReader.ReadToEnd($InstallFolder)
	$gZipStream.Dispose()
	
	return (Get-Item $ServerJreDir)
}

function Configure-Elasticsearch ($ElasticsearchDir, $ServerJreDir) {

	# install plugins
	if (!(Test-Path (Join-Path $ElasticsearchDir 'plugins'))){
		$pluginExe = Join-Path $ElasticsearchDir 'bin\plugin.bat'
		${Env:JAVA_HOME} = $ServerJreDir
		foreach($elasticsearchPlugin in $ElasticsearchPlugins){
			& $pluginExe @('-install', $elasticsearchPlugin)
		}
	}
	
	# config file
	$configFileName = Join-Path $ElasticsearchDir 'config\elasticsearch.yml'
	$content = Get-Content $configFileName
	$content = ReplaceOrAdd $content '(#)?cluster\.name(.)*$' "cluster.name: ""${Env:ComputerName}"""
	$content = ReplaceOrAdd $content '(#)?http\.compression(.)*$' "http.compression: true"
	$content = ReplaceOrAdd $content '(#)?action\.auto_create_index(.)*$' "action.auto_create_index: false"
	$content = ReplaceOrAdd $content '(#)?index\.mapper\.dynamic(.)*$' "index.mapper.dynamic: false"
	$content = ReplaceOrAdd $content '(#)?http\.max_header_size(.)*$' "http.max_header_size: 16kb"
	
	Set-Content $configFileName $content | Out-Null
}

function ReplaceOrAdd($content, $regex, $replacement){

	$replacedContent = $content -replace $regex, $replacement
	if ([string]$replacedContent -eq [string]$content){
		$replacedContent = $content + $replacement
	}
	
	return $replacedContent
}

function Install-ElasticsearchNode ($ElasticsearchDir, $ServerJreDir, $Index){
	$service = Get-Service -Include $Index
	if ($service -eq $null){
		$serviceExe = Join-Path $ElasticsearchDir 'bin\service.bat'
		${Env:JAVA_HOME} = $ServerJreDir
		${Env:ES_START_TYPE} = 'auto'
		& $serviceExe @('install', $Index)
	}
	
	$service = Get-Service $Index
	if ($service.Status -ne 'Running'){
		$service.Start()
	}
}

function Uninstall-ElasticsearchNode ($ElasticsearchDir, $ServerJreDir, $Index){
	$service = Get-Service -Include $Index
	if ($service -ne $null){
		$serviceExe = Join-Path $ElasticsearchDir 'bin\service.bat'
		${Env:JAVA_HOME} = $ServerJreDir
		& $serviceExe @('remove', $Index)
	}
}

function Install-Elasticsearch ($InstallFolder) {
	if (!(UserHasAdminRole)){
		Write-Error 'Run powershell as Administrator'
		return
	}
	if ($InstallFolder -eq $null){
		Write-Error 'Set install folder for Elasticsearch'
		return
	}

	Write-Host 'Downloading Elasticsearch...'
	$ElasticsearchDir = Download-ElasticSearch $InstallFolder
	Write-Host 'Downloading Server JRE...'
	$ServerJreDir = Download-ServerJre $ElasticsearchDir
	
	Write-Host 'Configuring Elasticsearch...'
	Configure-Elasticsearch $ElasticsearchDir $ServerJreDir
	Write-Host 'Installing service...'
	Install-ElasticsearchNode $ElasticsearchDir $ServerJreDir 0
	Write-Host 'Installing service...'
	Install-ElasticsearchNode $ElasticsearchDir $ServerJreDir 1
}

function Uninstall-Elasticsearch($InstallFolder) {
	if (!(UserHasAdminRole)){
		Write-Error 'Run powershell as Administrator'
		return
	}
	if ($InstallFolder -eq $null){
		Write-Error 'Set install folder for Elasticsearch'
		return
	}

	Write-Host 'Ensuring Elasticsearch folder...'
	$ElasticsearchDir = Download-ElasticSearch $InstallFolder
	Write-Host 'Ensuring Server JRE folder...'
	$ServerJreDir = Download-ServerJre $ElasticsearchDir

	Write-Host 'Uninstalling service...'
	Uninstall-ElasticsearchNode $ElasticsearchDir $ServerJreDir 0
	Write-Host 'Uninstalling service...'
	Uninstall-ElasticsearchNode $ElasticsearchDir $ServerJreDir 1
	
	Write-Host 'Removing folder...'
	rd $ElasticsearchDir -Recurse
}

function UserHasAdminRole(){
	$myWindowsID = [System.Security.Principal.WindowsIdentity]::GetCurrent()
	$myWindowsPrincipal = New-Object System.Security.Principal.WindowsPrincipal($myWindowsID)
	$adminRole = [System.Security.Principal.WindowsBuiltInRole]::Administrator

	return $myWindowsPrincipal.IsInRole($adminRole)
}

Export-ModuleMember -Function Install-Elasticsearch, Uninstall-Elasticsearch
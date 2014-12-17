Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module .\modules\nuget.psm1 -DisableNameChecking
Import-Module .\modules\metadata.psm1 -DisableNameChecking
Import-Module .\modules\msbuild.psm1 -DisableNameChecking

$PackageInfo = Get-PackageInfo 'Microsoft.Web.Xdt'
Add-Type -Path (Join-Path $PackageInfo.VersionedDir 'lib\net40\Microsoft.Web.XmlTransform.dll')

function Transform-Config ($ConfigFileName) {
	$configFileContent = Get-TransformedConfigFileContent $ConfigFileName
	$customXml = Get-MSBuildCustomXml $ConfigFileName $configFileContent
	return $customXml
}

function Get-MSBuildCustomXml ($ConfigFileName, $ConfigFileContent){
	$fileName = [System.IO.Path]::GetFileName($ConfigFileName)
	$newExtension = '.transformed' + [System.IO.Path]::GetExtension($ConfigFileName)
	$newConfigFileName = [System.IO.Path]::ChangeExtension($ConfigFileName, $newExtension)
	
	Set-Content $newConfigFileName $ConfigFileContent -Encoding UTF8
	
	$targetName = "Target-$(Get-Random)"
	[xml]$xml = @"
<Project>
	<PropertyGroup>
		<CoreBuildDependsOn>
			$targetName;
			`$(CoreBuildDependsOn)
		</CoreBuildDependsOn>
	</PropertyGroup>
	<Target Name="$targetName">
		<ItemGroup Condition="Exists('$fileName')">
		
			<!-- primary -->
		    <NoneToRemove Include="@(None)" Condition=" '%(None.Identity)' == '$fileName' " />
		    <None Remove="@(NoneToRemove)" />
		    <None Include="$newConfigFileName" Condition=" '@(NoneToRemove)' != '' ">
				<Link>$fileName</Link>
				<CopyToOutputDirectory>Always</CopyToOutputDirectory>
			</None>

			<!-- secondary -->
		    <ContentToRemove Include="@(Content)" Condition=" '%(Content.Identity)' == '$fileName' " />
		    <Content Remove="@(ContentToRemove)" />
		    <Content Include="$newConfigFileName" Condition=" '@(ContentToRemove)' != '' ">
				<Link>$fileName</Link>			
			</Content>

		</ItemGroup>
		<Message Importance="high" Text="!!!TESTTRANSFORM!!!" />
	</Target>
</Project>
"@
	return $xml
}

function Get-TransformedConfigFileContent ($ConfigFileName){
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
	[xml]$configFileContent = Get-TransformedConfigFileContent $configFileName

	$xmlNode = $configFileContent.SelectNodes("configuration/connectionStrings/add[@name = '$ConnectionStringName']")
	if ($xmlNode -eq $null){
		throw "Could not find connection string '$ConnectionStringName' in config file of project '$projectFileName'"
	}

	return $xmlNode.connectionString
}

function Get-ServiceUriString ($ServiceName) {
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.UI.Web.Mvc'
	$projectDir = Split-Path $ProjectFileName
	$configFileName = Join-Path $projectDir 'web.config'
	[xml]$configFileContent = Get-TransformedConfigFileContent $configFileName

	$xmlNode = $configFileContent.SelectNodes("configuration/ermServicesSettings/ermServices/ermService[@name = '$ServiceName']")
	if ($xmlNode -eq $null){
		throw "Could not find service '$ServiceName' in config file of project '$projectFileName'"
	}

	return $xmlNode.baseUrl
}

function Get-AppSetting ($SettingName) {
	$projectFileName = Get-ProjectFileName '.' '2Gis.Erm.UI.Web.Mvc'
	$projectDir = Split-Path $ProjectFileName
	$configFileName = Join-Path $projectDir 'web.config'
	[xml]$configFileContent = Get-TransformedConfigFileContent $configFileName

	$xmlNode = $configFileContent.SelectNodes("configuration/appSettings/add[@key = '$SettingName']")
	if ($xmlNode -eq $null){
		throw "Could not find setting '$SettingName' in config file of project '$projectFileName'"
	}

	return $xmlNode.value
}

Export-ModuleMember -Function Transform-Config, Get-ConnectionString, Get-ServiceUriString, Get-AppSetting
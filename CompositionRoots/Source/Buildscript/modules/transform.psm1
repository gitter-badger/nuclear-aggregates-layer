Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module "$PSScriptRoot\nuget.psm1" -DisableNameChecking
Import-Module "$PSScriptRoot\metadata.psm1" -DisableNameChecking
Import-Module "$PSScriptRoot\msbuild.psm1" -DisableNameChecking

$PackageInfo = Get-PackageInfo 'Microsoft.Web.Xdt'
Add-Type -Path (Join-Path $PackageInfo.VersionedDir 'lib\net40\Microsoft.Web.XmlTransform.dll')

function Transform-Config ($ConfigFileName) {
	[xml]$configFile = Get-TransformedConfigFileContent $ConfigFileName
	$customXml = Get-MSBuildCustomXml $ConfigFileName $configFile
	return $customXml
}

function Get-MSBuildCustomXml ($ConfigFileName, [xml]$configFile){
	$fileName = [System.IO.Path]::GetFileName($ConfigFileName)
	$newExtension = '.transformed' + [System.IO.Path]::GetExtension($ConfigFileName)
	$newConfigFileName = [System.IO.Path]::ChangeExtension($ConfigFileName, $newExtension)
	
	$configFile.Save($newConfigFileName)
	
	$targetName = "TransformConfig-$(Get-Random)"
	[xml]$xml = @"
<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
	<!-- Трансформация $fileName -->
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
	</Target>
</Project>
"@
	return $xml
}

function Get-TransformedConfigFileContent ($ConfigFileName){
	$configTransforms = Get-ConfigTransforms
	
	[xml]$configFile = Apply-XdtTransform $ConfigFileName $configTransforms.Xdt
	[xml]$configFile = Apply-RegexTransform $configFile $configTransforms.Regex

	return $configFile
}

function Apply-XdtTransform ($ConfigFileName, [string[]]$TransformFileNames) {
	$configXml = New-Object Microsoft.Web.XmlTransform.XmlTransformableDocument
    $configXml.PreserveWhitespace = $true
    $configXml.Load($ConfigFileName)
	
	foreach ($TransformFileName in $TransformFileNames){
		$xmlTransformation = New-Object Microsoft.Web.XmlTransform.XmlTransformation($TransformFileName)
		$success = $xmlTransformation.Apply($configXml)
		if (!$success){
			throw "Failed to transform $configFileName by $transformFileName"
		}
	}
	
	return $configXml
}

function Apply-RegexTransform ([xml]$configXml, [hashtable]$Regexes){

	$allAttributes = New-Object System.Collections.ArrayList
	LoadAllAttributes $configXml.DocumentElement $allAttributes

	foreach ($regex in $Regexes.GetEnumerator()){
	
		foreach ($attribute in $allAttributes){
			if ($attribute.Value.Contains($regex.Key)){
				$attribute.Value = $attribute.Value.Replace($regex.Key, $regex.Value)
			}
		}
	}
	
	return $configXml
}

function LoadAllAttributes ([System.Xml.XmlNode]$xmlNode, [System.Collections.ArrayList]$arrayList){

	$arrayList.AddRange($xmlNode.Attributes)
	
	foreach($childNode in $xmlNode.ChildNodes){
		if ($childNode.NodeType -eq 'Element'){
			LoadAllAttributes $childNode $arrayList
		}
	}
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
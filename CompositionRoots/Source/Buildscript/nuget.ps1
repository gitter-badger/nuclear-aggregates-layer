Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

$Servers = @{
	'PublishPackages' = @{
		'Url' = 'http://nuget.2gis.local'
		'ApiKey' = ':enrbq rjl'
	}
	'PublishSymbols' = @{
		'Url' = 'http://nuget.2gis.local/SymbolServer/NuGet'
		'ApiKey' = ':enrbq rjl'
	}
}

Import-Module .\modules\nuget.psm1 -DisableNameChecking
Import-Module .\modules\versioning.psm1 -DisableNameChecking

Task Build-AutoTestsPackages -Depends Create-GlobalContext, Set-BuildNumber {

	$tempDir = Join-Path $global:Context.Dir.Temp 'NuGet'
	if (!(Test-Path $tempDir)){
		md $tempDir | Out-Null
	}

	Copy-NugetConfig
	$nuspecDir = Join-Path $global:Context.Dir.Solution '..\..'
	Create-NuspecFiles $nuspecDir

	$projectsDir = Join-Path $global:Context.Dir.Solution '..\..\Platform'
	$include = @(
		'2Gis.Erm.Platform.Model.csproj'
		'2Gis.Erm.Platform.Common.csproj'

		'2Gis.Erm.Platform.API.ServiceBusBroker.csproj'
		'2Gis.Erm.Platform.API.Core.csproj'
	)
	Build-Packages $projectsDir $include $tempDir
	
	$projectsDir = Join-Path $global:Context.Dir.Solution '..\..\BLQuerying'
	$include = @(
		'2Gis.Erm.Qds.API.Operations.csproj'
	)
	Build-Packages $projectsDir $include $tempDir
	
	Publish-Artifacts $tempDir 'NuGet'
}

Task Deploy-NuGet {
	$artifactName = Get-Artifacts 'NuGet'
	$packages = Get-ChildItem $artifactName -Filter '*.nupkg' -Recurse
	foreach($package in $packages){
		
		if ($package.Name.EndsWith('.symbols.nupkg')){
			$source = $Servers.PublishSymbols.Url
			$apiKey = $Servers.PublishSymbols.ApiKey
		} else{
			$source = $Servers.PublishPackages.Url
			$apiKey = $Servers.PublishPackages.ApiKey
		}
		
		Invoke-NuGet @(
			'push'
			"$($package.FullName)"
			'-Source'
			$source
			'-ApiKey'
			$apiKey
		)
	}
}

function Build-Packages ($ProjectsDir, $Include, $OutputDirectory){

	$assemblyInfos = Get-ChildItem $projectsDir -Filter 'AssemblyInfo.Version.cs' -Recurse
	Update-AssemblyInfo $assemblyInfos

	$projects = Get-ChildItem $projectsDir -Include $include -Recurse
	foreach($project in $projects){
		
		$buildFileName = Create-BuildFile $project.FullName
		
		Invoke-NuGet @(
			'pack'
			$buildFileName
			'-Build'
			'-IncludeReferencedProjects'
			'-ExcludeEmptyDirectories'
			'-NoPackageAnalysis'
			'-OutputDirectory'
			$OutputDirectory
			# create '.symbols.nupkg'
			'-Symbols'
		)
	}
}

# создаём типовой nuspec файл
function Create-NuspecFiles ($solutionDir){
	$content = @'
<?xml version="1.0" encoding="utf-8"?>
<package>
  <metadata>
    <id>$id$</id>
    <version>$version$</version>
    <authors>$author$</authors>
    <description>$description$</description>
	
	<tags>ERM</tags>
  </metadata>
  <files>
  	<file src="bin\$configuration$\**\$id$.resources.dll" />
  </files>
</package>
'@

	$projects = Get-ChildItem $solutionDir -Filter '*.csproj' -Recurse
	foreach($project in $projects){
		$projectFileName = $project.FullName
		$projectDir = Split-Path $projectFileName
		
		$nuspecFileName = Join-Path $projectDir ([System.IO.Path]::GetFileNameWithoutExtension($projectFileName) + '.nuspec')
		if (!(Test-Path $nuspecFileName)){
			Set-Content $nuspecFileName $content -Encoding UTF8 -Force
		}
	}
}

# костыль чтобы указать repositoryPath
function Copy-NugetConfig {
	$content = @"
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <config>
    <add key="repositoryPath" value="CompositionRoots\Source\packages" />
  </config>
</configuration>
"@
	$fileName = Join-Path $global:Context.Dir.Solution '..\..\NuGet.Config'
	
	if (!(Test-Path $fileName)){
		Set-Content $fileName $content -Encoding UTF8 -Force
	}
}
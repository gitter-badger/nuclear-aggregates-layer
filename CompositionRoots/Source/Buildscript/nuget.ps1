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

Task Build-AutoTestsPackages -Depends Set-BuildNumber, Update-AssemblyInfo {

	$tempDir = Join-Path $global:Context.Dir.Temp 'NuGet'
	if (!(Test-Path $tempDir)){
		md $tempDir | Out-Null
	}

	Copy-NugetConfig
	
	# нужно создать nuspec файлы для вообще всех проектов, чтобы правильно работал флаг IncludeReferencedProjects
	$projects = Find-Projects @('..\..')
	Create-NuspecFiles $projects
	
	$projectDirs = @(
		'..\..\Platform'
		'..\..\BLQuerying'
	)
	
	$include = @(
		'2Gis.Erm.Platform.Model.csproj'
		'2Gis.Erm.Platform.Common.csproj'
		'2Gis.Erm.Platform.API.ServiceBusBroker.csproj'
		'2Gis.Erm.Platform.API.Core.csproj'
		
		'2Gis.Erm.Qds.API.Operations.csproj'		
	)
	
	$projects = Find-Projects $projectDirs $include
	Build-Packages $projects $tempDir
	
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

function Build-Packages ($Projects, $OutputDirectory){

	foreach($project in $Projects){
		
		$buildFileName = Create-BuildFile $project.FullName
		Invoke-MSBuild $buildFileName
		
		Invoke-NuGet @(
			'pack'
			$buildFileName
			'-Properties'
			'Configuration=Release'
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
function Create-NuspecFiles ($Projects){

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

	foreach($project in $Projects){
	
		$nuspecFileName = [System.IO.Path]::ChangeExtension($project.FullName, '.nuspec')
		if ((Test-Path $nuspecFileName)){
			continue
		}
		
		Set-Content $nuspecFileName $content -Encoding UTF8 -Force
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
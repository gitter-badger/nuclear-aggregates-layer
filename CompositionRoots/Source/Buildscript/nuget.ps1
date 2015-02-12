Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module "$PSScriptRoot\modules\nuget.psm1" -DisableNameChecking

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
		'..\..\BLCore'
		'..\..\BLQuerying'
	)
	
	$include = @(
		'2Gis.Erm.Platform.Model.csproj'
		'2Gis.Erm.Platform.Common.csproj'
		'2Gis.Erm.Platform.API.ServiceBusBroker.csproj'
		'2Gis.Erm.Platform.API.Core.csproj'
		'2Gis.Erm.BLCore.API.Releasing.csproj'
		'2Gis.Erm.BLCore.API.Operations.Special.csproj'
	
		'2Gis.Erm.Qds.API.Operations.csproj'
	)
	
	$projects = Find-Projects $projectDirs $include
	Build-Packages $projects $tempDir
	
	Publish-Artifacts $tempDir 'NuGet'
}

Task Deploy-NuGet {
	$artifactName = Get-Artifacts 'NuGet'

	$packges = Get-ChildItem $artifactName -Include '*.nupkg' -Exclude '*.symbols.nupkg' -Recurse
	Deploy-Packages $packges 'http://nuget.2gis.local' ':enrbq rjl'

	$symbolPackges = Get-ChildItem $artifactName -Include '*.symbols.nupkg' -Recurse
	Deploy-Packages $symbolPackges 'http://nuget.2gis.local/SymbolServer/NuGet' ':enrbq rjl'
}

function Build-Packages ($Projects, $OutputDirectory){

	foreach($project in $Projects){
		
		$buildFileName = Create-BuildFile $project.FullName
		Invoke-MSBuild $buildFileName
		
		Invoke-NuGet @(
			'pack'
			$buildFileName
			'-Properties'
			# TODO отрефакторить
			'Configuration=Release;VisualStudioVersion=12.0'
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
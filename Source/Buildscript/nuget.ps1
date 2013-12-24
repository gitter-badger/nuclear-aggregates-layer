Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module .\modules\versioning.psm1 -DisableNameChecking
Import-Module .\modules\msbuild.psm1 -DisableNameChecking

Task Build-NugetPackages -Depends `
Build-PlatformNugetPackages

Task Update-PlatformAssemblyInfo {
	$assemblyInfoPath = Join-Path $global:Context.Dir.Solution '..\..\Platform\Source\AssemblyInfo.Version.cs'
	Update-AssemblyInfo $assemblyInfoPath $global:Context.Version $global:Context.PackageVersion
}

Task Build-PlatformProjects -Depends Update-PlatformAssemblyInfo {

	$projectsDir = Join-Path $global:Context.Dir.Solution '..\..\Platform\Source'
	#$projectsDir = Join-Path $global:Context.Dir.Solution '..\..\Platform\Source\2Gis.Erm.Platform.Common'

	$projectFileNames = Get-ChildItem $projectsDir -Filter '*.csproj' -Recurse
	foreach($projectFileName in $projectFileNames){
		Invoke-MSBuild @("""$($projectFileName.FullName)""")
	}
}
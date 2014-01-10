Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module .\modules\nuget.psm1 -DisableNameChecking
Import-Module .\modules\versioning.psm1 -DisableNameChecking
Import-Module .\modules\msbuild.psm1 -DisableNameChecking

Task Deploy-PlatformNuGetPackages -Depends Build-PlatformNuGetPackages {

	# URI for publishing
	$nuGetServerUriBuilder = New-Object System.UriBuilder("http://uk-erm-build03:81")
	$nuGetServerUriBuilder.Path = 'nuget\Platform-Main'

	$artifactName = Get-Artifacts 'NuGet\Platform-Main'
	
	$nuGetPackages = Get-ChildItem $artifactName -Filter '*.nupkg' -Recurse
	foreach($nuGetPackage in $nuGetPackages){
		$nuGetPackageFileName = $nuGetPackage.FullName
		
		Invoke-NuGet @(
			"push"
			"""$nuGetPackageFileName"""
			"-Source"
			"$nuGetServerUriBuilder"
			"-ApiKey"
			"Admin:Admin"
		)
	}
}

Task Build-PlatformNuGetPackages -Depends Update-PlatformAssemblyInfo {

	$projectsDir = Join-Path $global:Context.Dir.Solution '..\..\Platform\Source'
	$projects = Get-ChildItem $projectsDir -Filter '*.csproj' -Recurse
	
	$tempDir = Join-Path $global:Context.Dir.Temp ('Nuget-' + (Get-Random))
	md $tempDir | Out-Null
	
	foreach($project in $projects){
		$projectFileName = $project.FullName
		$projectDir = Split-Path $projectFileName -Parent
		
		$nuSpecFile = Join-Path $projectDir ([System.IO.Path]::GetFileNameWithoutExtension($projectFileName) + '.nuspec')
		if (! (Test-Path $nuSpecFile)){
			continue
		}
		
		Invoke-MSBuild @(
			"""$projectFileName"""
		)
		
		Invoke-NuGet @(
			"pack"
			"""$projectFileName"""
			"-Properties"
			"""Configuration=Release"""
			"-ExcludeEmptyDirectories"
			"-OutputDirectory"
			"""$tempDir"""
			"-Symbols"
		)
	}
	
	Publish-Artifacts $tempDir 'NuGet\Platform-Main'
}

Task Update-PlatformAssemblyInfo {
	$assemblyInfoPath = Join-Path $global:Context.Dir.Solution '..\..\Platform\Source\AssemblyInfo.Version.cs'
	Update-AssemblyInfo $assemblyInfoPath $global:Context.Version $global:Context.PackageVersion
}
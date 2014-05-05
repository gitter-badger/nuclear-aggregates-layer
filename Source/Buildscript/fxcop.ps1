Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module .\modules\msbuild.psm1 -DisableNameChecking
Import-Module .\modules\fxcop.psm1 -DisableNameChecking

$Configuration = 'Release'

Task Analyze-CoreTeam {

	$projectsDir1 = Join-Path $global:Context.Dir.Solution '..\..\Platform\Source'
	$projectsDir2 = Join-Path $global:Context.Dir.Solution '..\..\BLCore\Source'
	$projectsDirs = @($projectsDir1, $projectsDir2)
	$includeAssemblies = @('2Gis.Erm.Platform.*.dll', '2Gis.Erm.BLCore.*.dll')
	
	$includeProjects1 = @('*.csproj')
	$excludeProjects1 = @('*MsCRM*.csproj', '*Silverlight*.csproj', '*Tests*.csproj', '*Migrations*.csproj')
	Analyze-Projects $projectsDirs $includeProjects1 $excludeProjects1 $includeAssemblies -TargetFramework '.NETFramework,Version=v4.5.1'
	
	$includeProjects2 = @('*MsCRM*.csproj')
	$excludeProjects2 = @()
	Analyze-Projects $projectsDirs $includeProjects2 $excludeProjects2 $includeAssemblies -TargetFramework '.NETFramework,Version=v3.5'

	# TODO: сказать команде Core чтобы они сделали Reinstall на Prism
	#$includeProjects3 = @('*Silverlight*.csproj')
	#$excludeProjects3 = @()
	#Analyze-Projects $projectsDirs $includeProjects3 $excludeProjects3 $includeAssemblies -MsBuildPlatform 'x86' -TargetFramework 'Silverlight,Version=v5.0'
}

function Analyze-Projects ([string[]]$projectsDirs, $includeProjects, $excludeProjects, $includeAssemblies, $MsBuildPlatform = 'x64', $TargetFramework) {
	$projects = Get-ChildItem $projectsDirs -Include $includeProjects -Exclude $excludeProjects -Recurse
	$assembliesMap = @{}
	
	foreach($project in $projects){
	
		Invoke-MSBuild @(
			"$($project.FullName)"
		) -MsBuildPlatform $MsBuildPlatform -Configuration $Configuration
		
		Fill-AssembliesMap $assembliesMap $project.FullName $includeAssemblies
	}
	
	Invoke-FxCop $assembliesMap.Values -TargetFramework $TargetFramework
}

function Fill-AssembliesMap ([HashTable]$assembliesMap, $projectFullName, $includeAssemblies){
	$projectDir = Split-Path $projectFullName
	$excludeAssemblies = @('*.resources.dll')
	
	$assembliesDir = Join-Path $projectDir "bin\$Configuration"
	if (!(Test-Path $assembliesDir)){
		$assembliesDir = Join-Path $projectDir 'bin'
		if (!(Test-Path $assembliesDir)){
			throw "Can't find assemblies for project $projectFullName"
		}
	}

	$assemblies = Get-ChildItem $assembliesDir -Include $includeAssemblies -Exclude $excludeAssemblies -Recurse
	foreach($assembly in $assemblies){
		# using hashtable to make 'distinct' between repeating assemblies
		$assembliesMap["$($assembly.Name)"] = "/file:$($assembly.FullName)"
	}
}
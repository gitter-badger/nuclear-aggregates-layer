Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module .\modules\msbuild.psm1 -DisableNameChecking
Import-Module .\modules\fxcop.psm1 -DisableNameChecking

$Configuration = 'Release'

Task Analyze-CoreTeam {

	$projects = Get-Projects @('..\..\Platform\Source', '..\..\BLCore\Source')
	Build-Projects $projects
	
	$includeAssemblies1 = @(
		'2Gis.Erm.Platform.*'
		'2Gis.Erm.BLCore.*'
	)
	$excludeAssemblies1 = @(
		'*MsCRM*'
		'*Silverlight*'
	)
	
	Analyze-Projects $projects $includeAssemblies1 $excludeAssemblies1 -TargetFramework '.NETFramework,Version=v4.5.1'
	
	$includeAssemblies2 = @('*MsCRM*')
	$excludeAssemblies2 = @()
	Analyze-Projects $projects $includeAssemblies2 $excludeAssemblies2 -TargetFramework '.NETFramework,Version=v3.5'

	#$includeAssemblies3 = @('*Silverlight*')
	#$excludeAssemblies3 = @()
	#Analyze-Projects $projects $includeAssemblies3 $excludeAssemblies3 -TargetFramework 'Silverlight,Version=v5.0'
	
	Publish-FxCopReport
}

Task Analyze-BLTeam {

	$projects = Get-Projects @('..\..\BL\Source', '..\..\BLFlex\Source')
	Build-Projects $projects

	$includeAssemblies1 = @(
		'2Gis.Erm.BL.*'
		'2Gis.Erm.BLFlex.*'
	)
	$excludeAssemblies1 = @(
		'2Gis.Erm.BL.Reports*'
	)

	Analyze-Projects $projects $includeAssemblies1 $excludeAssemblies1 -TargetFramework '.NETFramework,Version=v4.5.1'
	Publish-FxCopReport
}

Task Analyze-ResearchTeam {

	$projects = Get-Projects @('..\..\BLQuerying\Source')
	Build-Projects $projects

	$includeAssemblies1 = @(
		'2Gis.Erm.BLQuerying.*'
		'2Gis.Erm.Qds.*'
	)
	$excludeAssemblies1 = @()

	Analyze-Projects $projects $includeAssemblies1 $excludeAssemblies1 -TargetFramework '.NETFramework,Version=v4.5.1'
	
	Publish-FxCopReport
}

Task Analyze-CompositionTeam {

	$projects = Get-Projects @('.')
	Build-Projects $projects

	$includeAssemblies1 = @(
		'2Gis.Erm.API.WCF.Operations.Special.dll'
		'2Gis.Erm.API.WCF.MoDi.dll'
		'2Gis.Erm.API.WCF.Releasing.dll'
		
		'2Gis.Erm.API.WCF.Metadata.dll'
		'2Gis.Erm.API.WCF.OrderValidation.dll'
		
		'2Gis.Erm.Migrator.exe'
		'2Gis.Erm.UI*'
	)
	$excludeAssemblies1 = @()

	Analyze-Projects $projects $includeAssemblies1 $excludeAssemblies1 -TargetFramework '.NETFramework,Version=v4.5.1'
	
	$includeAssemblies2 = @('2Gis.Erm.API.WCF.Operations.dll')
	$excludeAssemblies2 = @()
	Analyze-Projects $projects $includeAssemblies2 $excludeAssemblies2 -TargetFramework '.NETFramework,Version=v4.5.1'

	$includeAssemblies3 = @('2Gis.Erm.TaskService.exe')
	$excludeAssemblies3 = @()
	Analyze-Projects $projects $includeAssemblies3 $excludeAssemblies3 -TargetFramework '.NETFramework,Version=v4.5.1'

	Publish-FxCopReport
}

function Get-Projects ($solutionRelatedDirs){

	$projectsDirs = @()
	foreach($solutionRelatedDir in $solutionRelatedDirs){
		$projectsDirs += Join-Path $global:Context.Dir.Solution $solutionRelatedDir
	}

	$commonExcludeProjects = @('*Tests*.csporj')
	$projects = Get-ChildItem $projectsDirs -Include '*.csproj' -Exclude $commonExcludeProjects -Recurse
	$projects = $projects | Where { $_.DirectoryName -notmatch 'packages' }
	return $projects
}

function Build-Projects ($projects, $MsBuildPlatform = 'x64') {
	foreach($project in $projects){
		Invoke-MSBuild @(
			"$($project.FullName)"
		) -MsBuildPlatform $MsBuildPlatform -Configuration $Configuration
	}
}

function Analyze-Projects ($projects, $includeAssemblies, $excludeAssemblies, $TargetFramework) {

	$allExcludeAssemblies = @('*Resources*', '*Tests*', '*Migrations*') + $excludeAssemblies

	$assembliesMap = @{}
	foreach($project in $projects){
		Fill-AssembliesMap $assembliesMap $project.FullName $includeAssemblies $allExcludeAssemblies
	}
	
	if ($assembliesMap.Count -eq 0){
		throw "Can't find any assemblies include = $includeAssemblies, exclude = $allExcludeAssemblies"
	}
	
	Invoke-FxCop $assembliesMap.Values $TargetFramework
}

function Fill-AssembliesMap ([HashTable]$assembliesMap, $projectFullName, $includeAssemblies, $excludeAssemblies){
	$projectDir = Split-Path $projectFullName
	
	$assembliesDir = Join-Path $projectDir "bin\$Configuration"
	if (!(Test-Path $assembliesDir)){
		$assembliesDir = Join-Path $projectDir 'bin'
		if (!(Test-Path $assembliesDir)){
			throw "Can't find assemblies for project $projectFullName"
		}
	}

	$assemblies = Get-ChildItem $assembliesDir -Include $includeAssemblies -Exclude $excludeAssemblies -Recurse
	$assemblies = $assemblies | Where { @('.dll', '.exe') -contains $_.Extension }
	foreach($assembly in $assemblies){
		# using hashtable to make 'distinct' between repeating assemblies
		$assembliesMap["$($assembly.Name)"] = """/file:$($assembly.FullName)"""
	}
}
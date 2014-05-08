Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module .\modules\msbuild.psm1 -DisableNameChecking

$ThisDir = Split-Path $MyInvocation.MyCommand.Path
$RulesetPath = Join-Path $ThisDir 'fxcop.ruleset'

Task Analyze-CoreTeam {

	$projectDirs = @(
		'..\..\Platform\Source'
		'..\..\BLCore\Source'
	)
	
	$projects = Get-Projects $projectDirs -ExcludeProjects '*Silverlight*'
	Analyze-Projects $projects
	
	# silverlight
	$silverlightProjectFileName = Get-ProjectFileName '..\..\BLCore\Source\EntryPoints\UI\Web' '2Gis.Erm.BLCore.UI.Web.Silverlight'
	$silverlightProject = Get-Item $silverlightProjectFileName
	Analyze-Projects $silverlightProject -MsBuildPlatform 'x86'
	
	Publish-AnalysisReport ($projects + $silverlightProject)
}

Task Analyze-BLTeam {

	$projectDirs = @(
		'..\..\BL\Source'
		'..\..\BLFlex\Source'
	)

	$projects = Get-Projects $projectDirs -ExcludeProjects '*Reports*'
	Analyze-Projects $projects
	Publish-AnalysisReport $projects
}

Task Analyze-ResearchTeam {
	$projects = Get-Projects '..\..\BLQuerying\Source'
	Analyze-Projects $projects
	Publish-AnalysisReport $projects
}

Task Analyze-CompositionTeam {

	$projects = Get-Projects '.' -ExcludeProjects '*Mvc*'
	Analyze-Projects $projects
	
	# dependent from silverlight
	$mvcProjectFileName = Get-ProjectFileName '.' '2Gis.Erm.UI.Web.Mvc'
	$mvcProject = Get-Item $mvcProjectFileName
	Analyze-Projects $mvcProject -MsBuildPlatform 'x86'
	
	Publish-AnalysisReport ($projects + $mvcProject)
}

function Get-Projects ($solutionRelatedDirs, $excludeProjects){

	$projectsDirs = @()
	foreach($solutionRelatedDir in $solutionRelatedDirs){
		$projectsDirs += Join-Path $global:Context.Dir.Solution $solutionRelatedDir
	}
	
	$allExcludeProjects = @(
		'*Resources*'
		'*Tests*'
		'*Migrations*'
	) + $excludeProjects
	
	$projects = Get-ChildItem $projectsDirs -Include '*.csproj' -Exclude $allExcludeProjects -Recurse
	$projects = $projects | Where { $_.DirectoryName -notmatch 'packages' }
	return $projects
}

function Analyze-Projects ($projects, $MsBuildPlatform = 'x64') {

	$projectNames = $projects | Select-Object -Property Name
	Write-Output "Projects to analyze:"
	$projectNames

	foreach($project in $projects){
	
		Invoke-MSBuild @(
			"$($project.FullName)"
			
			'/t:Build'
			'/p:RunCodeAnalysis=true'
			"""/p:CodeAnalysisRuleSet=$RulesetPath"""
		) -MsBuildPlatform $MsBuildPlatform
	}
}

function Publish-AnalysisReport ($projects) {

	foreach($project in $projects){
	
		$successFile = Get-ChildItem $project.Directory.FullName -Filter '*.lastcodeanalysissucceeded' -Recurse
		if ($successFile -eq $null){
			throw "Error in code analysis for project $($project.FullName)"
		}
		if ($successFile -is [array]){
			throw "Found more than one success flie for project $($project.FullName)"
		}
		
		$analysisFilePath = [System.IO.Path]::ChangeExtension($successFile.FullName, 'CodeAnalysisLog.xml')
		if (!(Test-Path $analysisFilePath)){
			throw "Can't find code analysis file $analysisFilePath"
		}
		Write-Host "##teamcity[importData type='FxCop' path='$analysisFilePath']"	
	}
}
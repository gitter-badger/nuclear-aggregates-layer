Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module "$BuildToolsRoot\modules\msbuild.psm1" -DisableNameChecking

$RulesetPath = Join-Path $PSScriptRoot 'fxcop.ruleset'

$ExcludeProjects = @('*Resources*', '*Tests*', '*Migrations*')

Task Analyze-CoreTeam {

	$projectDirs = @(
		'..\..\Platform'
		'..\..\BLCore'
	)
	
	$projects = Find-Projects $projectDirs -Exclude ($ExcludeProjects + '*Silverlight*')
	Analyze-Projects $projects
	
	# silverlight
	$silverlightProjectFileName = Get-ProjectFileName '..\..\BLCore\Source\EntryPoints\UI\Web' '2Gis.Erm.BLCore.UI.Web.Silverlight'
	$silverlightProject = Get-Item $silverlightProjectFileName
	Analyze-Projects $silverlightProject -MsBuildPlatform 'x86'
	
	Publish-AnalysisReport ($projects + $silverlightProject)
}

Task Analyze-BLTeam {

	$projectDirs = @(
		'..\..\BL'
		'..\..\BLFlex'
	)

	$projects = Find-Projects $projectDirs -Exclude ($ExcludeProjects + '*Reports*')
	Analyze-Projects $projects
	Publish-AnalysisReport $projects
}

Task Analyze-ResearchTeam {

	$projectDirs = @('..\..\BLQuerying')

	$projects = Find-Projects $projectDirs -Exclude $ExcludeProjects
	Analyze-Projects $projects
	Publish-AnalysisReport $projects
}

Task Analyze-CompositionTeam {

	# NCrunch projects located in 'packages' folder
	$projects = Find-Projects '.' -Exclude ($ExcludeProjects + @('*Mvc*', '*NCrunch*'))
	Analyze-Projects $projects
	
	# dependent from silverlight
	$mvcProjectFileName = Get-ProjectFileName '.' '2Gis.Erm.UI.Web.Mvc'
	$mvcProject = Get-Item $mvcProjectFileName
	Analyze-Projects $mvcProject -MsBuildPlatform 'x86'
	
	Publish-AnalysisReport ($projects + $mvcProject)
}

function Analyze-Projects ($Projects, $MsBuildPlatform = 'x64') {

	foreach($project in $Projects){
	
		$buildFileName = Create-BuildFile $project.FullName -Properties @{
			'RunCodeAnalysis' = $true
			'CodeAnalysisRuleSet' = $RulesetPath
		}
	
		Invoke-MSBuild $buildFileName -MsBuildPlatform $MsBuildPlatform
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
		
		if (Test-Path 'Env:\TEAMCITY_VERSION') {
			Write-Host "##teamcity[importData type='FxCop' path='$analysisFilePath']"	
		}
	}
}
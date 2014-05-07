Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

$StaticAnalysisDir = "${Env:ProgramFiles(x86)}\Microsoft Visual Studio 12.0\Team Tools\Static Analysis Tools"
$FxCop_12_0 = Join-Path $StaticAnalysisDir '\FxCop\FxCopCmd.exe'

$ThisDir = Split-Path $MyInvocation.MyCommand.Path
$RulesetPath = Join-Path $ThisDir 'fxcop.ruleset'
$ChildRulesetDir = Join-Path $StaticAnalysisDir 'Rule Sets'

$TargetFrameworkArguments = @{
	'.NETFramework,Version=v4.5.1' = @('/searchgac')
	'.NETFramework,Version=v3.5' = @('/searchgac')
	'Silverlight,Version=v5.0' = @(
		"/directory:${Env:ProgramFiles(x86)}\Microsoft SDKs\Silverlight\v5.0\Libraries\Client"
	)
}

function Invoke-FxCop([string[]]$Arguments, $TargetFramework) {

	$outputDir = Join-Path $global:Context.Dir.Temp '\FxCop'
	if (!(Test-Path $outputDir)){
		md $outputDir | Out-Null
	}
	
	$randomFileName = [System.IO.Path]::GetRandomFileName()
	$randomFileName = [System.IO.Path]::ChangeExtension($randomFileName, 'xml')
	$outputPath = Join-Path $outputDir $randomFileName
	
	$targetFrameworkArguments = $TargetFrameworkArguments[$TargetFramework]
	if ($targetFrameworkArguments -eq $null){
		throw "Unsupported target framework $targetFramework"
	}

	$allArguments = @(
		"""/out:$outputPath"""
		'/outxsl:none'
		'/forceoutput'
		'/ignoregeneratedcode'
		
		'/culture:en-US'
		
		"""/ruleset:=$RulesetPath"""
		"""/rulesetdirectory:$ChildRulesetDir"""
		
		#'/verbose'
	) + $targetFrameworkArguments + $Arguments

	& $FxCop_12_0 $allArguments
	
	if ($lastExitCode -ne 0) {
		throw "Command failed with exit code $lastExitCode"
	}
}

function Publish-FxCopReport {
	
	$outputDir = Join-Path $global:Context.Dir.Temp '\FxCop'
	$reports = Get-ChildItem $outputDir -Filter '*.xml' | Sort-Object -Property 'Length' -Descending
	
	foreach($report in $reports){
		Write-Host "##teamcity[importData type='FxCop' path='$($report.FullName)']"	
	}
}

Export-ModuleMember -Function Invoke-FxCop, Publish-FxCopReport
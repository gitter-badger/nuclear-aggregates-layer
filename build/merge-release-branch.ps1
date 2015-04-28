Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module "$BuildToolsRoot\modules\git.psm1" -DisableNameChecking -Force
Import-Module "$PSScriptRoot\releaseLetter-sender.psm1" -DisableNameChecking -Force

Properties {
	$BranchList = $null
	$VersionNumber = $null
	$ReleaseName = $null
	$ReleaseNotesLink = $null
	$AdditionalInstrLink = $null
	$DeployDueDate = $null
}

Task Merge-ReleaseBranch {
	$commonMetadata = Get-Metadata 'Common'
	
	[string]$ermDir = Join-path $commonMetadata.Dir.Solution "..\.."
	
	# COMMENT FOR LOCAL DEBUG
	[string]$branchName = $commonMetadata.Branch
	[string]$branchList = $BranchList
	[string]$versionNumber = $VersionNumber
	[string]$releaseName = $ReleaseName
	[string]$releaseNotesLink = $ReleaseNotesLink
	[string]$additionalInstrLink = $AdditionalInstrLink
	[string]$deployDueDate = $DeployDueDate
	# COMMENT FOR LOCAL DEBUG
		
	# UNCOMMENT FOR LOCAL DEBUG
	# $branchName = 'feature/ERM-5715-ReleaseFinishAutomation'
	# $branchList = 'release/ERM-5810-HelpMe, release/ERM-6155-TurnOffOrderValidationRule, release/ERM-5942-PrintForm'
	# $versionNumber = '2.78.0'
	# $releaseName = 'FoolsGold'
	# $releaseNotesLink = 'https://confluence.2gis.ru/pages/viewpage.action?pageId=153358308'
	# $additionalInstrLink = ''
	# $deployDueDate = ''
	# UNCOMMENT FOR LOCAL DEBUG

	if (!$branchName -eq 'master'){
		throw "You should not merge master in master. It's ridiculous."
	}

	if ([String]::IsNullOrEmpty($branchList)){
		$branchList = $branchName
	}

	"Fetching"

	Invoke-Git $ermDir @(
		'fetch'
		'--all'
	)

	"Checking out branch {0}" -f $branchName

	Invoke-GitSilent $ermDir @(
		'checkout'
		$branchName
		)

	# Обновляем номер версии
	$setVersionScriptPath = Join-Path $ermDir 'CompositionRoots\Source\set-version.ps1'
	. $setVersionScriptPath $versionNumber


	$commitResult = Invoke-Git $ermDir @(
		'commit'
		'-a'
	    '-m'
	    '[NA] Обновлён номер версии'
		)
	$encFrom = [System.Text.Encoding]::GetEncoding("CP866")
	$encTo = [System.Text.Encoding]::GetEncoding("UTF-8")
	$bytesFrom = $encFrom.GetBytes($commitResult)
	$commitResultAfterEncoding = $encTo.GetString($bytesFrom)
	$commitResultAfterEncoding

	"Merging branch {0} to for master" -f $branchName

	Invoke-GitSilent $ermDir @(
	    'checkout'
	    'master'
		)

	$mergeResult = Invoke-Git $ermDir @(
		'merge'
		$branchName
	    '--no-ff'
	    '-m'
		"Merge branch '" + $branchName + "'"
		)
	$mergeResult

	#Смотрим SHA последнего коммита в master
	$masterLastSHA = Invoke-Git $ermDir @(
		'log'
		'master'
	    "--pretty=format:'%h'"
		'-n'
		'1'
		)

	"Creating tag"

	$tagName = $versionNumber + '-' + $releaseName

	$featureList = $branchList.Split(@(',')) | ForEach{ $_.Trim().Split('/')[1] }

	$featureListInString = $featureList -join ', '

	$tagMessage = 'Finish ' + $featureListInString

	Invoke-Git $ermDir @(
		'tag'
		'-a'
		$tagName
	    '-m'
		$tagMessage
		)

	"Pushing commits for branch {0}" -f $branchName

	Invoke-GitSilent $ermDir @(
		'push'
		'origin'
	    $branchName
		)
		
	"Pushing commits for master"

	Invoke-GitSilent $ermDir @(
		'push'
		'origin'
	    'master'
		)

	"Pushing tag"

	Invoke-GitSilent $ermDir @(
		'push'
		'origin'
	    $tagName
		)

	"Sending release e-mail"
	Send-ReleaseLetter $versionNumber $releaseName $deployDueDate $releaseNotesLink $additionalInstrLink $masterLastSHA $branchList
}
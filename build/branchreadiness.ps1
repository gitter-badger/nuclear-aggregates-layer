Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

[string]$masterName = 'master'

Import-Module "$BuildToolsRoot\modules\git.psm1" -DisableNameChecking -Force

Task Check-BranchReadiness {
	$commonMetadata = Get-Metadata 'Common'
	
	[string]$ermDir = Join-path $commonMetadata.Dir.Solution "..\.."
	
	# COMMENT FOR LOCAL DEBUG
	[string]$branchName = $commonMetadata.Branch
	# COMMENT FOR LOCAL DEBUG
		
	# UNCOMMENT FOR LOCAL DEBUG
	#$branchName = 'feature/ERM-5715-ReleaseFinishAutomation'
	# UNCOMMENT FOR LOCAL DEBUG
	
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

	"Checking branch {0} actualization from master" -f $branchName

	$mergedBranchList = Invoke-Git $ermDir @(
		'branch'
		'-a'
	    '--merged'
	)

	$branchListWithoutSpaces = $mergedBranchList|ForEach{ $_.Trim() }

	if (!$branchListWithoutSpaces.Contains('remotes/origin/' + $masterName)){
		throw "Master was not merged into branch $BranchName"
	}
	
	"Branch {0} is actual" -f $branchName
}
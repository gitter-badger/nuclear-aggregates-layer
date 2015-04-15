Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module "$BuildToolsRoot\modules\git.psm1" -DisableNameChecking -Force

Task Check-Branch-Readiness {
	[string]$ermDir = Join-path $global:Context.Dir.Solution "..\.."	
	
	# COMMENT FOR LOCAL DEBUG
	[string]$branchName = $global:Context.Branch
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

	if (!$branchListWithoutSpaces.Contains('remotes/origin/master')){
		throw "Master was not merged into branch $BranchName"
	}
}
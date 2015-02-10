Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

if (Test-Path 'Env:\TEAMCITY_VERSION'){
	$host.UI.RawUI.BufferSize = New-Object System.Management.Automation.Host.Size(8192, 50)
}

Import-Module .\modules\nuget.psm1 -DisableNameChecking

$PackageInfo = Get-PackageInfo 'psake'
Import-Module "$($PackageInfo.VersionedDir)\tools\psake.psm1" -DisableNameChecking -Force

function Create-GlobalContext ($Properties) {

	$global:Context = @{}

	if ($Properties.ContainsKey('Dirs')){
		$global:Context.Add('Dirs', $Properties['Dirs'])
		$Properties.Remove('Dirs')
	}

	if (!(Test-Path $global:Context.Dirs.Solution)){
		throw "Can't find solution dir '$($global:Context.Dirs.Solution)'"
	}

	if (Test-Path $global:Context.Dirs.Temp){
		rd $global:Context.Dirs.Temp -Recurse -Force | Out-Null
	}
	md $global:Context.Dirs.Temp | Out-Null

	if (Test-Path $global:Context.Dirs.Artifacts){
		rd $global:Context.Dirs.Artifacts -Recurse -Force | Out-Null
	}
	md $global:Context.Dirs.Artifacts | Out-Null
	
	
	if ($Properties.ContainsKey('Revision')){
		$global:Context.Add('Revision', $Properties['Revision'])
		$Properties.Remove('Revision')
	}
	if ($Properties.ContainsKey('Build')){
		$global:Context.Add('Build', $Properties['Build'])
		$Properties.Remove('Build')
	}
	if ($Properties.ContainsKey('Branch')){
		$global:Context.Add('Branch', $Properties['Branch'])
		$Properties.Remove('Branch')
	}
	if ($Properties.ContainsKey('EnvironmentName')){
		$global:Context.Add('EnvironmentName', $Properties['EnvironmentName'])
		$Properties.Remove('EnvironmentName')
	}
}

function Run-Build ($BuildFile, $TaskList, $Properties) {

	Create-GlobalContext $Properties

	Invoke-psake $BuildFile `
	-nologo `
	-taskList $TaskList `
	-properties $Properties
	
	Exit [int]!$psake.build_success
}

Export-ModuleMember -Function Run-Build
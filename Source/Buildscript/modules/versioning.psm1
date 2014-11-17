#------ Указать версию --------
$GlobalVersion = @{
	'Major' = 2
	'Minor' = 48
	'Patch' = 0
}
#------------------------------

$DeveloperAssemblyVersion = @{
	'AssemblyVersion' = '2.0.0'
}

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

Import-Module .\modules\metadata.psm1 -DisableNameChecking

function Get-Version {

	$build = [int]$global:Context.Build
	$revision = [string]$global:Context.Revision
	# git shorthash contains 8 letters
	if ($revision.Length -gt 8){
		$revision = $revision.Substring(0, 8)
	}

	$semanticVersion = '{0}.{1}.{2}' -f $GlobalVersion.Major, $GlobalVersion.Minor, $GlobalVersion.Patch
	
	$commonMetadata = Get-EntryPointMetadata 'Common'
	if (@('Production', 'Edu') -notcontains $commonMetadata.EnvType){
		$branch = Get-Branch
		$semanticVersion += '-{0}.{1}.{2}' -f $branch, $revision, $build
	}
	
	return @{
		'NumericVersion' = New-Object System.Version($GlobalVersion.Major, $GlobalVersion.Minor, $build)
		'SemanticVersion' = $semanticVersion
	}
}

function Update-AssemblyInfo ($AssemblyInfoPaths)  {

	$version = Get-Version

	foreach($AssemblyInfoPath in $AssemblyInfoPaths){
	
	    $file = Get-Item $AssemblyInfoPath
	    $file.IsReadOnly = $false
		
		$content = Get-Content -Encoding UTF8 -Path $AssemblyInfoPath
		
		$content = $content -replace 'AssemblyVersion\(".*"\)', "AssemblyVersion(""$($DeveloperAssemblyVersion.AssemblyVersion)"")"
		$content = $content -replace 'SatelliteContractVersion\(".*"\)', "SatelliteContractVersion(""$($DeveloperAssemblyVersion.AssemblyVersion)"")"

		$content = $content -replace 'AssemblyInformationalVersion\(".*"\)', "AssemblyInformationalVersion(""$($version.SemanticVersion)"")"
		$content = $content -replace 'AssemblyFileVersion\(".*"\)', "AssemblyFileVersion(""$($version.NumericVersion)"")"
		
		$content = $content -replace 'SemanticVersion\s*=\s*".*"', "SemanticVersion = ""$($version.SemanticVersion)"""
		$content = $content -replace 'Build\s*=\s*".*"', "Build = ""$($version.NumericVersion.Build)"""
		
		Set-Content -Path $AssemblyInfoPath -Value $content -Encoding UTF8
	}
}

function Get-Branch {
	$branch = [int]$global:Context.Branch.Trim('/')
	
	$slashIndex = $branch.LastIndexOf('/')
	if ($slashIndex -ne -1){
		$branch = $branch.Substring($slashIndex + 1)
	}
	
	return $branch
}

Export-ModuleMember -Function Get-Version, Get-Branch, Update-AssemblyInfo
#------ Указать версию --------
$GlobalVersion = @{
	'Major' = 2
	'Minor' = 65
	'Patch' = 0
}
#------------------------------

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

Import-Module .\modules\metadata.psm1 -DisableNameChecking

function Get-Version {

	$semanticVersion = '{0}.{1}.{2}' -f $GlobalVersion.Major, $GlobalVersion.Minor, $GlobalVersion.Patch
	
	$commonMetadata = Get-EntryPointMetadata 'Common'
	if (@('Production', 'Edu') -notcontains $commonMetadata.EnvType){
		$buildMetadata = Get-BuildMetadata
		if (![string]::IsNullOrEmpty($buildMetadata)){
			$semanticVersion += '-' + $buildMetadata
		}
	}
	
	return @{
		'NumericVersion' = New-Object System.Version($GlobalVersion.Major, $GlobalVersion.Minor, [int]$global:Context.Build)
		'SemanticVersion' = $semanticVersion
	}
}

function Update-AssemblyInfo ($AssemblyInfos)  {

	$version = Get-Version

	foreach($assemblyInfo in $AssemblyInfos){
	
		$content = Get-Content -Encoding UTF8 -Path $assemblyInfo.FullName
		
		$content = $content -replace 'AssemblyInformationalVersion\(".*"\)', "AssemblyInformationalVersion(""$($version.SemanticVersion)"")"
		$content = $content -replace 'AssemblyFileVersion\(".*"\)', "AssemblyFileVersion(""$($version.NumericVersion)"")"
		
		$content = $content -replace 'SemanticVersion\s*=\s*".*"', "SemanticVersion = ""$($version.SemanticVersion)"""
		$content = $content -replace 'Build\s*=\s*".*"', "Build = ""$($version.NumericVersion.Build)"""
		
		Set-Content -Path $assemblyInfo.FullName -Value $content -Encoding UTF8
	}
}

function Get-BuildMetadata{
	$buildMetadata = $null

	$branch = Get-Branch
	if (![string]::IsNullOrEmpty($branch)){
		if (![string]::IsNullOrEmpty($buildMetadata)){
			$buildMetadata += '-'
		}
		$buildMetadata += $branch
	}
	
	$revision = Get-Revision
	if (![string]::IsNullOrEmpty($revision)){
		if (![string]::IsNullOrEmpty($buildMetadata)){
			$buildMetadata += '-'
		}
		$buildMetadata += $revision
	}

	$build = [int]$global:Context.Build
	if (![string]::IsNullOrEmpty($buildMetadata)){
		$buildMetadata += '-'
	}
	$buildMetadata += $build

	return $buildMetadata
}

function Get-Revision {
	$revision = [string]$global:Context.Revision
	
	if ($revision.Length -gt 6){
		$revision = $revision.Substring(0, 6)
	}

	return $revision
}

function Get-Branch {
	$branch = [string]$global:Context.Branch
	
	# trim branch prefix
	$slashIndex = $branch.LastIndexOf('/')
	if ($slashIndex -ne -1){
		$branch = $branch.Substring($slashIndex + 1)
	}
	
	if ($branch.Length -gt 8){
		$branch = $branch.Substring(0, 8)
	}
	
	return $branch
}

Export-ModuleMember -Function Get-Version, Get-Branch, Update-AssemblyInfo
Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

function Update-AssemblyInfo ($AssemblyInfoPath, [System.Version]$Version, [string]$PackageVersion)  {

    $file = Get-Item $AssemblyInfoPath
    $file.IsReadOnly = $false

	$content = Get-Content -Encoding UTF8 -Path $AssemblyInfoPath
	
    # (3 numbers, ends with zero)
	$assemblyVersion = 'AssemblyVersion("{0}.{1}.{2}.0")' -f $Version.Major, $Version.Minor, $Version.Build
    $content = $content -replace 'AssemblyVersion\(".*"\)', $assemblyVersion
	
    # (3 numbers, ends with zero)
	$satelliteContractVersion = 'SatelliteContractVersion("{0}.{1}.{2}.0")' -f $Version.Major, $Version.Minor, $Version.Build
    $content = $content -replace 'SatelliteContractVersion\(".*"\)', $satelliteContractVersion

    # (4 numbers)
	$assemblyFileVersion = 'AssemblyFileVersion("{0}.{1}.{2}.{3}")' -f $Version.Major, $Version.Minor, $Version.Build, $Version.Revision
    $content = $content -replace 'AssemblyFileVersion\(".*"\)', $assemblyFileVersion

    # (3 numbers OR package version)
	$assemblyInformationalVersion = 'AssemblyInformationalVersion("{0}.{1}.{2}")' -f $Version.Major, $Version.Minor, $Version.Build
	if (![string]::IsNullOrEmpty($PackageVersion)){
		$assemblyInformationalVersion = 'AssemblyInformationalVersion("{0}")' -f $PackageVersion
	}
    $content = $content -replace 'AssemblyInformationalVersion\(".*"\)', $assemblyInformationalVersion

	Set-Content -Path $AssemblyInfoPath -Value $content -Encoding UTF8
}

function Get-VersionFileName ($Dir, [System.Version] $Version) {

	$fileName = ('version_{0}_{1}_{2}_{3}' -f $Version.Major, $Version.Minor, $Version.Build, $Version.Revision)
	$filePath = Join-Path $Dir $fileName
	
	Set-Content -Path $filePath -Value $Version.ToString(4) -Encoding UTF8
	
	return $filePath
}

Export-ModuleMember -Function Update-AssemblyInfo, Get-VersionFileName
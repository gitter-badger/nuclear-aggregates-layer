Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

function Update-AssemblyInfo ($AssemblyInfoPath, [System.Version] $Version)  {

    $file = Get-Item $AssemblyInfoPath
    $file.IsReadOnly = $false

	$content = Get-Content -Encoding UTF8 -Path $AssemblyInfoPath
	
    # update assemblies version (have 3 numbers)
    $content = $content -replace '(AssemblyVersion|AssemblyVersionAttribute)\("([0-9]+)\.([0-9]+)\.([0-9]+)\.([0-9]+)?"\)' , ('$1("{0}.{1}.{2}.0")' -f $Version.Major, $Version.Minor, $Version.Build)
	
    # update satellites version (have 3 numbers)
    $content = $content -replace '(SatelliteContractVersion|SatelliteContractVersionAttribute)\("([0-9]+)\.([0-9]+)\.([0-9]+)\.([0-9]+)?"\)' , ('$1("{0}.{1}.{2}.0")' -f $Version.Major, $Version.Minor, $Version.Build)

    # update assemblies file version (have 4 numbers)
    $content = $content -replace '(AssemblyFileVersion|AssemblyFileVersionAttribute)\("([0-9]+)\.([0-9]+)\.([0-9]+)\.([0-9]+)?"\)' , ('$1("{0}.{1}.{2}.{3}")' -f $Version.Major, $Version.Minor, $Version.Build, $Version.Revision)

    # update product version (have only 3 numbers)
    $content = $content -replace '(AssemblyInformationalVersion|AssemblyInformationalVersionAttribute)\("([0-9]+)\.([0-9]+)\.([0-9]+)?"\)' , ('$1("{0}.{1}.{2}")' -f $Version.Major, $Version.Minor, $Version.Build)

	Set-Content -Path $AssemblyInfoPath -Value $content -Encoding UTF8
}

function Get-VersionFileName ($Dir, [System.Version] $Version) {

	$fileName = ('version_{0}_{1}_{2}_{3}' -f $Version.Major, $Version.Minor, $Version.Build, $Version.Revision)
	$filePath = Join-Path $Dir $fileName
	
	Set-Content -Path $filePath -Value $Version.ToString(4) -Encoding UTF8
	
	return $filePath
}

Export-ModuleMember -Function Update-AssemblyInfo, Get-VersionFileName
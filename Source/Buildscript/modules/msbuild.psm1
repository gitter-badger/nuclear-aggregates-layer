Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

$Configuration = 'Release'
$VisualStudioVersion = '12.0'

#$MsBuild4x86 = "${Env:SystemRoot}\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"
#$MsBuild4x64 = "${Env:SystemRoot}\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe"

$MsBuild12x86 = "${Env:ProgramFiles(x86)}\MSBuild\12.0\Bin\MSBuild.exe"
$MsBuild12x64 = "${Env:ProgramFiles(x86)}\MSBuild\12.0\Bin\amd64\MSBuild.exe"

function Invoke-MSBuild ($MsBuildPlatform = 'x64', [string[]]$Arguments){

		$verbosity = $global:Context.Verbosity
		
		$allArguments = $Arguments + @(
		"/nologo"
		"/m"
		"/verbosity:$verbosity"
		"/p:Configuration=$Configuration"
		"/p:VisualStudioVersion=$VisualStudioVersion"
		)

		switch ($MsBuildPlatform){
			'x86' {
				Exec {
					& $MsBuild12x86 $allArguments
				}
			}
			'x64' {
				Exec {
					& $MsBuild12x64 $allArguments
				}
			}
			default {
				throw "Platform is not defined"
			}
		}

}

function Get-ProjectFileName($ProjectDir, $ProjectName, $Extension = '.csproj'){
	return Join-Path $global:Context.Dir.Solution (Join-Path $ProjectDir (Join-Path  $ProjectName ($ProjectName + $Extension)))
}

function Get-PackageFileName ($ProjectFileName, $Extension = $null) {
	$projectName = [System.IO.Path]::GetFileNameWithoutExtension($ProjectFileName)
	
	$packageFileName = $projectName + '-' + $global:Context.Version.ToString(3)
	
	if ($Extension -ne $null){
		$packageFileName = $packageFileName + '.' + $Extension
	}
	
	$packageFileName = Join-Path $global:Context.Dir.Artifacts $packageFileName
	return $packageFileName
}

Export-ModuleMember -Function Invoke-MSBuild, Get-ProjectFileName, Get-PackageFileName
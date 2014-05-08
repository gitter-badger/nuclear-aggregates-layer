Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

$MSBuildSdk_12_0 = "${Env:ProgramFiles(x86)}\MSBuild\12.0\Bin"
$MsBuild_12_0_x86 = Join-Path $MSBuildSdk_12_0 'MSBuild.exe'
$MsBuild_12_0_x64 = Join-Path $MSBuildSdk_12_0 'amd64\MSBuild.exe'

$VisualStudioVersion = '12.0'
$Configuration = 'Release'

function Invoke-MSBuild ([string[]]$Arguments, $MsBuildPlatform = 'x64'){

		$allArguments = @(
		"/nologo"
		"/m"
		'/consoleloggerparameters:ErrorsOnly'
		"/p:Configuration=$Configuration"
		"/p:VisualStudioVersion=$VisualStudioVersion"
		) + $Arguments

		switch ($MsBuildPlatform){
			'x86' {
				& $MsBuild_12_0_x86 $allArguments
			}
			'x64' {
				& $MsBuild_12_0_x64 $allArguments
			}
			default {
				throw "MSBuild platform (x86, x64) is not defined"
			}
		}

		if ($lastExitCode -ne 0) {
			throw "Command failed with exit code $lastExitCode"
		}
}

function Get-ProjectFileName($ProjectDir, $ProjectName, $Extension = '.csproj'){

	$projectFileName = Join-Path $global:Context.Dir.Solution (Join-Path $ProjectDir (Join-Path  $ProjectName ($ProjectName + $Extension)))
	if (!(Test-Path $projectFileName)){
		throw "Cannot find file '$projectFileName'"
	}

	return $projectFileName
}

function Get-Artifacts ($RelativeArtifactDir, $FileName) {

	if ([string]::IsNullOrEmpty($FileName)){
	
		$versionedDirName = Get-VersionedDirName $RelativeArtifactDir
		$artifactName = Join-Path $global:Context.Dir.Artifacts $versionedDirName
		
	} else {
	
		$versionedFileName = Get-VersionedFileName $FileName
		$artifactDirName = Join-Path $global:Context.Dir.Artifacts $RelativeArtifactDir
		$artifactName = Join-Path $artifactDirName $versionedFileName
	}
	
	if (!(Test-Path $artifactName)){
		throw "Cannot find artifact $artifactName"
	}
	
	return $artifactName
}

function Publish-Artifacts ($Path, $RelativeArtifactDir) {

	$item = Get-Item $Path
	if ( $item.Attributes.HasFlag([System.IO.FileAttributes]'Directory')){
		
		$emptyTest = Get-ChildItem $Path
		if ($emptyTest -eq $null){
			throw "Directory empty $path"
		}
	
		$versionedDirName = Get-VersionedDirName $RelativeArtifactDir
		$artifactDirName = Join-Path $global:Context.Dir.Artifacts $versionedDirName

		Copy-Item $item $artifactDirName -Force -Recurse
		Write-Output "##teamcity[publishArtifacts '$artifactDirName => $versionedDirName']"

	} else {
	
		$versionedFileName = Get-VersionedFileName $item
		$artifactDirName = Join-Path $global:Context.Dir.Artifacts $RelativeArtifactDir
		if (!(Test-Path $artifactDirName)){
			md $artifactDirName | Out-Null
		}
		
		$artifactFileName = Join-Path $artifactDirName $versionedFileName
		
		Copy-Item $item $artifactFileName -Force -Recurse
		Write-Output "##teamcity[publishArtifacts '$artifactFileName => $RelativeArtifactDir']"
	}
}

function Get-VersionedFileName ($FileName) {

	$fileNameWithoutExtension = [System.IO.Path]::GetFileNameWithoutExtension($FileName)
	$versionedFileName = $fileNameWithoutExtension + '-' + $global:Context.Version.ToString(3)
	
	$extension = [System.IO.Path]::GetExtension($FileName)
	$versionedFileName += $extension
		
	return $versionedFileName
}

function Get-VersionedDirName ($DirName) {

	if ([string]::IsNullOrEmpty($DirName)){
		return $DirName
	}

	$versionedDirName = $DirName + '-' + $global:Context.Version.ToString(3)
	return $versionedDirName
}

Export-ModuleMember -Function Invoke-MSBuild, Get-ProjectFileName, Get-Artifacts, Publish-Artifacts
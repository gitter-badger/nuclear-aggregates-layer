﻿Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module .\modules\versioning.psm1 -DisableNameChecking

$MSBuildVersion = '12.0'
$VisualStudioVersion = '12.0'
$Configuration = 'Release'

$MSBuildDir = "${Env:ProgramFiles(x86)}\MSBuild\$MSBuildVersion\Bin"
$MsBuildPath_x86 = Join-Path $MSBuildDir 'MSBuild.exe'
$MsBuildPath_x64 = Join-Path $MSBuildDir 'amd64\MSBuild.exe'

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
				& $MsBuildPath_x86 $allArguments
			}
			'x64' {
				& $MsBuildPath_x64 $allArguments
			}
			default {
				throw "MSBuild platform (x86, x64) is not defined"
			}
		}

		if ($lastExitCode -ne 0) {
			throw "Command failed with exit code $lastExitCode"
		}
}

# Invoke-MSBuild -ProjectFileName -Target -Properties
function Create-BuildProj ($ProjectFileName, $Targets = $null, $Properties = $null){

	if ($Targets -eq $null -and $Properties -eq $null){
		return $ProjectFileName
	}

	$content = [xml]@"
<?xml version="1.0" encoding="utf-8"?>
<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003" />
"@
	$root = $content.DocumentElement

	if ($Targets -ne $null){
		$root.SetAttribute('DefaultTargets', )
		$TargetsXml = '="{0}"' -f [string]::Join
	}

	$propertyGroup
	foreach($Property in $Properties){
		
	}

	$buildProjFileName = [System.IO.Path]::ChangeExtension($ProjectFileName, '.buildproj')
	Set-Content $buildProjFileName $content -Encoding UTF8
	return $buildProjFileName
}

function Get-ProjectFileName ($ProjectDir, $ProjectName, $Extension = '.csproj'){

	$projectFileName = Join-Path $global:Context.Dir.Solution (Join-Path $ProjectDir (Join-Path  $ProjectName ($ProjectName + $Extension)))
	if (!(Test-Path $projectFileName)){
		throw "Cannot find file '$projectFileName'"
	}

	return $projectFileName
}

function Find-Projects ($ProjectDirs, $Include, $Exclude, $Filter = '*.csproj'){

	$solutionProjectDirs = $ProjectDirs | Select-Object @{Name='Expand'; Expression = {
		$solutionProjectDir = Join-Path $global:Context.Dir.Solution $_
		if (!(Test-Path $solutionProjectDir)){
			throw "Can't find project dir $solutionProjectDir"
		}
		return $solutionProjectDir
	}} | Select-Object -ExpandProperty 'Expand'

	$projects = Get-ChildItem $solutionProjectDirs -Filter $Filter -Include $Include -Exclude $Exclude -Recurse
	
	$projectNames = $projects | Select-Object -ExpandProperty 'Name'
	Write-Host "Found projects:" $projectNames -Separator "`n"
	
	return $projects
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
		
		if (Test-Path 'Env:\TEAMCITY_VERSION') {
			Write-Host "##teamcity[publishArtifacts '$artifactDirName => $versionedDirName']"
		}

	} else {
	
		$versionedFileName = Get-VersionedFileName $item
		$artifactDirName = Join-Path $global:Context.Dir.Artifacts $RelativeArtifactDir
		if (!(Test-Path $artifactDirName)){
			md $artifactDirName | Out-Null
		}
		
		$artifactFileName = Join-Path $artifactDirName $versionedFileName
		
		Copy-Item $item $artifactFileName -Force -Recurse
		if (Test-Path 'Env:\TEAMCITY_VERSION') {
			Write-Host "##teamcity[publishArtifacts '$artifactFileName => $RelativeArtifactDir']"
		}
	}
}

function Get-VersionedFileName ($FileName) {

	$fileNameWithoutExtension = [System.IO.Path]::GetFileNameWithoutExtension($FileName)
	$versionedFileName = $fileNameWithoutExtension + '-' + $global:Context.EnvironmentName + '-' + (Get-Version).SemanticVersion
	
	$extension = [System.IO.Path]::GetExtension($FileName)
	$versionedFileName += $extension
		
	return $versionedFileName
}

function Get-VersionedDirName ($DirName) {

	if ([string]::IsNullOrEmpty($DirName)){
		return $DirName
	}

	$versionedDirName = $DirName + '-' + $global:Context.EnvironmentName + '-' + (Get-Version).SemanticVersion
	return $versionedDirName
}

Export-ModuleMember -Function Invoke-MSBuild, Get-ProjectFileName, Find-Projects, Get-Artifacts, Publish-Artifacts
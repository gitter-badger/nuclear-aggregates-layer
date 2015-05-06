param ([string]$version)

if ([string]::IsNullOrEmpty($version)){
	throw "Version is not set"
}

$buildFile = Join-Path $PSScriptRoot '..\..\build\run-build.ps1'
if (!(Test-Path $buildFile)){
	throw "Cannot find file $buildFile"
}

$content = Get-Content $buildFile
$newContent = $content -replace "Version.=.'(.*)'", "Version = '$version'"
if ($newContent -ne $content) {
	Set-Content $buildFile $newContent
	Write-Host "SemanticVersion changed to $version"
}
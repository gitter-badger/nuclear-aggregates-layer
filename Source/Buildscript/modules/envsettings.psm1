Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

$ThisDir = Split-Path $MyInvocation.MyCommand.Path -Parent

# AppName = {'erm', 'quartz', 'lon4net'}
function Get-EnvironmentTransformations ($AppName) {
	$commonDirectory = Join-Path $ThisDir '../../Environments/Common'
	$templateDirectory = Join-Path $ThisDir '../../Environments/Templates'
	$overridesDirectory = Join-Path $ThisDir '../../Environments'
	$tokens = $global:Context.EnvironmentName -Split '\.'
	if ($tokens.Length -ne 2) {
		throw 'EnvironmentName should contain Configuration and Number parts'
	}

	$configuration = $tokens[0]

    $transformations = @()

	$transformations += Get-ChildItem $commonDirectory -Filter "$AppName.Release.config"

	# template transformations are only applied for test configurations
	if ($configuration -eq 'Test') {
		$templateName = Resolve-TemplateName
		$transformations += Get-ChildItem $templateDirectory -Filter "$AppName.$configuration.$templateName.config" 
	}

	$transformations += Get-ChildItem $overridesDirectory -Filter "$AppName.$($global:Context.EnvironmentName).config"

	return $transformations | Select-Object -ExpandProperty FullName
}

function Resolve-TemplateName ()  {
	$tokens = $global:Context.EnvironmentName -Split '\.'
	if ($tokens.Length -ne 2) {
		throw 'EnvironmentName should contain Configuration and Number parts'
	}

	$number = $tokens[1]

	if ($number -in 1,2,3,4,5,6) {
		return 'Russia.CRM'
	}

	if ($number -in 1..100) {
		return 'Russia'
	}

	if ($number -in 101..200) {
		return 'Cyprus'
	}

    if ($number -in 201..300) {
		return 'Czech'
	}

	if ($number -in 301..400) {
		return 'Chile'
	}

	if ($number -in 401..500) {
		return 'Ukraine'
	}

	if ($number -in 501..600) {
		return 'Emirates'
	}
	    
	# return $Number
    throw "Cant't resolve template name for environment [$Number]"
}

function Get-EnvironmentSettings () {
	$tokens = $global:Context.EnvironmentName -Split '\.'
	if ($tokens.Length -ne 2) {
		throw 'EnvironmentName should contain Configuration and Number parts'
	}

	$configuration = $tokens[0]
	$number = $tokens[1]

	return @{ EnvNum = $number }
}

Export-ModuleMember -Function Get-EnvironmentTransformations, Get-EnvironmentSettings
Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

[string[]]$adminEmail = ("d.sidelnikov@2gis.ru", "m.kukushkin@2gis.ru")
$supportEmail = ("a.sapotko@2Gis.ru", "m.shemetov@2gis.ru")
$copyTo = ("erm.test@2Gis.ru", "erm.dev@2Gis.ru", "bit@2Gis.ru")
$hotfixLetterTemplateName = "Hotfix.htm"
$hotfixLetterWithAddTemplateName = "Hotfix_With_AddInfo.htm"
$releaseLetterTemplateName = "Release.htm"
$releaseLetterWithAddTemplateName = "Release_With_AddInfo.htm"

Import-Module "$BuildToolsRoot\modules\email.psm1" -DisableNameChecking -Force

function Send-ReleaseLetter ([string]$versionNumber, [string]$releaseName, [string]$deployDueDate, [string]$releaseNotesLink, [string]$additionalInstrLink, [string]$masterSHA, [string]$releasedBranches){

	$releaseFullName = $versionNumber + '-' + $releaseName

	if ($releaseName.Contains("Hotfix")) {
		$messageSubject = $releaseFullName + " для России, Кипра, Чехии, Чили, Украины, ОАЭ и Казахстана"
		if ([String]::IsNullOrEmpty($additionalInstrLink)) {
			$letterTemplateName = $hotfixLetterTemplateName
	        }
	    else {
			$letterTemplateName = $hotfixLetterWithAddTemplateName
			}
		}
	else{
		$messageSubject = "Релиз ERM " + $releaseFullName + " для России, Кипра, Чехии, Чили, Украины, ОАЭ и Казахстана"
	    if ([String]::IsNullOrEmpty($additionalInstrLink)) {
	        $letterTemplateName = $releaseLetterTemplateName
	        }
	    else {
	        $letterTemplateName = $releaseLetterWithAddTemplateName
	        }
		}

	$smtpTo = $adminEmail + $supportEmail

	if ([String]::IsNullOrEmpty($deployDueDate)) {
		$today = Get-Date
		$deployDate = '{0:dd.MM.yyyy}' -f $today.AddDays("$(1+$(@(1,2-eq7-$today.dayofweek)))")
	}
	else{
		$deployDate = $deployDueDate
	}

	$letterTemplateDir = Join-Path $PSScriptRoot "letter_templates"
	$letterTemplatePath = Join-Path $letterTemplateDir $letterTemplateName

	[string]$messagebody = Get-Content $letterTemplatePath -Encoding UTF8

	$messagebody = $messagebody -replace "{{RELEASE_NAME}}", $releaseFullName
	$messagebody = $messagebody -replace "{{RELEASE_NOTES_LINK}}", $releaseNotesLink
	$messagebody = $messagebody -replace "{{RELEASE_NUMBER}}", $versionNumber
	$messagebody = $messagebody -replace "{{MASTER_SHA}}", $masterSHA.ToString()
	$messagebody = $messagebody -replace "{{DEPLOY_DATE}}", $deployDate
	$messagebody = $messagebody -replace "{{RELEASED_BRANCHES}}", $releasedBranches
	$messagebody = $messagebody -replace "{{ADDITIONAL_INSTRUCTIONS}}", $additionalInstrLink

	Send-EmailFromBuildagent $smtpTo $copyTo $messageSubject $messagebody
}

Export-ModuleMember -Function Send-ReleaseLetter
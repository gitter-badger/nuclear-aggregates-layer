[string[]]$adminEmail = ("d.sidelnikov@2gis.ru")
$supportEmail = ("a.sapotko@2Gis.ru", "m.shemetov@2gis.ru")
$copyTo = ("v.odoev@2Gis.ru", "yu.titova@2Gis.ru", "a.ukraintsev@2gis.ru", "k.ulyanov@2Gis.ru", "yu.belova2@2Gis.ru", "a.churbanov@2Gis.ru", "a.yashina@2Gis.ru", "a.gutorov@2Gis.ru", "m.dik@2Gis.ru", "y.baranihin@2Gis.ru", "d.ivanov@2gis.ru", "a.rechkalov@2Gis.ru", "d.ivanov@2gis.ru", "i.maslennikov@2gis.ru", "a.tukaev@2Gis.ru", "m.pashuk@2gis.ru", "a.pashkin@2Gis.ru", "s.pomadin@2Gis.ru", "a.ukraintsev@2gis.ru", "o.shevchenko@2gis.ru", "a.chupina@2Gis.ru")
$hotfixLetterTemplateName = "Hotfix_Letter_template.htm"
$hotfixLetterWithAddTemplateName = "Hotfix_Letter_With_AddInfo_template.htm"
$releaseLetterTemplateName = "Release_Letter_template.htm"
$releaseLetterWithAddTemplateName = "Release_Letter_With_AddInfo_template.htm"

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

	$letterTemplatePath = Join-Path $PSScriptRoot $letterTemplateName

	[string]$messagebody = Get-Content $letterTemplatePath -Encoding UTF8

	$messagebody = $messagebody -replace "{{RELEASE_NAME}}", $releaseFullName
	$messagebody = $messagebody -replace "RELEASE_NOTES_LINK", $releaseNotesLink
	$messagebody = $messagebody -replace "{{RELEASE_NUMBER}}", $versionNumber
	$messagebody = $messagebody -replace "{{MASTER_SHA}}", $masterSHA.ToString()
	$messagebody = $messagebody -replace "{{DEPLOY_DATE}}", $deployDate
	$messagebody = $messagebody -replace "{{RELEASED_BRANCHES}}", $releasedBranches
	$messagebody = $messagebody -replace "ADDITIONAL_INSTRUCTIONS", $additionalInstrLink

	Import-Module "$BuildToolsRoot\modules\email.psm1" -DisableNameChecking -Force
	Send-Email $smtpTo $copyTo $messageSubject $messagebody
}
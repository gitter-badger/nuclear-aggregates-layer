Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module .\modules\msdeploy.psm1 -DisableNameChecking

Properties{ $OptionWpf=$false }
Task Build-Wpf -Precondition { return $OptionWpf }{
	$iisAppPath	= 'erm06.test.erm.russia'
	$clickOnceUri = New-Object System.Uri("http://$iisAppPath")
	
	# WPF там пока сильно не понятно
}
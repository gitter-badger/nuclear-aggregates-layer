Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

function PSUsing ([System.IDisposable] $Disposable, [ScriptBlock] $ScriptBlock) {
    try {
        & $ScriptBlock
    }
	finally {
        if ($Disposable -ne $null) {
			$Disposable.Dispose()
        }
    }
}

Export-ModuleMember -Function PSUsing
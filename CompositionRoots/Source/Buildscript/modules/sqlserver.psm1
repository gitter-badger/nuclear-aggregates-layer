Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

$SqlSdkVersion = '120'

$SqlSdkDir = "${Env:ProgramFiles}\Microsoft SQL Server\$SqlSdkVersion\SDK\Assemblies"
Add-Type -Path (Join-Path $SqlSdkDir 'Microsoft.SqlServer.ConnectionInfo.dll')
Add-Type -Path (Join-Path $SqlSdkDir 'Microsoft.SqlServer.Smo.dll')

function Replace-StoredProcs ($SourceConnectionString, $Regex, $Replace){
	
	$builder = Get-ConnectionStringBuilder $SourceConnectionString
	
	$sqlConnection = New-Object System.Data.SqlClient.SqlConnection($builder.ConnectionString)
	try{
		$serverConnection = New-Object Microsoft.SqlServer.Management.Common.ServerConnection($sqlConnection)
		$server = New-Object Microsoft.SqlServer.Management.Smo.Server($serverConnection)
		$database = $server.Databases[$builder['Initial Catalog']];
		$storedProcs = $database.StoredProcedures + $database.UserDefinedFunctions | where { $_.Schema -ne 'sys' -and $_.TextMode }
		
		foreach($storedProc in $storedProcs){
			
			$newTextBody = $storedProc.TextBody -replace $Regex, $Replace
	        if ($newTextBody -eq $storedProc.TextBody){
		        continue
	        }

	        $storedProc.TextBody = $newTextBody
	        $storedProc.Alter()

			Write-Host "Update '$($storedProc.Name)'"
		}
	}
	finally{
		$sqlConnection.Dispose()
	}
}

function Get-ConnectionStringBuilder($ConnectionString) {
	$dbConnectionStringBuilder = New-Object System.Data.Common.DbConnectionStringBuilder
	$dbConnectionStringBuilder.set_ConnectionString($ConnectionString)
	
	return $dbConnectionStringBuilder
}

Export-ModuleMember -Function Replace-StoredProcs
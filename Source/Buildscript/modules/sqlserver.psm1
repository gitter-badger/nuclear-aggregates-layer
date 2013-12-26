Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

function Replace-StoredProcs ($SourceConnectionString, $Regex, $Replace){
	# only SQL Server 2012 and later
	Push-Location
	Import-Module 'sqlps' -DisableNameChecking	
	Pop-Location
	
	$builder = Get-ConnectionStringBuilder $SourceConnectionString
	$storedProcs = Get-StoredProcs $builder

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

function Get-StoredProcs ($ConnectionStringBuilder){
	$sqlConnection = New-Object System.Data.SqlClient.SqlConnection($ConnectionStringBuilder.ConnectionString)
	$serverConnection = New-Object Microsoft.SqlServer.Management.Common.ServerConnection($sqlConnection)
	$server = New-Object Microsoft.SqlServer.Management.Smo.Server($serverConnection)
	$database = $server.Databases[$ConnectionStringBuilder['Initial Catalog']];
	
	return $database.StoredProcedures | where { $_.Schema -ne 'sys' }
}

function Get-ConnectionStringBuilder($ConnectionString) {
	$dbConnectionStringBuilder = New-Object System.Data.Common.DbConnectionStringBuilder
	$dbConnectionStringBuilder.set_ConnectionString($ConnectionString)
	
	return $dbConnectionStringBuilder
}

Export-ModuleMember -Function Replace-StoredProcs
Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
#------------------------------

function Replace-StoredProcs ($ConfigFileName, $SourceConnectionStringName, $TargetConnectionStringName){
	# only SQL Server 2012 and later
	Push-Location
	Import-Module 'sqlps' -DisableNameChecking	
	Pop-Location
	
	$targetDatabaseName = Get-DatabaseName $ConfigFileName $TargetConnectionStringName
	$sourceBuilder = Get-ConnectionStringBuilder $ConfigFileName $SourceConnectionStringName
	$storedProcs = Get-StoredProcs $sourceBuilder

	$symbolPart = $targetDatabaseName -replace '\d'
	$regex = "$symbolPart[0-9]*"

	foreach($storedProc in $storedProcs){
		
		$newTextBody = $storedProc.TextBody -replace $regex, $targetDatabaseName
        if ($newTextBody -eq $storedProc.TextBody){
	        continue
        }

        $storedProc.TextBody = $newTextBody
        $storedProc.Alter()

		Write-Host "Update '$($storedProc.Name)'"
	}
}

# hack соглашение для dynamics
function Get-DatabaseName($ConfigFileName, $ConnectionStringName){
	$builder = Get-ConnectionStringBuilder $ConfigFileName $ConnectionStringName

	if ($ConnectionStringName -eq 'CrmConnection'){
		$uri = $builder['Server']
		$uriBuilder = New-Object System.UriBuilder($uri)
		# не добавляем суффикс _MSCRM, он не нужен
		$databaseName = $uriBuilder.Path.Trim('/')
	}
	else{
		$databaseName = $builder['Initial Catalog']
	}

	return $databaseName
}

function Get-StoredProcs ($ConnectionStringBuilder){
	$sqlConnection = New-Object System.Data.SqlClient.SqlConnection($ConnectionStringBuilder.ConnectionString)
	$serverConnection = New-Object Microsoft.SqlServer.Management.Common.ServerConnection($sqlConnection)
	$server = New-Object Microsoft.SqlServer.Management.Smo.Server($serverConnection)
	$database = $server.Databases[$ConnectionStringBuilder['Initial Catalog']];
	
	return $database.StoredProcedures | where { $_.Schema -ne 'sys' }
}

function Get-ConnectionStringBuilder($configFileName, $connectionStringName) {
	$connectionString = Get-ConnectionString $configFileName $connectionStringName
	
	$dbConnectionStringBuilder = New-Object System.Data.Common.DbConnectionStringBuilder
	$dbConnectionStringBuilder.set_ConnectionString($connectionString)
	
	return $dbConnectionStringBuilder
}

function Get-ConnectionString($configFileName, $connectionStringName) {
	[xml]$config = Get-Content $configFileName -Encoding UTF8	
	
	$xmlNode = $config.SelectNodes("configuration/connectionStrings/add[@name = '$connectionStringName']")
	if ($xmlNode -eq $null){
		throw "Could not find connection string $connectionStringName in file '$configFileName'"
	}

	return $xmlNode.connectionString
}

Export-ModuleMember -Function Get-ConnectionString, Replace-StoredProcs
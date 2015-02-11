Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

function Get-EntryPointMetadata($EntryPoint){

	$environmentMetadatas = $global:Context.EnvironmentMetadata
	$environmentName = $global:Context.EnvironmentName

	if (!$environmentMetadatas.ContainsKey($environmentName)){
		throw "Can't find metadata for environment '$environmentName'!"
	}
	$environmentMetadata = $environmentMetadatas[$environmentName]
	
	if (!$environmentMetadata.ContainsKey($EntryPoint)){
		throw "Can't find metadata for entry point '$EntryPoint' in environment '$environmentName'!"
	}
	$entryPointMetadata = $environmentMetadata[$EntryPoint]
	
	return $entryPointMetadata
}

Export-ModuleMember -Function Get-EntryPointMetadata
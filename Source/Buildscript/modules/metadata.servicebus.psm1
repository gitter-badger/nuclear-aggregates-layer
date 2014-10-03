Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

# TODO: Наполнить метаданные
$SharedAccessKeys = @{
	'Test.04' = '6k1N+enGbXNLaSuFCNe2+4OrVB8+QccRZQMhpgOwVZ8='

}

function Get-SharedAccessKeyMetadata($EnvName){

	if (!$SharedAccessKeys.ContainsKey($EnvName)){
		return @{}
	}

	return @{ '{SharedAccessKey}' = $SharedAccessKeys[$EnvName] }
}

Export-ModuleMember -Function Get-SharedAccessKeyMetadata
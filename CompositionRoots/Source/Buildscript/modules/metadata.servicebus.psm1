Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

# TODO: Наполнить метаданные
$SharedAccessKeys = @{
	'Production.Russia' = 'e97SXT9BYyBHpANc+ZjiOjp97EHqcaFii/BK77Ntijo='
	'Load.Russia' = 'daOQn6EYUWbJPlfV1592+xCIQRTumxZzzEw2c1i1u0M='
	'Test.01' = 'VMjlhLcjp4K3+Ur6c2c8vEbEsRZVbFXHuiv9MRW7Y34='
	'Test.02' = 'UQcwkyrf7YrfjVVmq0Y3J7fIQIpfc4UuUHcORMeg+vA='
	'Test.03' = 'TmElHfY7fGenC9thyWL+/P/lDrcthzzKSoF2lB8LSBs='
	'Test.04' = 'qXjGMK2DyIySJtVL0SjQM3RcNYnN365gHtmpkb9IWKs='
	'Test.05' =	'fJrj72O4D4JJ4dE06xDCp1eFFRDkYbqbHL1aIYAEelE='
	'Test.06' =	'JB2JhAMShwaCCbhOxohqnNy/HpxSQUT2SWpDeUSgOM4='
	'Test.08' =	'P+wgbmmPDO+3G3OQi/cT3e0Ym6TgMpV4BpN8zkNW9WI='
}

function Get-SharedAccessKeyMetadata($EnvName){

	if (!$SharedAccessKeys.ContainsKey($EnvName)){
		return @{}
	}

	return @{ '{SharedAccessKey}' = $SharedAccessKeys[$EnvName] }
}

Export-ModuleMember -Function Get-SharedAccessKeyMetadata
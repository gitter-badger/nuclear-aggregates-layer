Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

# TODO: Наполнить метаданные
$SharedAccessKeys = @{
	'Production.Russia' = 'ekQp9UcnukDWwNK9ZCiHXU91ovxdXs/XMZ05fAsam+s='
	'Load.Russia' = 'daOQn6EYUWbJPlfV1592+xCIQRTumxZzzEw2c1i1u0M='
	'Test.01' = 'dxorbZPNw1/uiwnp86NfFOgRaJqe3IMzFfMLTG3omM4='
	'Test.02' = '2t4kdp1F87nw01Gkjgy7s7gzblYCHgt6lzn/emeNNg8='
	'Test.03' = '68Av4IZXBAKVu2fpUntiGx8Jliz7V9N4T9aYRRW/olM='
	'Test.04' = '+HRJRuoNqYB+UI+b36zQOFqckofYKwbhHjDLRL+zi0c='
	'Test.05' =	'0XGFXdeOOgu2ANkrw+2cZ1AabmEIhDvNbxvNd0m5kIQ='
	'Test.06' =	'Vx7wWYOw6QEZl579WRC/NF+x1xiZ23rzpF1SIWbc2BY='
	'Test.08' =	'uXqO99MHiKRDNHQbpnjgOpwVfx135vq+GnoDYdzmAPc='
}

function Get-SharedAccessKeyMetadata($EnvName){

	if (!$SharedAccessKeys.ContainsKey($EnvName)){
		return @{}
	}

	return @{ '{SharedAccessKey}' = $SharedAccessKeys[$EnvName] }
}

Export-ModuleMember -Function Get-SharedAccessKeyMetadata
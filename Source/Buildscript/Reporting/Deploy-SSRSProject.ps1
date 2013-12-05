#requires -version 2.0
[CmdletBinding()]
param (
    [parameter(Mandatory=$true)]
    [ValidatePattern('\.rptproj$')]
    [ValidateScript({ Test-Path -PathType Leaf -Path $_ })]
    [string]
    $Path,
    
    [parameter(
        ParameterSetName='Configuration',
        Mandatory=$true)] 
    [string]
    $Configuration,
    
    [parameter(
        ParameterSetName='Target',
        Mandatory=$true)]
    [ValidatePattern('^https?://')]
    [string]
    $ServerUrl,
    
    [parameter(
        ParameterSetName='Target',
        Mandatory=$true)]
    [string]
    $Folder,

    [parameter(
        ParameterSetName='Target',
        Mandatory=$true)]
    [string]
    $DataSourceFolder,
    
    [parameter(ParameterSetName='Target')]
    [switch]
    $OverwriteDataSources,

    [System.Management.Automation.PSCredential]
    $Credential
)

function New-XmlNamespaceManager ($XmlDocument, $DefaultNamespacePrefix) {
    $NsMgr = New-Object -TypeName System.Xml.XmlNamespaceManager -ArgumentList $XmlDocument.NameTable
    $DefaultNamespace = $XmlDocument.DocumentElement.GetAttribute('xmlns')
    if ($DefaultNamespace -and $DefaultNamespacePrefix) {
        $NsMgr.AddNamespace($DefaultNamespacePrefix, $DefaultNamespace)
    }
    return ,$NsMgr # unary comma wraps $NsMgr so it isn't unrolled
}

function Normalize-SSRSFolder (
    [string]$Folder
) {
    if (-not $Folder.StartsWith('/')) {
        $Folder = '/' + $Folder
    }
    
    return $Folder
}

function New-SSRSFolder (
    $Proxy,
    [string]
    $Name
) {
    Write-Verbose "New-SSRSFolder -Name $Name"

    $Name = Normalize-SSRSFolder -Folder $Name

    if ($Proxy.GetItemType($Name) -ne 'Folder') {
        $Parts = $Name -split '/'
        $Leaf = $Parts[-1]
        $Parent = $Parts[0..($Parts.Length-2)] -join '/'

        if ($Parent) {
            New-SSRSFolder -Proxy $Proxy -Name $Parent
        } else {
            $Parent = '/'
        }
        
        $Proxy.CreateFolder($Leaf, $Parent, $null) | Out-Null
    }
}

function New-SSRSDataSource (
    $Proxy,
    [string]$RdsPath,
    [string]$Folder
) {
    Write-Verbose "New-SSRSDataSource -RdsPath $RdsPath -Folder $Folder"

    $Folder = Normalize-SSRSFolder -Folder $Folder

    [xml]$Rds = Get-Content -Encoding UTF8 -Path $RdsPath
    $ConnProps = $Rds.RptDataSource.ConnectionProperties
    
    $Definition = New-Object -TypeName SSRS.ReportingService2010.DataSourceDefinition
    $Definition.ConnectString = $ConnProps.ConnectString
    $Definition.Extension = $ConnProps.Extension
	
	# ERM HACK - ������ integrated security
	$Definition.CredentialRetrieval = 'Integrated'
	
    $DataSource = New-Object -TypeName PSObject -Property @{
        Name = $Rds.RptDataSource.Name
        Path =  $Folder + '/' + $Rds.RptDataSource.Name
    }
    
	$Proxy.CreateDataSource($DataSource.Name, $Folder, $true, $Definition, $null) | Out-Null
    return $DataSource
}

function New-SSRSDataset (
    $Proxy,
    [string]$RsdPath,
    [string]$Folder
) {
	Write-Verbose "New-SSRSDataset -RsdPath $RsdPath -Folder $Folder"
	$Folder = Normalize-SSRSFolder -Folder $Folder
	
    $Dataset = New-Object -TypeName PSObject -Property @{
        Name = [System.IO.Path]::GetFileNameWithoutExtension($RsdPath)
        Path =  $Folder + '/' + [System.IO.Path]::GetFileNameWithoutExtension($RsdPath)
    }
	
	$RsdBytes = Get-Content -Encoding Byte -Path $RsdPath
	$warnings = @()
	$Proxy.CreateCatalogItem("DataSet", $Dataset.Name, $Folder, $true, $RsdBytes, $null, [ref]$warnings) | Out-Null
	
	return $Dataset
}

$script:ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$PSScriptRoot = $MyInvocation.MyCommand.Path | Split-Path

$Path = $Path | Convert-Path
$ProjectRoot = $Path | Split-Path
[xml]$Project = Get-Content -Path $Path -Encoding UTF8

if ($PSCmdlet.ParameterSetName -eq 'Configuration') {
    $Config = & $PSScriptRoot\Get-SSRSProjectConfiguration.ps1 -Path $Path -Configuration $Configuration
    $ServerUrl = $Config.ServerUrl
    $Folder = $Config.Folder
    $DataSourceFolder = $Config.DataSourceFolder
    $OverwriteDataSources = $Config.OverwriteDataSources
    $DatasetFolder = $Config.DatasetFolder
    $OverwriteDatasets = $Config.OverwriteDatasets
}

$Folder = Normalize-SSRSFolder -Folder $Folder
$DataSourceFolder = Normalize-SSRSFolder -Folder $DataSourceFolder
$DatasetFolder = Normalize-SSRSFolder -Folder $DatasetFolder

$Proxy = & $PSScriptRoot\New-SSRSWebServiceProxy.ps1 -Uri $ServerUrl -Credential $Credential

New-SSRSFolder -Proxy $Proxy -Name $Folder
New-SSRSFolder -Proxy $Proxy -Name $DataSourceFolder
New-SSRSFolder -Proxy $Proxy -Name $DatasetFolder

$DataSourcePaths = @{}
$Project.SelectNodes('Project/DataSources/ProjectItem') |
    ForEach-Object {
        $RdsPath = $ProjectRoot | Join-Path -ChildPath $_.FullPath
        $DataSource = New-SSRSDataSource -Proxy $Proxy -RdsPath $RdsPath -Folder $DataSourceFolder
        $DataSourcePaths.Add($DataSource.Name, $DataSource.Path)
    }
$DatasetPaths = @{}
$Project.SelectNodes('Project/DataSets/ProjectItem') |
    ForEach-Object {
        $RsdPath = $ProjectRoot | Join-Path -ChildPath $_.FullPath
        $Dataset = New-SSRSDataset -Proxy $Proxy -RsdPath $RsdPath -Folder $DatasetFolder
        $DatasetPaths.Add($Dataset.Name, $Dataset.Path)
		
		# set data source
		[xml]$Rsd = Get-Content -Encoding UTF8 -Path $RsdPath
		$DataSourceName = $Rsd.SharedDataSet.DataSet.Query.DataSourceReference
		$DataSourcePath = $DataSourcePaths[$DataSourceName]
        if (-not $DataSourcePath) {
            throw "Invalid data source reference '$DataSourceName' in $RsdPath"
        }

		$Reference = New-Object -TypeName SSRS.ReportingService2010.DataSourceReference
		$Reference.Reference = $DataSourcePath
        $DataSource = New-Object -TypeName SSRS.ReportingService2010.DataSource
        $DataSource.Item = $Reference
        $DataSource.Name = "DataSetDataSource"

		$Proxy.SetItemDataSources($DatasetFolder + '/' + $Dataset.Name, [array]$DataSource) | Out-Null
    }

$Project.SelectNodes('Project/Reports/ProjectItem') |
    ForEach-Object {
        $RdlPath = $ProjectRoot | Join-Path -ChildPath $_.FullPath
        [xml]$Definition = Get-Content -Encoding UTF8 -Path $RdlPath
        $NsMgr = New-XmlNamespaceManager $Definition d

        $RawDefinition = Get-Content -Encoding Byte -Path $RdlPath
        
        $Name = $_.Name -replace '\.rdl$',''
        
        Write-Verbose "Creating report $Name"
		$warnings = @()
        $Proxy.CreateCatalogItem("Report", $Name, $Folder, $true, $RawDefinition, $null, [ref]$warnings) | Out-Null

		# set report data sources
        $DataSourceXpath = 'd:Report/d:DataSources/d:DataSource/d:DataSourceReference/..'
        $DataSources = $Definition.SelectNodes($DataSourceXpath, $NsMgr) |
            ForEach-Object {
                $DataSourcePath = $DataSourcePaths[$_.DataSourceReference]
                if (-not $DataSourcePath) {
                    throw "Invalid data source reference '$($_.DataSourceReference)' in $RdlPath"
                }
                $Reference = New-Object -TypeName SSRS.ReportingService2010.DataSourceReference
                $Reference.Reference = $DataSourcePath
                $DataSource = New-Object -TypeName SSRS.ReportingService2010.DataSource
                $DataSource.Item = $Reference
                $DataSource.Name = $_.Name
                $DataSource
            }
        if ($DataSources) {
            $Proxy.SetItemDataSources($Folder + '/' + $Name, $DataSources) | Out-Null
        }
		
		# set report data sets
		$DatasetXpath = 'd:Report/d:DataSets/d:DataSet/d:SharedDataSet/d:SharedDataSetReference/../..'
		$Datasets = $Definition.SelectNodes($DatasetXpath, $NsMgr) |
			ForEach-Object {
				$DatasetPath = $DatasetPaths[$_.SharedDataSet.SharedDataSetReference]
                if (-not $DatasetPath) {
                    throw "Invalid data set reference '$($_.SharedDataSet.SharedDataSetReference)' in $RdlPath"
                }
				
                $Reference = New-Object -TypeName SSRS.ReportingService2010.ItemReference
                $Reference.Reference = $DatasetPath
                $Reference.Name = $_.Name
                $Reference
			}
        if ($Datasets) {
            $Proxy.SetItemReferences($Folder + '/' + $Name, $Datasets) | Out-Null
        }
    }

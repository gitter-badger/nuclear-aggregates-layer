DROP TYPE [Integration].[ContactDTOsTableType]
GO

CREATE TYPE [Integration].[ContactDTOsTableType] AS TABLE(
	[InsertOrder] [int] NOT NULL,
	[CardCode] [bigint] NOT NULL,
	[ContactName] [nvarchar](250) NOT NULL,
	[ContactTypeId] [int] NOT NULL,
	[Contact] [nvarchar](max) NULL,
	[ZoneCode] [nvarchar](max) NULL,
	[FormatCode] [nvarchar](max) NULL,
	[CanReceiveFax] [bit] NULL,
	[ZoneName] [nvarchar](max) NULL,
	[FormatName] [nvarchar](max) NULL,
	[IsContactInactive] [bit] NULL,
	[SortingPosition] [int] NULL,
	[AddressId] [bigint] NULL
)
GO



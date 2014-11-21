DROP TYPE [Integration].[CardsTableType]
GO

CREATE TYPE [Integration].[CardsTableType] AS TABLE(
	[CardCode] [bigint] NOT NULL,
	[FirmCode] [bigint] NOT NULL,
	[BranchCode] [int] NOT NULL,
	[CardType] [nvarchar](3) NOT NULL,
	[BuildingCode] [bigint] NULL,
	[Address] [nvarchar](max) NULL,
	[PromotionalReferencePoint] [nvarchar](max) NULL,
	[ReferencePoint] [nvarchar](max) NULL,
	[IsCardHiddenOrArchived] [bit] NULL,
	[WorkingTime] [nvarchar](max) NULL,
	[PaymentMethods] [nvarchar](max) NULL,
	[IsHidden] [bit] NULL,
	[IsArchived] [bit] NULL,
	[IsDeleted] [bit] NULL,
	[FirmId] [bigint] NULL,
	[AddressId] [bigint] NULL,
	[OrganizationUnitId] [bigint] NULL,
	[IsLocal] [bit] NOT NULL
)
GO



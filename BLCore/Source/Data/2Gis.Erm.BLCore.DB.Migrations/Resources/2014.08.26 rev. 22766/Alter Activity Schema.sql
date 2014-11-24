IF object_id('[Activity].[LetterReferences]') IS NOT NULL DROP TABLE [Activity].[LetterReferences];
IF object_id('[Activity].[LetterBase]') IS NOT NULL DROP TABLE [Activity].[LetterBase];

CREATE TABLE [Activity].[LetterBase](
	[Id] [bigint] NOT NULL CONSTRAINT [PK_Letters] PRIMARY KEY CLUSTERED,
	[ReplicationCode] [uniqueidentifier] NOT NULL,

	[OwnerCode] [bigint] NOT NULL,
	[Subject] [nvarchar](256) NULL,
	[Description] [nvarchar](max) NULL,
	[ScheduledStart] [datetime2] NOT NULL,
	[ScheduledEnd] [datetime2] NOT NULL,
	[ActualEnd] [datetime2] NULL,
	[Priority] [int] NOT NULL CONSTRAINT [DF_Letter_Priority]  DEFAULT 0,
	[Status] [int] NOT NULL CONSTRAINT [DF_Letter_Status]  DEFAULT 0,

	[IsActive] [bit] NOT NULL CONSTRAINT [DF_Letter_IsActive]  DEFAULT 1,
	[IsDeleted] [bit] NOT NULL CONSTRAINT [DF_Letter_IsDeleted]  DEFAULT 0,
	[CreatedBy] [bigint] NOT NULL,
	[CreatedOn] [datetime2] NOT NULL,
	[ModifiedBy] [bigint] NULL,
	[ModifiedOn] [datetime2] NULL,
	[Timestamp] [timestamp] NOT NULL
)

-- holds regarding object references
CREATE TABLE [Activity].[LetterReferences](
	[LetterId] [bigint] NOT NULL CONSTRAINT [FK_Letters] REFERENCES [Activity].[LetterBase]([Id]),
	[Reference] [int] NOT NULL,
	[ReferencedType] [int] NOT NULL,
	[ReferencedObjectId] [bigint] NOT NULL
	CONSTRAINT [PK_LetterReferences] PRIMARY KEY CLUSTERED ([LetterId], [Reference], [ReferencedType], [ReferencedObjectId])
)

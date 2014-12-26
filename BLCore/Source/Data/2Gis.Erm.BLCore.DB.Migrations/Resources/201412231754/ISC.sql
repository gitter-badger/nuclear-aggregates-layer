SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE SCHEMA [Metadata]

CREATE TABLE [Metadata].[IdentityServiceIds](
	[IdentityServiceUniqueId] [tinyint] NOT NULL,
	[InstallationId] [int] NOT NULL,
	[ServiceInstanceId] [uniqueidentifier] NULL,
 CONSTRAINT [PK_IdentityServiceIds] PRIMARY KEY CLUSTERED 
(
	[IdentityServiceUniqueId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


CREATE TABLE [Metadata].[ServiceInstances](
	[Id] [uniqueidentifier] NOT NULL,
	[Environment] [nvarchar](100) NOT NULL,
	[EntryPoint] [nvarchar](100) NOT NULL,
	[ServiceName] [nvarchar](100) NOT NULL,
	[Host] [nvarchar](100) NOT NULL,
	[FirstCheckinTime] [datetimeoffset](3) NOT NULL,
	[LastCheckinTime] [datetimeoffset](3) NOT NULL,
	[CheckinIntervalMs] [int] NOT NULL,
	[TimeSafetyOffsetMs] [int] NOT NULL,
	[IsRunning] [bit] NOT NULL,
	[IsSelfReport] [bit] NOT NULL,
 CONSTRAINT [PK_Metadata.ServiceInstances] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

;WITH N1(C) AS (SELECT 0 UNION ALL SELECT 0) -- 2 rows
,N2(C) AS (SELECT 0 FROM N1 AS T1 CROSS JOIN N1 AS T2) -- 4 rows
,N3(C) AS (SELECT 0 FROM N2 AS T1 CROSS JOIN N2 AS T2) -- 16 rows
,N4(C) AS (SELECT 0 FROM N3 AS T1 CROSS JOIN N3 AS T2) -- 256 rows
,Ids(Id) AS (SELECT ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) -1 FROM N4) 


INSERT INTO [Metadata].[IdentityServiceIds] ([IdentityServiceUniqueId], [InstallationId], [ServiceInstanceId])
SELECT [Id], [Id] / 8 + 1, NULL FROM Ids OPTION (MAXRECURSION 0)

GO
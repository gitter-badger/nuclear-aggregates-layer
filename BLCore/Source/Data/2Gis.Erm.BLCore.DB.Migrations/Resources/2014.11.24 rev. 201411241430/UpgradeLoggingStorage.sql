USE [{0}]
GO

IF NOT EXISTS (
SELECT  schema_name
FROM    information_schema.schemata
WHERE   schema_name = N'Log' ) 
BEGIN
EXEC sp_executesql N'CREATE SCHEMA Log'
END
ELSE 
BEGIN 
	print 'Target schema for log events is already exists, so do nothing'
END

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = N'Log' 
                 AND  TABLE_NAME = N'Events'))
BEGIN
    EXEC sp_executesql N'CREATE TABLE [Log].[Events](
						[Id] [bigint] IDENTITY(1,1) NOT NULL,
						[Date] [datetime2](7) NOT NULL,
						[Level] [char](5) NOT NULL,
						[Message] [nvarchar](max) NULL,
						[ExceptionData] [nvarchar](max) NULL,
						[Environment] [nvarchar](100) NOT NULL,
						[EntryPoint] [nvarchar](100) NOT NULL,
						[EntryPointHost] [nvarchar](250) NOT NULL,
						[EntryPointInstanceId] [uniqueidentifier] NOT NULL,
						[UserAccount] [nvarchar](100) NOT NULL,
						[UserSession] [nvarchar](100) NULL,
						[UserAddress] [nvarchar](100) NULL,
						[UserAgent] [nvarchar](100) NULL,
					 CONSTRAINT [PK_Events] PRIMARY KEY CLUSTERED 
					(
						[Id] ASC
					)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
					) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]'
END
ELSE 
BEGIN 
	print 'Target table for log events is already exists, so do nothing'
END
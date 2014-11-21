CREATE TABLE [dbo].[LogJournal] (
    [Id]              BIGINT          IDENTITY (1, 1) NOT NULL,
    [UserDataID]      INT             NOT NULL,
    [MessageDate]     DATETIME2 (7)   NOT NULL,
    [MessageLevel]    CHAR (5)        NOT NULL,
    [MessageText]     NVARCHAR (MAX)  NULL,
    [ExceptionData]   NVARCHAR (MAX)  NULL,
    [MethodName]      NVARCHAR (250)  NULL,
    [InputParameters] NVARCHAR (1024) NULL,
    CONSTRAINT [PK_LogJournal] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UserData_LogJournal] FOREIGN KEY ([UserDataID]) REFERENCES [dbo].[UserData] ([Id])
);


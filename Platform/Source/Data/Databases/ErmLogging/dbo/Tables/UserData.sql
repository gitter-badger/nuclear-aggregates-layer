CREATE TABLE [dbo].[UserData] (
    [Id]          INT              IDENTITY (1, 1) NOT NULL,
    [SeanceID]    UNIQUEIDENTIFIER NULL,
    [SessionID]   NVARCHAR (50)    NULL,
    [UserName]    NVARCHAR (100)   NULL,
    [UserIP]      NVARCHAR (50)    NULL,
    [UserBrowser] NVARCHAR (100)   NULL,
    [HashCode]    NVARCHAR (50)    NULL,
    CONSTRAINT [PK_UserInstance] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UserData_SeanceData] FOREIGN KEY ([SeanceID]) REFERENCES [dbo].[Seances] ([Id])
);


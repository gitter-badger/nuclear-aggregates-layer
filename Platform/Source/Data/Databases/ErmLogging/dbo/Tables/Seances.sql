CREATE TABLE [dbo].[Seances] (
    [Id]          UNIQUEIDENTIFIER NOT NULL,
    [ModuleId]    TINYINT          NOT NULL,
    [MessageDate] DATETIME2 (7)    NOT NULL,
    [ConfigFile]  NVARCHAR (MAX)   NULL,
    CONSTRAINT [PK_SeanceConfig] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_SeanceData_Modules] FOREIGN KEY ([ModuleId]) REFERENCES [dbo].[Modules] ([Id])
);


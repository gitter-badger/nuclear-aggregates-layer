SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[SystemUserErmView]
WITH SCHEMABINDING
AS
SELECT LEFT(STUFF([DomainName], 1, CHARINDEX('\', [DomainName]), ''), 50) AS [ErmUserAccount],
       [SystemUserId],
	   [BusinessUnitId],
	   [OrganizationId]
FROM  [dbo].[SystemUserBase]	 
GO

CREATE UNIQUE CLUSTERED INDEX [IX_SystemUserErmView_SystemUserId] ON [dbo].[SystemUserErmView] ([SystemUserId])
CREATE NONCLUSTERED INDEX [IX_SystemUserErmView_ErmUserAccount] ON [dbo].[SystemUserErmView] ([ErmUserAccount]) INCLUDE ([SystemUserId], [BusinessUnitId])
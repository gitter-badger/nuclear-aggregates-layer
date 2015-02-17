
CREATE View [Log].[Events_LocalTimeZone] as
SELECT [Id]
      ,DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), [Date]) as DateLocalTimeZone
      ,[Level]
      ,[Message]
      ,[ExceptionData]
      ,[Environment]
      ,[EntryPoint]
      ,[EntryPointHost]
      ,[EntryPointInstanceId]
      ,[UserAccount]
      ,[UserSession]
      ,[UserAddress]
      ,[UserAgent]
FROM [Log].[Events]
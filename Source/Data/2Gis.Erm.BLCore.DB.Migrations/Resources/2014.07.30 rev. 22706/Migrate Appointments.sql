DELETE FROM [Activity].[AppointmentReferences]
DELETE FROM [Activity].[AppointmentBase]
GO

INSERT INTO [Activity].[AppointmentBase]
	([Id],[ReplicationCode],[CreatedBy],[CreatedOn],[ModifiedBy],[ModifiedOn],[IsActive],[IsDeleted],[OwnerCode]
	,[Subject],[Description],[ScheduledStart],[ScheduledEnd],[ActualEnd],[Priority],[Status],[IsAllDayEvent],[Location],[Purpose]
	)
SELECT [Id], NEWID(),[CreatedBy],[CreatedOn],[ModifiedBy],[ModifiedOn],[IsActive],[IsDeleted],[OwnerCode]
	, [Header], [Description], [ScheduledStart], [ScheduledEnd], [ActualEnd], [Priority], [Status], 0, NULL, [Purpose]
  FROM [Activity].[ActivityInstances] ai
  LEFT OUTER JOIN (SELECT ActivityId,[TextValue] AS [Header] FROM [Activity].[ActivityPropertyInstances] WHERE [PropertyId] = 1) p1 ON ai.Id = p1.ActivityId
  LEFT OUTER JOIN (SELECT ActivityId,[TextValue] AS [Description] FROM [Activity].[ActivityPropertyInstances] WHERE [PropertyId] = 8) p2 ON ai.Id = p2.ActivityId
  LEFT OUTER JOIN (SELECT ActivityId,[DateTimeValue] AS ScheduledStart FROM [Activity].[ActivityPropertyInstances] WHERE [PropertyId] = 2) p3 ON ai.Id = p3.ActivityId
  LEFT OUTER JOIN (SELECT ActivityId,[DateTimeValue] AS ScheduledEnd FROM [Activity].[ActivityPropertyInstances] WHERE [PropertyId] = 3) p4 ON ai.Id = p4.ActivityId
  LEFT OUTER JOIN (SELECT ActivityId,[DateTimeValue] AS ActualEnd FROM [Activity].[ActivityPropertyInstances] WHERE [PropertyId] = 11) p5 ON ai.Id = p5.ActivityId
  LEFT OUTER JOIN (SELECT ActivityId,[NumericValue] AS [Priority] FROM [Activity].[ActivityPropertyInstances] WHERE [PropertyId] = 4) p6 ON ai.Id = p6.ActivityId
  LEFT OUTER JOIN (SELECT ActivityId,[NumericValue] AS [Status] FROM [Activity].[ActivityPropertyInstances] WHERE [PropertyId] = 6) p7 ON ai.Id = p7.ActivityId
  LEFT OUTER JOIN (SELECT ActivityId,[NumericValue] AS [Purpose] FROM [Activity].[ActivityPropertyInstances] WHERE [PropertyId] = 5) p8 ON ai.Id = p8.ActivityId
 WHERE [Type] = 2 -- appointment
GO
 
INSERT INTO [Activity].[AppointmentReferences] ([AppointmentId],[Reference],[ReferencedType],[ReferencedObjectId])
SELECT [Id], 1, 200, [ClientId] AS [ObjectID] FROM [Activity].[ActivityInstances] WHERE [Type] = 2 AND ClientId IS NOT NULL
UNION
SELECT [Id], 1, 206, [ContactId] AS [ObjectID]  FROM [Activity].[ActivityInstances] WHERE [Type] = 2 AND ContactId IS NOT NULL
UNION
SELECT [Id], 1, 146, [FirmId] AS [ObjectID]  FROM [Activity].[ActivityInstances] WHERE [Type] = 2 AND FirmId IS NOT NULL
GO
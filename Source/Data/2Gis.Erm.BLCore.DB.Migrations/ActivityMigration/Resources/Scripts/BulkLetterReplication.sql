-- обновляем общую таблицу
INSERT INTO [{0}].[dbo].[ActivityPointerBase]
    (
        [ActivityTypeCode], [ActivityId], 
        [Subject], [Description], [PriorityCode], [StateCode], [StatusCode],
        [ActualStart], [ActualEnd], [ActualDurationMinutes],
        [ScheduledStart], [ScheduledEnd], [ScheduledDurationMinutes],
	    [RegardingObjectTypeCode], [RegardingObjectId], [RegardingObjectIdName],
        [OwningBusinessUnit], [OwningUser],
	    [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn],
        [DeletionStateCode]
    )
SELECT 
    4207 as [ActivityTypeCode]
	, [letters].[ReplicationCode] as [ActivityId]

	, [letters].[Subject]
	, [letters].[Description]
	, CASE [letters].[Priority] WHEN 1 THEN 0 WHEN 2 THEN 1 WHEN 3 THEN 2 ELSE 1 END as [PriorityCode]
    -- (0 - Open, 1 - Completed, 2 - Canceled)
    -- (Open: 1 - Open, 2 - Draft; Completed: 3 - Received, 4 - Sent; Canceled: 5 - Canceled)
	, CASE [letters].[Status] WHEN 1 THEN 0 WHEN 2 THEN 1 WHEN 3 THEN 2 END as [StateCode]
	, CASE [letters].[Status] WHEN 1 THEN 1 WHEN 2 THEN 4 WHEN 3 THEN 5 END as [StatusCode]

	, [letters].[CreatedOn] as [ActualStart]
	, CASE WHEN [letters].[Status] = 2 OR [Status] = 3 THEN [letters].[ModifiedOn] ELSE NULL END as [ActualEnd]
	, CASE WHEN [letters].[Status] = 2 OR [Status] = 3 THEN DATEDIFF(minute, [letters].[CreatedOn], [letters].[ModifiedOn]) ELSE NULL END as [ActualDurationMinutes]

	, [letters].[ScheduledOn] as [ScheduledStart]
	, [letters].[ScheduledOn] as [ScheduledEnd]
	, 0 as [ScheduledDurationMinutes]

	, [refs].[RegardingObjectTypeCode]
	, [refs].[RegardingObjectId]
	, [refs].[RegardingObjectIdName]

	, [crmUsers].[BusinessUnitId] AS [OwningBusinessUnit]
	, [crmUsers].[SystemUserId] AS [OwningUser]

	, [Shared].[GetCrmUserId]([letters].[CreatedBy]) as [CreatedBy]
	, [letters].[CreatedOn]
	, [Shared].[GetCrmUserId]([letters].[ModifiedBy]) as [ModifiedBy]
	, [letters].[ModifiedOn]
    , CASE WHEN [letters].[IsDeleted] = 1 THEN 2 ELSE 0 END as [DeletionStateCode]
FROM [Activity].[LetterBase] [letters]
JOIN [Security].[Users] [users] ON [users].[Id] = [letters].[OwnerCode]
LEFT JOIN [{0}].[dbo].[SystemUserErmView] [crmUsers] WITH ( NOEXPAND ) ON [crmUsers].[ErmUserAccount] = [users].[Account] COLLATE Database_Default
OUTER APPLY (
	SELECT TOP 1
	    CASE [refs].[ReferencedType] 
		    WHEN 200 THEN 1			-- Clients		(ERM: 200, CRM: 1)
		    WHEN 199 THEN 3			-- Deals		(ERM: 199, CRM: 3)
		    WHEN 146 THEN 10013		-- Firms		(ERM: 146, CRM: 10013)
		    END AS [RegardingObjectTypeCode],
		COALESCE([clients].[ReplicationCode], [deals].[ReplicationCode], [firms].[ReplicationCode]) as [RegardingObjectId], 
		COALESCE([clients].[Name], [deals].[Name], [firms].[Name]) as [RegardingObjectIdName]
	FROM (
		SELECT [LetterId], [Reference], [ReferencedType], [ReferencedObjectId]
		FROM [Activity].[LetterReferences]
		WHERE [Reference] = 1 and [ReferencedType] = 199
		UNION ALL
		SELECT [LetterId], [Reference], [ReferencedType], [ReferencedObjectId]
		FROM [Activity].[LetterReferences]
		WHERE [Reference] = 1 and [ReferencedType] = 200
		UNION ALL
		SELECT [LetterId], [Reference], [ReferencedType], [ReferencedObjectId]
		FROM [Activity].[LetterReferences]
		WHERE [Reference] = 1 and [ReferencedType] = 146
	) [refs]
	LEFT JOIN [Billing].[Clients] [clients] on [refs].[ReferencedObjectId] = [clients].[Id] and [ReferencedType] = 200
	LEFT JOIN [Billing].[Deals] [deals] on [refs].[ReferencedObjectId] = [deals].[Id] and [ReferencedType] = 199
	LEFT JOIN [BusinessDirectory].[Firms] [firms] on [refs].[ReferencedObjectId] = [firms].[Id] and [ReferencedType] = 146
	WHERE [refs].[LetterId] = [letters].[Id]
) [refs]

-- обновляем основную таблицу
INSERT INTO [{0}].[dbo].[LetterBase]
    ([ActivityId]) 
SELECT 
    [ReplicationCode] as [ActivityId]
FROM [Activity].[LetterBase]

-- обновляем дополнительную таблицу
INSERT INTO [{0}].[dbo].[LetterExtensionBase] 
    ([ActivityId])
SELECT 
    [ReplicationCode] as [ActivityId]
FROM [Activity].[LetterBase]

-- обновляем связи
INSERT INTO [{0}].[dbo].[ActivityPartyBase]
    ([ActivityPartyId], [ActivityId], [PartyId], [PartyObjectTypeCode], [ParticipationTypeMask], [PartyIdName])
SELECT 
    [letters].[ReplicationCode] as [ActivityId],
	CASE [refs].[Reference]
		WHEN 0 THEN 9			-- Owner			(CRM: 9)
		WHEN 1 THEN 8			-- RegardingObject	(ERM: 1, CRM: 8)
		WHEN 2 THEN 1			-- Sender			(ERM: 2, CRM: 1)
		WHEN 3 THEN 2			-- Recipient		(ERM: 3, CRM: 2)
		END AS [ParticipationTypeMask], 
	CASE refs.ReferencedType 
		WHEN 200 THEN 1			-- Clients		(ERM: 200, CRM: 1)
		WHEN 206 THEN 2			-- Contacts		(ERM: 206, CRM: 2)
		WHEN 199 THEN 3			-- Deals		(ERM: 199, CRM: 3)
		WHEN 146 THEN 10013		-- Firms		(ERM: 146, CRM: 10013)
		WHEN  53 THEN 8			-- Users		(ERM:  53, CRM: 8)
		END AS [PartyObjectTypeCode],
    [refs].[ReferencedObjectId] as [PartyId], 
    [refs].[ReferencedObjectName] as [PartyIdName]
FROM [Activity].[LetterBase] [letters]
CROSS APPLY (
	SELECT TOP 1
        [Reference], 
		[ReferencedType], 
		coalesce([clients].[ReplicationCode],[deals].[ReplicationCode],[firms].[ReplicationCode]) as [ReferencedObjectId], 
		coalesce([clients].[Name],[deals].[Name],[firms].[Name]) as [ReferencedObjectName]
	FROM (
		SELECT [LetterId], [Reference], [ReferencedType], [ReferencedObjectId]
		FROM [Activity].[LetterReferences]
		WHERE [Reference] = 1 and [ReferencedType] = 199
		UNION ALL
		SELECT [LetterId], [Reference], [ReferencedType], [ReferencedObjectId]
		FROM [Activity].[LetterReferences]
		WHERE [Reference] = 1 and [ReferencedType] = 200
		UNION ALL
		SELECT [LetterId], [Reference], [ReferencedType], [ReferencedObjectId]
		FROM [Activity].[LetterReferences]
		WHERE [Reference] = 1 and [ReferencedType] = 146
	) [refs]
	LEFT JOIN [Billing].[Clients] [clients] on [refs].[ReferencedObjectId] = [clients].[Id] and [ReferencedType] = 200
	LEFT JOIN [Billing].[Deals] [deals] on [refs].[ReferencedObjectId] = [deals].[Id] and [ReferencedType] = 199
	LEFT JOIN [BusinessDirectory].[Firms] [firms] on [refs].[ReferencedObjectId] = [firms].[Id] and [ReferencedType] = 146
	WHERE [refs].[LetterId] = [letters].[Id]
                
    UNION ALL
	            
    SELECT 
		[Reference],
		[ReferencedType], 
		coalesce([contacts].[ReplicationCode],Shared.GetCrmUserId([users].[Id])) as [ReferencedObjectId], 
		coalesce([contacts].[FullName],[users].[DisplayName]) as [ReferencedObjectName]
	FROM [Activity].[LetterReferences]
	LEFT JOIN [Billing].[Contacts] contacts on ReferencedObjectId = contacts.Id and ReferencedType = 206
	LEFT JOIN [Security].[Users] users on ReferencedObjectId = users.Id and ReferencedType = 53
	WHERE [Reference] != 1 and [LetterId] = [letters].[Id]
	            
    UNION ALL
	            
    SELECT 
		0 as [Reference], 
		53 as [ReferencedType], 
		Shared.GetCrmUserId(OwnerCode) as [ReferencedObjectId], 
		[users].DisplayName as [ReferencedObjectName]
	FROM [Activity].[LetterBase] [ab]
	LEFT JOIN [Security].[Users] [users] on [ab].[OwnerCode] = [users].Id
	WHERE [ab].[Id] = [letters].[Id]
) [refs]

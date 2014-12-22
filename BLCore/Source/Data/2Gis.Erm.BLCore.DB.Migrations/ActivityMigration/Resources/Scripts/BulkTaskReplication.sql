-- обновляем общую таблицу
INSERT INTO [{0}].[dbo].[ActivityPointerBase]
    (
        [ActivityTypeCode], [ActivityId], 
        [Subject], [Description], [PriorityCode], [StateCode], [StatusCode],
        [ActualStart], [ActualEnd], [ActualDurationMinutes],
        [ScheduledStart], [ScheduledEnd], [ScheduledDurationMinutes],
	    [RegardingObjectTypeCode], [RegardingObjectId], [RegardingObjectIdDsc], [RegardingObjectIdName],
        [OwningBusinessUnit], [OwningUser],
	    [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn],
        [DeletionStateCode]
    )
SELECT 
    4212 as [ActivityTypeCode]
	, [tasks].[ReplicationCode] as [ActivityId]

	, [tasks].[Subject]
	, [tasks].[Description]
	, CASE [tasks].[Priority] WHEN 1 THEN 0 WHEN 2 THEN 1 WHEN 3 THEN 2 ELSE 1 END as [PriorityCode]
	-- (0 - Open, 1 - Completed, 2 - Canceled)
	-- (Open: 2 - Not Started, 3 - In Progress, 4 - Waiting on someone else, 7 - Deferred; Completed: 5 - Completed; Canceled: 6 - Canceled)
	, CASE [tasks].[Status] WHEN 1 THEN 0 WHEN 2 THEN 1 WHEN 3 THEN 2 END as [StateCode]
	, CASE [tasks].[Status] WHEN 1 THEN 3 WHEN 2 THEN 5 WHEN 3 THEN 6 END as [StatusCode]

	, [tasks].[CreatedOn] as [ActualStart]
	, CASE WHEN [tasks].[Status] = 2 OR [Status] = 3 THEN [tasks].[ModifiedOn] ELSE NULL END as [ActualEnd]
	, CASE WHEN [tasks].[Status] = 2 OR [Status] = 3 THEN DATEDIFF(minute, [tasks].[CreatedOn], [tasks].[ModifiedOn]) ELSE NULL END as [ActualDurationMinutes]

	, [tasks].[ScheduledOn] as [ScheduledStart]
	, [tasks].[ScheduledOn] as [ScheduledEnd]
	, 0 as [ScheduledDurationMinutes]

	, [refs].[RegardingObjectTypeCode]
	, [refs].[RegardingObjectId]
    , CASE WHEN [refs].[RegardingObjectId] IS NOT NULL THEN 0 END as [RegardingObjectIdDsc]
	, [refs].[RegardingObjectIdName]

	, [crmOwners].[BusinessUnitId] AS [OwningBusinessUnit]
	, [crmOwners].[SystemUserId] AS [OwningUser]

	, [crmCreators].[SystemUserId] as [CreatedBy]
	, [tasks].[CreatedOn]
	, [crmModifiers].[SystemUserId] as [ModifiedBy]
	, [tasks].[ModifiedOn]
    , CASE WHEN [tasks].[IsDeleted] = 1 THEN 2 ELSE 0 END as [DeletionStateCode]
FROM [Activity].[TaskBase] [tasks]
JOIN [Security].[Users] [owners] ON [owners].[Id] = [tasks].[OwnerCode]
LEFT JOIN [{0}].[dbo].[SystemUserErmView] [crmOwners] WITH ( NOEXPAND ) ON [crmOwners].[ErmUserAccount] = [owners].[Account] COLLATE Database_Default
JOIN [Security].[Users] [creators] ON [creators].[Id] = [tasks].[CreatedBy]
LEFT JOIN [{0}].[dbo].[SystemUserErmView] [crmCreators] WITH ( NOEXPAND ) ON [crmCreators].[ErmUserAccount] = [creators].[Account] COLLATE Database_Default
JOIN [Security].[Users] [modifiers] ON [modifiers].[Id] = [tasks].[ModifiedBy]
LEFT JOIN [{0}].[dbo].[SystemUserErmView] [crmModifiers] WITH ( NOEXPAND ) ON [crmModifiers].[ErmUserAccount] = [modifiers].[Account] COLLATE Database_Default
OUTER APPLY (
	SELECT TOP 1
	    CASE [refs].[ReferencedType] 
		    WHEN 200 THEN 1			-- Clients		(ERM: 200, CRM: 1)
		    WHEN 146 THEN 10013		-- Firms		(ERM: 146, CRM: 10013)
		    END AS [RegardingObjectTypeCode],
		COALESCE([clients].[ReplicationCode], [firms].[ReplicationCode]) as [RegardingObjectId], 
		COALESCE([clients].[Name], [firms].[Name]) as [RegardingObjectIdName]
	FROM (
		SELECT [TaskId], [Reference], [ReferencedType], [ReferencedObjectId]
		FROM [Activity].[TaskReferences]
		WHERE [Reference] = 1 and [ReferencedType] = 200
		UNION ALL
		SELECT [TaskId], [Reference], [ReferencedType], [ReferencedObjectId]
		FROM [Activity].[TaskReferences]
		WHERE [Reference] = 1 and [ReferencedType] = 146
	) [refs]
	LEFT JOIN [Billing].[Clients] [clients] on [refs].[ReferencedObjectId] = [clients].[Id] and [ReferencedType] = 200
	LEFT JOIN [BusinessDirectory].[Firms] [firms] on [refs].[ReferencedObjectId] = [firms].[Id] and [ReferencedType] = 146
	WHERE [refs].[TaskId] = [tasks].[Id]
) [refs]

-- обновляем основную таблицу
INSERT INTO [{0}].[dbo].[TaskBase]
    ([ActivityId]) 
SELECT 
    [ReplicationCode] as [ActivityId]
FROM [Activity].[TaskBase]

-- обновляем дополнительную таблицу
INSERT INTO [{0}].[dbo].[TaskExtensionBase]
    ([ActivityId], [Dg_type]) 
SELECT 
    [ReplicationCode] as [ActivityId], 
    [TaskType] as [Dg_type]
FROM [Activity].[TaskBase]

-- обновляем связи
INSERT INTO [{0}].[dbo].[ActivityPartyBase]
    ([ActivityPartyId], [ActivityId], [ParticipationTypeMask], [PartyObjectTypeCode], [PartyId], [PartyIdName])
SELECT 
	NEWID(),
    [tasks].[ReplicationCode] as [ActivityId],
	CASE [refs].[Reference]
		WHEN 0 THEN 9			-- Owner				(CRM: 9)
		WHEN 1 THEN 8			-- RegardingObject		(ERM: 1, CRM: 8)
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
FROM [Activity].[TaskBase] [tasks]
CROSS APPLY (
	SELECT TOP 1
        [Reference], 
		[ReferencedType], 
		coalesce([clients].[ReplicationCode],[firms].[ReplicationCode]) as [ReferencedObjectId], 
		coalesce([clients].[Name],[firms].[Name]) as [ReferencedObjectName]
	FROM (
		SELECT [TaskId], [Reference], [ReferencedType], [ReferencedObjectId]
		FROM [Activity].[TaskReferences]
		WHERE [Reference] = 1 and [ReferencedType] = 200
		UNION ALL
		SELECT [TaskId], [Reference], [ReferencedType], [ReferencedObjectId]
		FROM [Activity].[TaskReferences]
		WHERE [Reference] = 1 and [ReferencedType] = 146
	) [refs]
	LEFT JOIN [Billing].[Clients] [clients] on [refs].[ReferencedObjectId] = [clients].[Id] and [ReferencedType] = 200
	LEFT JOIN [BusinessDirectory].[Firms] [firms] on [refs].[ReferencedObjectId] = [firms].[Id] and [ReferencedType] = 146
	WHERE [refs].[TaskId] = [tasks].[Id]
                
    UNION ALL
	            
    SELECT 
		0 as [Reference], 
		53 as [ReferencedType], 
		[crmOwners].[SystemUserId] as [ReferencedObjectId], 
		[owners].[DisplayName] as [ReferencedObjectName]
	FROM [Activity].[TaskBase] [ab]
	JOIN [Security].[Users] [owners] ON [owners].[Id] = [ab].[OwnerCode]
	LEFT JOIN [{0}].[dbo].[SystemUserErmView] [crmOwners] WITH ( NOEXPAND ) ON [crmOwners].[ErmUserAccount] = [owners].[Account] COLLATE Database_Default
	WHERE [ab].[Id] = [tasks].[Id]
) [refs]

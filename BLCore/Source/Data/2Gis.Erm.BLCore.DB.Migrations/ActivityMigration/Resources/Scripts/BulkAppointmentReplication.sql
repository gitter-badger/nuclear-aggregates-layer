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
    4201 as [ActivityTypeCode]
	, [appointments].[ReplicationCode] as [ActivityId]

	, [appointments].[Subject]
	, [appointments].[Description]
	, CASE [appointments].[Priority] WHEN 1 THEN 0 WHEN 2 THEN 1 WHEN 3 THEN 2 ELSE 1 END as [PriorityCode]
    -- (0 - Open, 1 - Completed, 2 - Canceled, 3 - Scheduled)
	-- (Open: 1 - Free, 2 - Tentative; Completed: 3 - Completed; Canceled: 4 - Canceled; Scheduled: 5 - Busy, 6 - Out of Office)
	, CASE [appointments].[Status] WHEN 1 THEN 3 WHEN 2 THEN 1 WHEN 3 THEN 2 END as [StateCode]
	, CASE [appointments].[Status] WHEN 1 THEN 5 WHEN 2 THEN 3 WHEN 3 THEN 4 END as [StatusCode]

	, [appointments].[CreatedOn] as [ActualStart]
	, CASE WHEN [appointments].[Status] = 2 OR [Status] = 3 THEN [appointments].[ModifiedOn] ELSE NULL END as [ActualEnd]
	, CASE WHEN [appointments].[Status] = 2 OR [Status] = 3 THEN DATEDIFF(minute, [appointments].[CreatedOn], [appointments].[ModifiedOn]) ELSE NULL END as [ActualDurationMinutes]

	, [appointments].[ScheduledStart]
	, [appointments].[ScheduledEnd]
	, DATEDIFF(minute, [appointments].[ScheduledStart], [appointments].[ScheduledEnd]) as [ScheduledDurationMinutes]

	, [refs].[RegardingObjectTypeCode]
	, [refs].[RegardingObjectId]
    , CASE WHEN [refs].[RegardingObjectId] IS NOT NULL THEN 0 END as [RegardingObjectIdDsc]
	, [refs].[RegardingObjectIdName]

	, [crmOwners].[BusinessUnitId] AS [OwningBusinessUnit]
	, [crmOwners].[SystemUserId] AS [OwningUser]

	, [crmCreators].[SystemUserId] as [CreatedBy]
	, [appointments].[CreatedOn]
	, [crmModifiers].[SystemUserId] as [ModifiedBy]
	, [appointments].[ModifiedOn]
    , CASE WHEN [appointments].[IsDeleted] = 1 THEN 2 ELSE 0 END as [DeletionStateCode]
FROM [Activity].[AppointmentBase] [appointments]
JOIN [Security].[Users] [owners] ON [owners].[Id] = [appointments].[OwnerCode]
LEFT JOIN [{0}].[dbo].[SystemUserErmView] [crmOwners] WITH ( NOEXPAND ) ON [crmOwners].[ErmUserAccount] = [owners].[Account] COLLATE Database_Default
JOIN [Security].[Users] [creators] ON [creators].[Id] = [appointments].[CreatedBy]
LEFT JOIN [{0}].[dbo].[SystemUserErmView] [crmCreators] WITH ( NOEXPAND ) ON [crmCreators].[ErmUserAccount] = [creators].[Account] COLLATE Database_Default
JOIN [Security].[Users] [modifiers] ON [modifiers].[Id] = [appointments].[CreatedBy]
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
		SELECT [AppointmentId], [Reference], [ReferencedType], [ReferencedObjectId]
		FROM [Activity].[AppointmentReferences]
		WHERE [Reference] = 1 and [ReferencedType] = 200
		UNION ALL
		SELECT [AppointmentId], [Reference], [ReferencedType], [ReferencedObjectId]
		FROM [Activity].[AppointmentReferences]
		WHERE [Reference] = 1 and [ReferencedType] = 146
	) [refs]
	LEFT JOIN [Billing].[Clients] [clients] on [refs].[ReferencedObjectId] = [clients].[Id] and [ReferencedType] = 200
	LEFT JOIN [BusinessDirectory].[Firms] [firms] on [refs].[ReferencedObjectId] = [firms].[Id] and [ReferencedType] = 146
	WHERE [refs].[AppointmentId] = [appointments].[Id]
) [refs]

-- обновляем основную таблицу
INSERT INTO [{0}].[dbo].[AppointmentBase]
    ([ActivityId], [Location]) 
SELECT 
    [ReplicationCode] as [ActivityId], 
    [Location]
FROM [Activity].[AppointmentBase]

-- обновляем дополнительную таблицу
INSERT INTO [{0}].[dbo].[AppointmentExtensionBase]
    ([ActivityId], [Dg_purpose], [Dg_result]) 
SELECT 
    [ReplicationCode] as [ActivityId], 
    [Purpose] as [Dg_purpose],
    1 as [Dg_result]
FROM [Activity].[AppointmentBase]

-- обновляем связи
INSERT INTO [{0}].[dbo].[ActivityPartyBase]
    ([ActivityPartyId], [ActivityId], [ParticipationTypeMask], [PartyObjectTypeCode], [PartyId], [PartyIdName])
SELECT 
	NEWID(),
    [appointments].[ReplicationCode] as [ActivityId],
	CASE [refs].[Reference]
		WHEN 0 THEN 9			-- Owner				(CRM: 9)
		WHEN 1 THEN 8			-- RegardingObject		(ERM: 1, CRM: 8)
		WHEN 2 THEN 7			-- Organizer			(ERM: 2, CRM: 7)
		WHEN 3 THEN 5			-- RequiredAttendees	(ERM: 3, CRM: 5)
		WHEN 4 THEN 6			-- OptionalAttendees	(ERM: 4, CRM: 6)
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
FROM [Activity].[AppointmentBase] [appointments]
CROSS APPLY (
	SELECT TOP 1
        [Reference], 
		[ReferencedType], 
		coalesce([clients].[ReplicationCode],[firms].[ReplicationCode]) as [ReferencedObjectId], 
		coalesce([clients].[Name],[firms].[Name]) as [ReferencedObjectName]
	FROM (
		SELECT [AppointmentId], [Reference], [ReferencedType], [ReferencedObjectId]
		FROM [Activity].[AppointmentReferences]
		WHERE [Reference] = 1 and [ReferencedType] = 200
		UNION ALL
		SELECT [AppointmentId], [Reference], [ReferencedType], [ReferencedObjectId]
		FROM [Activity].[AppointmentReferences]
		WHERE [Reference] = 1 and [ReferencedType] = 146
	) [refs]
	LEFT JOIN [Billing].[Clients] [clients] on [refs].[ReferencedObjectId] = [clients].[Id] and [ReferencedType] = 200
	LEFT JOIN [BusinessDirectory].[Firms] [firms] on [refs].[ReferencedObjectId] = [firms].[Id] and [ReferencedType] = 146
	WHERE [refs].[AppointmentId] = [appointments].[Id]
                
    UNION ALL
	            
    SELECT 
		[Reference],
		[ReferencedType], 
		coalesce([contacts].[ReplicationCode],[crmOwners].[SystemUserId]) as [ReferencedObjectId], 
		coalesce([contacts].[FullName],[owners].[DisplayName]) as [ReferencedObjectName]
	FROM [Activity].[AppointmentReferences]
	LEFT JOIN [Billing].[Contacts] contacts on ReferencedObjectId = contacts.Id and ReferencedType = 206
	LEFT JOIN [Security].[Users] [owners] on ReferencedObjectId = [owners].Id and ReferencedType = 53
	LEFT JOIN [{0}].[dbo].[SystemUserErmView] [crmOwners] WITH ( NOEXPAND ) ON [crmOwners].[ErmUserAccount] = [owners].[Account] COLLATE Database_Default
	WHERE [Reference] != 1 and [AppointmentId] = [appointments].[Id]
	            
    UNION ALL
	            
    SELECT 
		0 as [Reference], 
		53 as [ReferencedType], 
		[crmOwners].[SystemUserId] as [ReferencedObjectId], 
		[owners].[DisplayName] as [ReferencedObjectName]
	FROM [Activity].[AppointmentBase] [ab]
	JOIN [Security].[Users] [owners] ON [owners].[Id] = [ab].[OwnerCode]
	LEFT JOIN [{0}].[dbo].[SystemUserErmView] [crmOwners] WITH ( NOEXPAND ) ON [crmOwners].[ErmUserAccount] = [owners].[Account] COLLATE Database_Default
	WHERE [ab].[Id] = [appointments].[Id]
) [refs]

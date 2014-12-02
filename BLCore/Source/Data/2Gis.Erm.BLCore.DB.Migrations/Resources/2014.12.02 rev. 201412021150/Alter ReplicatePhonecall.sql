ALTER PROCEDURE [Activity].[ReplicatePhonecall]
	@Id bigint = NULL
AS
    IF @Id IS NULL RETURN 0;

    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRAN;

        -- обновляем общую таблицу
        MERGE INTO [DoubleGis_MSCRM].[dbo].[ActivityPointerBase] AS [Current]
        USING (
            SELECT 
                4210 as [ActivityTypeCode]
	            , [phonecalls].[ReplicationCode] as [ActivityId]

	            , [phonecalls].[Subject]
	            , [phonecalls].[Description]
	            , CASE [phonecalls].[Priority] WHEN 1 THEN 0 WHEN 2 THEN 1 WHEN 3 THEN 2 ELSE 1 END as [PriorityCode]
                -- (0 - Open, 1 - Completed, 2 - Canceled)
                -- (Open: 1 - Open; Completed: 2 - Made; 4 - Received; Canceled: 3 - Canceled)
                , CASE [phonecalls].[Status] WHEN 1 THEN 0 WHEN 2 THEN 1 WHEN 3 THEN 2 END as [StateCode]
                , CASE [phonecalls].[Status] WHEN 1 THEN 1 WHEN 2 THEN 2 WHEN 3 THEN 3 END as [StatusCode]

	            , [phonecalls].[CreatedOn] as [ActualStart]
	            , CASE WHEN [phonecalls].[Status] = 2 OR [Status] = 3 THEN [phonecalls].[ModifiedOn] ELSE NULL END as [ActualEnd]
	            , CASE WHEN [phonecalls].[Status] = 2 OR [Status] = 3 THEN DATEDIFF(minute, [phonecalls].[CreatedOn], [phonecalls].[ModifiedOn]) ELSE NULL END as [ActualDurationMinutes]

	            , [phonecalls].[ScheduledOn] as [ScheduledStart]
	            , [phonecalls].[ScheduledOn] as [ScheduledEnd]
	            , 0 as [ScheduledDurationMinutes]

	            , [refs].[RegardingObjectTypeCode]
	            , [refs].[RegardingObjectId]
                , CASE WHEN [refs].[RegardingObjectId] IS NOT NULL THEN 0 END as [RegardingObjectIdDsc]
	            , [refs].[RegardingObjectIdName]

	            , [crmOwners].[BusinessUnitId] AS [OwningBusinessUnit]
	            , [crmOwners].[SystemUserId] AS [OwningUser]

	            , [crmCreators].[SystemUserId] as [CreatedBy]
	            , [phonecalls].[CreatedOn]
	            , [crmModifiers].[SystemUserId] as [ModifiedBy]
	            , [phonecalls].[ModifiedOn]
                , CASE WHEN [phonecalls].[IsDeleted] = 1 THEN 2 ELSE 0 END as [DeletionStateCode]
            FROM [Activity].[PhonecallBase] [phonecalls]
            JOIN [Security].[Users] [owners] ON [owners].[Id] = [phonecalls].[OwnerCode]
            LEFT JOIN [DoubleGis_MSCRM].[dbo].[SystemUserErmView] [crmOwners] WITH ( NOEXPAND ) ON [crmOwners].[ErmUserAccount] = [owners].[Account] COLLATE Database_Default
            JOIN [Security].[Users] [creators] ON [creators].[Id] = [phonecalls].[CreatedBy]
            LEFT JOIN [DoubleGis_MSCRM].[dbo].[SystemUserErmView] [crmCreators] WITH ( NOEXPAND ) ON [crmCreators].[ErmUserAccount] = [creators].[Account] COLLATE Database_Default
            JOIN [Security].[Users] [modifiers] ON [modifiers].[Id] = [phonecalls].[CreatedBy]
            LEFT JOIN [DoubleGis_MSCRM].[dbo].[SystemUserErmView] [crmModifiers] WITH ( NOEXPAND ) ON [crmModifiers].[ErmUserAccount] = [modifiers].[Account] COLLATE Database_Default
            OUTER APPLY (
	            SELECT TOP 1
	                CASE [refs].[ReferencedType] 
		                WHEN 200 THEN 1			-- Clients		(ERM: 200, CRM: 1)
		                WHEN 146 THEN 10013		-- Firms		(ERM: 146, CRM: 10013)
		                END AS [RegardingObjectTypeCode],
		            COALESCE([clients].[ReplicationCode], [firms].[ReplicationCode]) as [RegardingObjectId], 
		            COALESCE([clients].[Name], [firms].[Name]) as [RegardingObjectIdName]
	            FROM (
		            SELECT [PhonecallId], [Reference], [ReferencedType], [ReferencedObjectId]
		            FROM [Activity].[PhonecallReferences]
		            WHERE [Reference] = 1 and [ReferencedType] = 200
		            UNION ALL
		            SELECT [PhonecallId], [Reference], [ReferencedType], [ReferencedObjectId]
		            FROM [Activity].[PhonecallReferences]
		            WHERE [Reference] = 1 and [ReferencedType] = 146
	            ) [refs]
	            LEFT JOIN [Billing].[Clients] [clients] on [refs].[ReferencedObjectId] = [clients].[Id] and [ReferencedType] = 200
	            LEFT JOIN [BusinessDirectory].[Firms] [firms] on [refs].[ReferencedObjectId] = [firms].[Id] and [ReferencedType] = 146
	            WHERE [refs].[PhonecallId] = [phonecalls].[Id]
            ) [refs]
            WHERE [phonecalls].[Id] = @Id
        ) AS [Modified]
        ON [Current].[ActivityId] = [Modified].[ActivityId] 
        WHEN MATCHED THEN
	        UPDATE SET 
				[Current].[Subject] = [Modified].[Subject],
				[Current].[Description] = [Modified].[Description],
				[Current].[PriorityCode] = [Modified].[PriorityCode],
				[Current].[StateCode] = [Modified].[StateCode],
				[Current].[StatusCode] = [Modified].[StatusCode],

				[Current].[ActualStart] = [Modified].[ActualStart],
				[Current].[ActualEnd] = [Modified].[ActualEnd],
				[Current].[ActualDurationMinutes] = [Modified].[ActualDurationMinutes],
				
                [Current].[ScheduledStart] = [Modified].[ScheduledStart],
				[Current].[ScheduledEnd] = [Modified].[ScheduledEnd],
				[Current].[ScheduledDurationMinutes] = [Modified].[ScheduledDurationMinutes],

                [Current].[RegardingObjectTypeCode] = [Modified].[RegardingObjectTypeCode],
                [Current].[RegardingObjectId] = [Modified].[RegardingObjectId],
                [Current].[RegardingObjectIdDsc] = [Modified].[RegardingObjectIdDsc],
				[Current].[RegardingObjectIdName] = [Modified].[RegardingObjectIdName],

                [Current].[OwningBusinessUnit] = [Modified].[OwningBusinessUnit],
				[Current].[OwningUser] = [Modified].[OwningUser],

				[Current].[ModifiedBy] = [Modified].[ModifiedBy],
				[Current].[ModifiedOn] = [Modified].[ModifiedOn],
				[Current].[DeletionStateCode] = [Modified].[DeletionStateCode]
        WHEN NOT MATCHED BY TARGET THEN
	        INSERT (
                [ActivityTypeCode], [ActivityId], 
                [Subject], [Description], [PriorityCode], [StateCode], [StatusCode],
                [ActualStart], [ActualEnd], [ActualDurationMinutes],
                [ScheduledStart], [ScheduledEnd], [ScheduledDurationMinutes],
	            [RegardingObjectTypeCode], [RegardingObjectId], [RegardingObjectIdDsc], [RegardingObjectIdName],
                [OwningBusinessUnit], [OwningUser],
	            [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn],
                [DeletionStateCode]
            )
	        VALUES ( 
                [ActivityTypeCode], [ActivityId], 
                [Subject], [Description], [PriorityCode], [StateCode], [StatusCode],
                [ActualStart], [ActualEnd], [ActualDurationMinutes],
                [ScheduledStart], [ScheduledEnd], [ScheduledDurationMinutes],
	            [RegardingObjectTypeCode], [RegardingObjectId], [RegardingObjectIdDsc], [RegardingObjectIdName],
                [OwningBusinessUnit], [OwningUser],
	            [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn],
                [DeletionStateCode]
            )
		;

        -- обновляем основную таблицу
        MERGE INTO [DoubleGis_MSCRM].[dbo].[PhonecallBase] as [Current]
        USING (
            SELECT 
                [ReplicationCode] as [ActivityId]
            FROM [Activity].[PhonecallBase]
            WHERE [Id] = @Id
        ) as [Modified]
        ON [Current].[ActivityId] = [Modified].[ActivityId]
        WHEN NOT MATCHED BY TARGET THEN
            INSERT ([ActivityId]) 
            VALUES ([ActivityId])
		;

        -- обновляем дополнительную таблицу
        MERGE INTO [DoubleGis_MSCRM].[dbo].[PhonecallExtensionBase] as [Current]
        USING (
            SELECT 
                [ReplicationCode] as [ActivityId], 
                [Purpose] as [Dg_purpose],
                1 as [Dg_result]
            FROM [Activity].[PhonecallBase]
            WHERE [Id] = @Id
        ) as [Modified]
        ON [Current].[ActivityId] = [Modified].[ActivityId]
        WHEN MATCHED THEN
	        UPDATE SET
                [Current].[Dg_purpose] = [Modified].[Dg_purpose],
                [Current].[Dg_result] = [Modified].[Dg_result]
        WHEN NOT MATCHED BY TARGET THEN
            INSERT ([ActivityId], [Dg_purpose], [Dg_result]) 
            VALUES ([ActivityId], [Dg_purpose], [Dg_result])
		;

        -- обновляем связи

		DELETE FROM [DoubleGis_MSCRM].[dbo].[ActivityPartyBase] 
		FROM [DoubleGis_MSCRM].[dbo].[ActivityPartyBase]
		JOIN [Activity].[PhonecallBase] [phonecalls]
		ON [ActivityId] = [phonecalls].[ReplicationCode] AND [phonecalls].[Id] = @Id;

        INSERT INTO [DoubleGis_MSCRM].[dbo].[ActivityPartyBase]
            ([ActivityPartyId], [ActivityId], [ParticipationTypeMask], [PartyObjectTypeCode], [PartyId], [PartyIdName])
        SELECT 
	        NEWID(),
            [phonecalls].[ReplicationCode] as [ActivityId],
	        CASE [refs].[Reference]
			    WHEN 0 THEN 9			-- Owner			(CRM: 9)
			    WHEN 1 THEN 8			-- RegardingObject	(ERM: 1, CRM: 8)
			    WHEN 2 THEN 2			-- Recipient		(ERM: 2, CRM: 2)
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
        FROM [Activity].[PhonecallBase] [phonecalls]
        CROSS APPLY (
	        SELECT TOP 1
                [Reference], 
		        [ReferencedType], 
		        coalesce([clients].[ReplicationCode],[firms].[ReplicationCode]) as [ReferencedObjectId], 
		        coalesce([clients].[Name],[firms].[Name]) as [ReferencedObjectName]
	        FROM (
		        SELECT [PhonecallId], [Reference], [ReferencedType], [ReferencedObjectId]
		        FROM [Activity].[PhonecallReferences]
		        WHERE [Reference] = 1 and [ReferencedType] = 200
		        UNION ALL
		        SELECT [PhonecallId], [Reference], [ReferencedType], [ReferencedObjectId]
		        FROM [Activity].[PhonecallReferences]
		        WHERE [Reference] = 1 and [ReferencedType] = 146
	        ) [refs]
	        LEFT JOIN [Billing].[Clients] [clients] on [refs].[ReferencedObjectId] = [clients].[Id] and [ReferencedType] = 200
	        LEFT JOIN [BusinessDirectory].[Firms] [firms] on [refs].[ReferencedObjectId] = [firms].[Id] and [ReferencedType] = 146
	        WHERE [refs].[PhonecallId] = [phonecalls].[Id]
                
            UNION ALL
	            
            SELECT 
		        [Reference],
		        [ReferencedType], 
		        coalesce([contacts].[ReplicationCode],[crmOwners].[SystemUserId]) as [ReferencedObjectId], 
		        coalesce([contacts].[FullName],[owners].[DisplayName]) as [ReferencedObjectName]
	        FROM [Activity].[PhonecallReferences]
	        LEFT JOIN [Billing].[Contacts] contacts on ReferencedObjectId = contacts.Id and ReferencedType = 206
	        LEFT JOIN [Security].[Users] [owners] on ReferencedObjectId = [owners].Id and ReferencedType = 53
	        LEFT JOIN [DoubleGis_MSCRM].[dbo].[SystemUserErmView] [crmOwners] WITH ( NOEXPAND ) ON [crmOwners].[ErmUserAccount] = [owners].[Account] COLLATE Database_Default
	        WHERE [Reference] != 1 and [PhonecallId] = [phonecalls].[Id]
	            
            UNION ALL
	            
            SELECT 
		        0 as [Reference], 
		        53 as [ReferencedType], 
		        [crmOwners].[SystemUserId] as [ReferencedObjectId], 
		        [owners].DisplayName as [ReferencedObjectName]
	        FROM [Activity].[PhonecallBase] [ab]
	        JOIN [Security].[Users] [owners] ON [owners].[Id] = [ab].[OwnerCode]
	        LEFT JOIN [DoubleGis_MSCRM].[dbo].[SystemUserErmView] [crmOwners] WITH ( NOEXPAND ) ON [crmOwners].[ErmUserAccount] = [owners].[Account] COLLATE Database_Default
	        WHERE [ab].[Id] = [phonecalls].[Id]
        ) [refs]
		WHERE [phonecalls].[Id] = @Id;

        COMMIT TRAN;

        RETURN 1;
    END TRY
    BEGIN CATCH
        IF ( XACT_STATE() != 0 )
            BEGIN
                ROLLBACK TRAN;
            END;

        DECLARE @ErrorMessage NVARCHAR(MAX),
            @ErrorSeverity INT,
            @ErrorState INT;

        SELECT  @ErrorMessage = ERROR_MESSAGE(),
                @ErrorSeverity = ERROR_SEVERITY(),
                @ErrorState = ERROR_STATE();

        RAISERROR ( @ErrorMessage , @ErrorSeverity , @ErrorState );
    END CATCH;

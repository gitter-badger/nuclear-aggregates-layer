CREATE PROCEDURE [Activity].[ReplicateLetter]
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

	            , [crmOwners].[BusinessUnitId] AS [OwningBusinessUnit]
	            , [crmOwners].[SystemUserId] AS [OwningUser]

	            , [crmCreators].[SystemUserId] as [CreatedBy]
	            , [letters].[CreatedOn]
	            , [crmModifiers].[SystemUserId] as [ModifiedBy]
	            , [letters].[ModifiedOn]
                , CASE WHEN [letters].[IsDeleted] = 1 THEN 2 ELSE 0 END as [DeletionStateCode]
            FROM [Activity].[LetterBase] [letters]
            JOIN [Security].[Users] [owners] ON [owners].[Id] = [letters].[OwnerCode]
            LEFT JOIN [DoubleGis_MSCRM].[dbo].[SystemUserErmView] [crmOwners] WITH ( NOEXPAND ) ON [crmOwners].[ErmUserAccount] = [owners].[Account] COLLATE Database_Default
            JOIN [Security].[Users] [creators] ON [creators].[Id] = [letters].[CreatedBy]
            LEFT JOIN [DoubleGis_MSCRM].[dbo].[SystemUserErmView] [crmCreators] WITH ( NOEXPAND ) ON [crmCreators].[ErmUserAccount] = [creators].[Account] COLLATE Database_Default
            JOIN [Security].[Users] [modifiers] ON [modifiers].[Id] = [letters].[ModifiedBy]
            LEFT JOIN [DoubleGis_MSCRM].[dbo].[SystemUserErmView] [crmModifiers] WITH ( NOEXPAND ) ON [crmModifiers].[ErmUserAccount] = [modifiers].[Account] COLLATE Database_Default
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
            WHERE [letters].[Id] = @Id
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
	            [RegardingObjectTypeCode], [RegardingObjectId], [RegardingObjectIdName],
                [OwningBusinessUnit], [OwningUser],
	            [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn],
                [DeletionStateCode]
            )
	        VALUES ( 
                [ActivityTypeCode], [ActivityId], 
                [Subject], [Description], [PriorityCode], [StateCode], [StatusCode],
                [ActualStart], [ActualEnd], [ActualDurationMinutes],
                [ScheduledStart], [ScheduledEnd], [ScheduledDurationMinutes],
	            [RegardingObjectTypeCode], [RegardingObjectId], [RegardingObjectIdName],
                [OwningBusinessUnit], [OwningUser],
	            [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn],
                [DeletionStateCode]
            )
		;

        -- обновляем основную таблицу
        MERGE INTO [DoubleGis_MSCRM].[dbo].[LetterBase] as [Current]
        USING (
            SELECT 
                [ReplicationCode] as [ActivityId]
            FROM [Activity].[LetterBase]
            WHERE [Id] = @Id
        ) as [Modified]
        ON [Current].[ActivityId] = [Modified].[ActivityId]
        WHEN NOT MATCHED BY TARGET THEN
            INSERT ([ActivityId]) 
            VALUES ([ActivityId])
		;

        -- обновляем дополнительную таблицу
        MERGE INTO [DoubleGis_MSCRM].[dbo].[LetterExtensionBase] as [Current]
        USING (
            SELECT 
                [ReplicationCode] as [ActivityId]
            FROM [Activity].[LetterBase]
            WHERE [Id] = @Id
        ) as [Modified]
        ON [Current].[ActivityId] = [Modified].[ActivityId]
        WHEN NOT MATCHED BY TARGET THEN
            INSERT ([ActivityId]) 
            VALUES ([ActivityId])
		;

        -- обновляем связи

		DELETE FROM [DoubleGis_MSCRM].[dbo].[ActivityPartyBase] 
		FROM [DoubleGis_MSCRM].[dbo].[ActivityPartyBase]
		JOIN [Activity].[LetterBase] [letters]
		ON [ActivityId] = [letters].[ReplicationCode] AND [letters].[Id] = @Id;

		INSERT INTO [DoubleGis_MSCRM].[dbo].[ActivityPartyBase]
			([ActivityPartyId], [ActivityId], [ParticipationTypeMask], [PartyObjectTypeCode], [PartyId], [PartyIdName])
		SELECT
			NEWID(), 
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
		        coalesce([contacts].[ReplicationCode],[crmOwners].[SystemUserId]) as [ReferencedObjectId], 
		        coalesce([contacts].[FullName],[owners].[DisplayName]) as [ReferencedObjectName]
	        FROM [Activity].[LetterReferences]
	        LEFT JOIN [Billing].[Contacts] contacts on ReferencedObjectId = contacts.Id and ReferencedType = 206
	        LEFT JOIN [Security].[Users] [owners] on ReferencedObjectId = [owners].Id and ReferencedType = 53
	        LEFT JOIN [DoubleGis_MSCRM].[dbo].[SystemUserErmView] [crmOwners] WITH ( NOEXPAND ) ON [crmOwners].[ErmUserAccount] = [owners].[Account] COLLATE Database_Default
	        WHERE [Reference] != 1 and [LetterId] = [letters].[Id]
	            
            UNION ALL
	            
            SELECT 
		        0 as [Reference], 
		        53 as [ReferencedType], 
		        [crmOwners].[SystemUserId] as [ReferencedObjectId], 
		        [owners].DisplayName as [ReferencedObjectName]
	        FROM [Activity].[LetterBase] [ab]
	        JOIN [Security].[Users] [owners] ON [owners].[Id] = [ab].[OwnerCode]
	        LEFT JOIN [DoubleGis_MSCRM].[dbo].[SystemUserErmView] [crmOwners] WITH ( NOEXPAND ) ON [crmOwners].[ErmUserAccount] = [owners].[Account] COLLATE Database_Default
	        WHERE [ab].[Id] = [letters].[Id]
        ) [refs]
		WHERE [letters].[Id] = @Id;

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

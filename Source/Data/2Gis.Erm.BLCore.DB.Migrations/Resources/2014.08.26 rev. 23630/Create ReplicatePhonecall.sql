-- changes
--   rev.1 - initial
CREATE PROCEDURE [Activity].[ReplicatePhonecall]
	@Id bigint = NULL
AS
    IF @Id IS NULL RETURN 0;

    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @CrmId UNIQUEIDENTIFIER;
    DECLARE @OwnerUserDomainName NVARCHAR(250);
    DECLARE @OwnerUserId UNIQUEIDENTIFIER;
    DECLARE @OwnerUserBusinessUnitId UNIQUEIDENTIFIER;

    -- get owner user domain name, CRM replication code
    SELECT @CrmId = [ermBase].[ReplicationCode],
           @OwnerUserDomainName = [users].[Account]
    FROM [Activity].[PhonecallBase] AS [ermBase]
	    LEFT OUTER JOIN [Security].[Users] AS [users] ON [users].[Id] = [ermBase].[OwnerCode]
    WHERE [ermBase].[Id] = @Id;

    -- get owner user CRM UserId
    SELECT 
	    @OwnerUserId = [SystemUserId],
	    @OwnerUserBusinessUnitId = [BusinessUnitId]
    FROM [DoubleGis_MSCRM].[dbo].[SystemUserBase]
    WHERE [DomainName] LIKE N'%\' + @OwnerUserDomainName;

    DECLARE @RegardingObjectId UNIQUEIDENTIFIER;
	DECLARE @RegardingObjectTypeCode INT;
    DECLARE @RegardingObjectIdName NVARCHAR(400);

	-- get deal regarding object id
	SELECT @RegardingObjectId = deals.ReplicationCode, @RegardingObjectTypeCode = 3, @RegardingObjectIdName = deals.Name
	FROM [Activity].[PhonecallReferences] refs
		LEFT OUTER JOIN [Billing].[Deals] deals ON refs.ReferencedObjectId = deals.Id
	WHERE refs.Reference = 1 AND refs.ReferencedType = 199 AND refs.PhonecallId = @Id;

	-- get contact regarding object id
	IF (@RegardingObjectId IS NULL)
		SELECT @RegardingObjectId = contacts.ReplicationCode, @RegardingObjectTypeCode = 2, @RegardingObjectIdName = contacts.FullName
		FROM [Activity].[PhonecallReferences] refs
			LEFT OUTER JOIN [Billing].[Contacts] contacts ON refs.ReferencedObjectId = contacts.Id
		WHERE refs.Reference = 1 AND refs.ReferencedType = 206 AND refs.PhonecallId = @Id;

	-- get client regarding object id
	IF (@RegardingObjectId IS NULL)
		SELECT @RegardingObjectId = clients.ReplicationCode, @RegardingObjectTypeCode = 1, @RegardingObjectIdName = clients.Name
		FROM [Activity].[PhonecallReferences] refs
			LEFT OUTER JOIN [Billing].[Clients] clients ON refs.ReferencedObjectId = clients.Id
		WHERE refs.Reference = 1 AND refs.ReferencedType = 200 AND refs.PhonecallId = @Id;
	
	-- get firm regarding object id
	IF (@RegardingObjectId IS NULL)
		SELECT @RegardingObjectId = firms.ReplicationCode, @RegardingObjectTypeCode = 1, @RegardingObjectIdName = firms.Name
		FROM [Activity].[PhonecallReferences] refs
			LEFT OUTER JOIN [BusinessDirectory].[Firms] firms ON refs.ReferencedObjectId = firms.Id
		WHERE refs.Reference = 1 AND refs.ReferencedType = 146 AND refs.PhonecallId = @Id;

    IF NOT EXISTS (SELECT 1 FROM [DoubleGis_MSCRM].[dbo].[ActivityPointerBase] WHERE [ActivityId] = @CrmId)
    --------------------------------------------------------------------------
    -- there is no such a record in the CRM database => insert a brand new one
    --------------------------------------------------------------------------
    BEGIN

		INSERT INTO [DoubleGis_MSCRM].[dbo].[ActivityPointerBase]
			( [ActivityId]
			, [ActivityTypeCode]
			, [CreatedOn]						
			, [CreatedBy]						
			, [ModifiedOn]						
			, [ModifiedBy]						

			, [Subject]							
			, [Description]						
			, [ActualStart]						
			, [ActualEnd]						
			, [ActualDurationMinutes]			
			, [ScheduledStart]					
			, [ScheduledEnd]					
			, [ScheduledDurationMinutes]		

			, [OwningBusinessUnit]				
			, [OwningUser]						

			, [StateCode]						-- (0 - Open, 1 - Completed, 2 - Canceled)
			, [StatusCode]						-- (Open: 1 - Open; Completed: 2 - Made; 4 - Received; Canceled: 3 - Canceled)

			, [RegardingObjectId]				
			, [RegardingObjectTypeCode]
			, [RegardingObjectIdDsc]
			, [RegardingObjectIdName]
			, [RegardingObjectIdYomiName]

			, [PriorityCode]					
			)
		SELECT
			  [ReplicationCode]
			, 4210								-- phonecall
		    , [CreatedOn]
		    , Shared.GetCrmUserId([CreatedBy])
		    , [ModifiedOn]
		    , Shared.GetCrmUserId([ModifiedBy])
			  
			, [Subject]
			, [Description]
			, [CreatedOn]
			, CASE WHEN [Status] = 2 OR [Status] = 3 THEN [ModifiedOn] ELSE NULL END
			, CASE WHEN [Status] = 2 OR [Status] = 3 THEN DATEDIFF(minute, [ModifiedOn], [CreatedOn]) ELSE NULL END
			, [ScheduledStart]					
			, [ScheduledEnd]					
			, DATEDIFF(minute, [ScheduledStart], [ScheduledEnd])

		    , @OwnerUserBusinessUnitId
			, @OwnerUserId

			, CASE [Status] WHEN 1 THEN 0 WHEN 2 THEN 1 WHEN 3 THEN 2 ELSE NULL END
			, CASE [Status] WHEN 1 THEN 1 WHEN 2 THEN 2 WHEN 3 THEN 3 ELSE NULL END

			, @RegardingObjectId
			, @RegardingObjectTypeCode
			, 0
			, @RegardingObjectIdName
			, NULL

			, CASE [Priority] WHEN 1 THEN 0 WHEN 2 THEN 1 WHEN 3 THEN 2 ELSE 1 END

	    FROM [Activity].[PhonecallBase]
	    WHERE [Id] = @Id;

		INSERT INTO [DoubleGis_MSCRM].[dbo].[PhonecallBase]
			( [ActivityId]
			, [PhoneNumber]
			, [DirectionCode]
			)
		SELECT
			  @CrmId
			, [PhoneNumber]
			, [Direction]
	    FROM [Activity].[PhonecallBase]
	    WHERE [Id] = @Id;

		INSERT INTO [DoubleGis_MSCRM].[dbo].[PhonecallExtensionBase]
			( [ActivityId]
			, [Dg_purpose]
			, [Dg_result]
			)
		SELECT
			  @CrmId
			, [Purpose]
			, 1
	    FROM [Activity].[PhonecallBase]
	    WHERE [Id] = @Id;

    END

    -------------------------------------------------------
    -- there is already saved record => update existing one
    -------------------------------------------------------
    ELSE
    BEGIN
		
	    UPDATE [crmPointer]
		    SET 
				  [DeletionStateCode] = CASE WHEN [ermBase].[IsDeleted] = 1 THEN 2 ELSE 0 END
				, [ModifiedBy] = Shared.GetCrmUserId([ermBase].[ModifiedBy])
				, [ModifiedOn] = [ermBase].[ModifiedOn]

				, [Subject]	= [ermBase].[Subject]
				, [Description] = [ermBase].[Description]
				, [ActualEnd] = CASE WHEN [ermBase].[Status] = 2 OR [ermBase].[Status] = 3 THEN [ermBase].[ModifiedOn] ELSE NULL END
				, [ActualDurationMinutes] = CASE WHEN [ermBase].[Status] = 2 OR [ermBase].[Status] = 3 THEN DATEDIFF(minute, [ermBase].[ModifiedOn], [ermBase].[CreatedOn]) ELSE NULL END
				, [ScheduledStart] = [ermBase].[ScheduledStart]
				, [ScheduledEnd] = [ermBase].[ScheduledEnd]
				, [ScheduledDurationMinutes] = DATEDIFF(minute, [ermBase].[ScheduledStart], [ermBase].[ScheduledEnd])

				, [OwningBusinessUnit] = @OwnerUserBusinessUnitId
				, [OwningUser] = @OwnerUserId

				, [StateCode]  = CASE [Status] WHEN 1 THEN 0 WHEN 2 THEN 1 WHEN 3 THEN 2 ELSE NULL END
				, [StatusCode] = CASE [Status] WHEN 1 THEN 1 WHEN 2 THEN 2 WHEN 3 THEN 3 ELSE NULL END

				, [RegardingObjectId] = @RegardingObjectId
				, [RegardingObjectTypeCode] = @RegardingObjectTypeCode
				, [RegardingObjectIdDsc] = CASE WHEN (@RegardingObjectId IS NOT NULL) THEN 0 ELSE NULL END
				, [RegardingObjectIdName] = @RegardingObjectIdName

				, [PriorityCode] = CASE [Priority] WHEN 1 THEN 0 WHEN 2 THEN 1 WHEN 3 THEN 2 ELSE 1 END
	    FROM [DoubleGis_MSCRM].[dbo].[ActivityPointerBase] as [crmPointer]
		    INNER JOIN [Activity].[PhonecallBase] as [ermBase] 
			ON [crmPointer].[ActivityId] = [ermBase].[ReplicationCode] AND [ermBase].[Id] = @Id;

	    UPDATE [crmBase]
		   SET [PhoneNumber] = [ermBase].[PhoneNumber]
		     , [DirectionCode] = [ermBase].[Direction]
	    FROM [DoubleGis_MSCRM].[dbo].[PhonecallBase] [crmBase]
		    INNER JOIN [Activity].[PhonecallBase] [ermBase]
			ON [crmBase].[ActivityId] = [ermBase].[ReplicationCode] AND [ermBase].Id = @Id;

	    UPDATE [crmExtension]
		   SET [Dg_purpose] = [ermBase].[Purpose]
	    FROM [DoubleGis_MSCRM].[dbo].[PhonecallExtensionBase] [crmExtension]
		    INNER JOIN [Activity].[PhonecallBase] [ermBase]
			ON [crmExtension].[ActivityId] = [ermBase].[ReplicationCode] AND [ermBase].Id = @Id;

    END;
	
	DELETE FROM [DoubleGis_MSCRM].[dbo].[ActivityPartyBase] WHERE [ActivityId] = @CrmId;
	INSERT INTO [DoubleGis_MSCRM].[dbo].[ActivityPartyBase]
		([ActivityId],[ActivityPartyId],[PartyId],[PartyObjectTypeCode],[ParticipationTypeMask])
	SELECT
		phonecalls.ReplicationCode,
		NEWID(),
		ISNULL(
			clients.ReplicationCode, 
			ISNULL(contacts.ReplicationCode, 
			ISNULL(deals.ReplicationCode, 
			ISNULL(firms.ReplicationCode, 
			Shared.GetCrmUserId(users.Id)
			)))) as PartyId,
		CASE refs.ReferencedType 
			WHEN 200 THEN 1			-- Clients		(ERM: 200, CRM: 1)
			WHEN 206 THEN 2			-- Contacts		(ERM: 206, CRM: 2)
			WHEN 199 THEN 3			-- Deals		(ERM: 199, CRM: 3)
			WHEN 146 THEN 10013		-- Firms		(ERM: 146, CRM: 10013)
			WHEN 53 THEN 8			-- Users		(ERM: 53, CRM: 8)
			END AS [PartyObjectTypeCode],
		CASE refs.Reference 
			WHEN 0 THEN 9			-- Owner			(CRM: 9)
			WHEN 1 THEN 8			-- RegardingObject	(ERM: 1, CRM: 8)
			WHEN 2 THEN 2			-- Recipient		(ERM: 2, CRM: 2)
			END AS [ParticipationTypeMask]
	FROM (
		SELECT 
			[regardingObject].[PhonecallId],
			[regardingObject].[Reference],
			[regardingObject].[ReferencedType],
			[regardingObject].[ReferencedObjectId]
		FROM (
			SELECT TOP 1 
				[PhonecallId],
				[Reference],
				[ReferencedType],
				[ReferencedObjectId],
				CASE ReferencedType 
					WHEN 199 THEN 1 -- a deal is the 1st option to refer to if any
					WHEN 206 THEN 2 -- then a contact 
					WHEN 200 THEN 3 -- then a client
					ELSE 4			-- otherwise a firm
					END as [Order]
			FROM [Activity].[PhonecallReferences]
			WHERE Reference = 1 AND PhonecallId = @Id 
			ORDER BY [Order]
			) [regardingObject]
		UNION ALL -- process other references except regarding object
		SELECT 				
			[PhonecallId],
			[Reference],
			[ReferencedType],
			[ReferencedObjectId]
		FROM [Activity].[PhonecallReferences]
		WHERE Reference != 1 AND PhonecallId = @Id
		UNION ALL -- add owner reference
		SELECT
			[Id] as PhonecallId,
			0 as Reference,
			53 as ReferencedType,		-- Users (ERM: 53)
			[OwnerCode] as ReferencedObjectId
		FROM [Activity].[PhonecallBase]
		WHERE Id = @Id
		) refs
		JOIN [Activity].[PhonecallBase] phonecalls on refs.PhonecallId = phonecalls.Id
		LEFT JOIN [Billing].[Clients] clients on refs.ReferencedObjectId = clients.Id and refs.ReferencedType = 200
		LEFT JOIN [Billing].[Contacts] contacts on refs.ReferencedObjectId = contacts.Id and refs.ReferencedType = 206
		LEFT JOIN [Billing].[Deals] deals on refs.ReferencedObjectId = deals.Id and refs.ReferencedType = 199
		LEFT JOIN [BusinessDirectory].[Firms] firms on refs.ReferencedObjectId = firms.Id and refs.ReferencedType = 146
		LEFT JOIN [Security].[Users] users on refs.ReferencedObjectId = users.Id and refs.ReferencedType = 53

    RETURN 1;

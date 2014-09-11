-- changes
--   rev.1 - initial
CREATE PROCEDURE [Activity].[ReplicateAppointment]
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
    FROM [Activity].[AppointmentBase] AS [ermBase]
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
	FROM [Activity].[AppointmentReferences] refs
		LEFT OUTER JOIN [Billing].[Deals] deals ON refs.ReferencedObjectId = deals.Id
	WHERE refs.Reference = 1 AND refs.ReferencedType = 199 AND refs.AppointmentId = @Id;

	-- get contact regarding object id
	IF (@RegardingObjectId IS NULL)
		SELECT @RegardingObjectId = contacts.ReplicationCode, @RegardingObjectTypeCode = 2, @RegardingObjectIdName = contacts.FullName
		FROM [Activity].[AppointmentReferences] refs
			LEFT OUTER JOIN [Billing].[Contacts] contacts ON refs.ReferencedObjectId = contacts.Id
		WHERE refs.Reference = 1 AND refs.ReferencedType = 206 AND refs.AppointmentId = @Id;

	-- get client regarding object id
	IF (@RegardingObjectId IS NULL)
		SELECT @RegardingObjectId = clients.ReplicationCode, @RegardingObjectTypeCode = 1, @RegardingObjectIdName = clients.Name
		FROM [Activity].[AppointmentReferences] refs
			LEFT OUTER JOIN [Billing].[Clients] clients ON refs.ReferencedObjectId = clients.Id
		WHERE refs.Reference = 1 AND refs.ReferencedType = 200 AND refs.AppointmentId = @Id;
	
	-- get firm regarding object id
	IF (@RegardingObjectId IS NULL)
		SELECT @RegardingObjectId = firms.ReplicationCode, @RegardingObjectTypeCode = 1, @RegardingObjectIdName = firms.Name
		FROM [Activity].[AppointmentReferences] refs
			LEFT OUTER JOIN [BusinessDirectory].[Firms] firms ON refs.ReferencedObjectId = firms.Id
		WHERE refs.Reference = 1 AND refs.ReferencedType = 146 AND refs.AppointmentId = @Id;

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

			, [StateCode]						-- (0 - Open, 1 - Completed, 2 - Canceled, 3 - Scheduled)
			, [StatusCode]						-- (Open: 1 - Free, 2 - Tentative; Completed: 3 - Completed; Canceled: 4 - Canceled; Scheduled: 5 - Busy, 6 - Out of Office)

			, [RegardingObjectId]				
			, [RegardingObjectTypeCode]
			, [RegardingObjectIdDsc]
			, [RegardingObjectIdName]
			, [RegardingObjectIdYomiName]

			, [PriorityCode]					
			)
		SELECT
			  [ReplicationCode]
			, 4201								-- appointment
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

			, CASE [Status] WHEN 1 THEN 3 WHEN 2 THEN 1 WHEN 3 THEN 2 ELSE NULL END
			, CASE [Status] WHEN 1 THEN 5 WHEN 2 THEN 3 WHEN 3 THEN 4 ELSE NULL END

			, @RegardingObjectId
			, @RegardingObjectTypeCode
			, 0
			, @RegardingObjectIdName
			, NULL

			, CASE [Priority] WHEN 1 THEN 0 WHEN 2 THEN 1 WHEN 3 THEN 2 ELSE 1 END

	    FROM [Activity].[AppointmentBase]
	    WHERE [Id] = @Id;

		INSERT INTO [DoubleGis_MSCRM].[dbo].[AppointmentBase]
			( [ActivityId], [Location] )
		SELECT
			  @CrmId, [Location]
	    FROM [Activity].[AppointmentBase]
	    WHERE [Id] = @Id;

		INSERT INTO [DoubleGis_MSCRM].[dbo].[AppointmentExtensionBase]
			( [ActivityId]
			, [Dg_purpose]
			, [Dg_result]
			)
		SELECT
			  @CrmId
			, [Purpose]
			, 1
	    FROM [Activity].[AppointmentBase]
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
				, [ActualEnd] = CASE WHEN [ermBase].[Status] = 2 OR [ermBase].[Status] = 3 THEN [ModifiedOn] ELSE NULL END
				, [ActualDurationMinutes] = CASE WHEN [ermBase].[Status] = 2 OR [ermBase].[Status] = 3 THEN DATEDIFF(minute, [ModifiedOn], [CreatedOn]) ELSE NULL END
				, [ScheduledStart] = [ermBase].[ScheduledStart]
				, [ScheduledEnd] = [ermBase].[ScheduledEnd]
				, [ScheduledDurationMinutes] = DATEDIFF(minute, [ermBase].[ScheduledStart], [ermBase].[ScheduledEnd])

				, [OwningBusinessUnit] = @OwnerUserBusinessUnitId
				, [OwningUser] = @OwnerUserId

				, [StateCode]  = CASE [ermBase].[Status] WHEN 1 THEN 3 WHEN 2 THEN 1 WHEN 3 THEN 2 ELSE NULL END
				, [StatusCode] = CASE [ermBase].[Status] WHEN 1 THEN 5 WHEN 2 THEN 3 WHEN 3 THEN 4 ELSE NULL END

				, [RegardingObjectId] = @RegardingObjectId
				, [RegardingObjectTypeCode] = @RegardingObjectTypeCode
				, [RegardingObjectIdDsc] = CASE WHEN (@RegardingObjectId IS NOT NULL) THEN 0 ELSE NULL END
				, [RegardingObjectIdName] = @RegardingObjectIdName

				, [PriorityCode] = CASE [ermBase].[Priority] WHEN 1 THEN 0 WHEN 2 THEN 1 WHEN 3 THEN 2 ELSE 1 END
	    FROM [DoubleGis_MSCRM].[dbo].[ActivityPointerBase] [crmPointer]
		    INNER JOIN [Activity].[AppointmentBase] [ermBase] ON [crmPointer].[ActivityId] = [ermBase].[ReplicationCode] AND [ermBase].[Id] = @Id;

	    UPDATE [crmBase]
		   SET [Location] = [ermBase].[Location]
	    FROM [DoubleGis_MSCRM].[dbo].[AppointmentBase] [crmBase]
		    INNER JOIN [Activity].[AppointmentBase] [ermBase] 
			ON [crmBase].[ActivityId] = [ermBase].[ReplicationCode] AND [ermBase].[Id] = @Id;

	    UPDATE [crmExtension]
		   SET [Dg_purpose] = [ermBase].[Purpose]
	    FROM [DoubleGis_MSCRM].[dbo].[AppointmentExtensionBase] [crmExtension]
		    INNER JOIN [Activity].[AppointmentBase] [ermBase] 
			ON [crmExtension].[ActivityId] = [ermBase].[ReplicationCode] AND [ermBase].[Id] = @Id;

    END;
	
	DELETE FROM [DoubleGis_MSCRM].[dbo].[ActivityPartyBase] WHERE [ActivityId] = @CrmId;
	INSERT INTO [DoubleGis_MSCRM].[dbo].[ActivityPartyBase]
		([ActivityId],[ActivityPartyId],[PartyId],[PartyObjectTypeCode],[ParticipationTypeMask])
	SELECT
		appointments.ReplicationCode,
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
			WHEN 0 THEN 9			-- Owner				(CRM: 9)
			WHEN 1 THEN 8			-- RegardingObject		(ERM: 1, CRM: 8)
			WHEN 2 THEN 7			-- Organizer			(ERM: 2, CRM: 7)
			WHEN 3 THEN 5			-- RequiredAttendees	(ERM: 3, CRM: 5)
			WHEN 4 THEN 6			-- OptionalAttendees	(ERM: 4, CRM: 6)
			END AS [ParticipationTypeMask]
	FROM (
		SELECT 
			[regardingObject].[AppointmentId],
			[regardingObject].[Reference],
			[regardingObject].[ReferencedType],
			[regardingObject].[ReferencedObjectId]
		FROM (
			SELECT TOP 1 
				[AppointmentId],
				[Reference],
				[ReferencedType],
				[ReferencedObjectId],
				CASE ReferencedType 
					WHEN 199 THEN 1 -- a deal is the 1st option to refer to if any
					WHEN 206 THEN 2 -- then a contact 
					WHEN 200 THEN 3 -- then a client
					ELSE 4			-- otherwise a firm
					END as [Order]
			FROM [Activity].[AppointmentReferences]
			WHERE Reference = 1 AND AppointmentId = @Id 
			ORDER BY [Order]
			) [regardingObject]
		UNION ALL -- process other references except regarding object
		SELECT 				
			[AppointmentId],
			[Reference],
			[ReferencedType],
			[ReferencedObjectId]
		FROM [Activity].[AppointmentReferences]
		WHERE Reference != 1 AND AppointmentId = @Id
		UNION ALL -- add owner reference
		SELECT
			[Id] as AppointmentId,
			0 as Reference,
			53 as ReferencedType,		-- Users (ERM: 53)
			[OwnerCode] as ReferencedObjectId
		FROM [Activity].[AppointmentBase]
		WHERE Id = @Id
		) refs
		JOIN [Activity].[AppointmentBase] appointments on refs.AppointmentId = appointments.Id
		LEFT JOIN [Billing].[Clients] clients on refs.ReferencedObjectId = clients.Id and refs.ReferencedType = 200
		LEFT JOIN [Billing].[Contacts] contacts on refs.ReferencedObjectId = contacts.Id and refs.ReferencedType = 206
		LEFT JOIN [Billing].[Deals] deals on refs.ReferencedObjectId = deals.Id and refs.ReferencedType = 199
		LEFT JOIN [BusinessDirectory].[Firms] firms on refs.ReferencedObjectId = firms.Id and refs.ReferencedType = 146
		LEFT JOIN [Security].[Users] users on refs.ReferencedObjectId = users.Id and refs.ReferencedType = 53

    RETURN 1;

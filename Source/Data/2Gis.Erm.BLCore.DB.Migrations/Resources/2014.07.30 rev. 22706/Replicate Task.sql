-- changes
--   rev.1 - initial
CREATE PROCEDURE [Activity].[ReplicateTask]
	@Id bigint = NULL
AS
    IF @Id IS NULL RETURN 0;

    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @CrmId UNIQUEIDENTIFIER;
    DECLARE @CreatedByUserDomainName NVARCHAR(250);
    DECLARE @CreatedByUserId UNIQUEIDENTIFIER;
    DECLARE @ModifiedByUserDomainName NVARCHAR(250);
    DECLARE @ModifiedByUserId UNIQUEIDENTIFIER;
    DECLARE @OwnerUserDomainName NVARCHAR(250);
    DECLARE @OwnerUserId UNIQUEIDENTIFIER;
    DECLARE @OwnerUserBusinessUnitId UNIQUEIDENTIFIER;

    -- get owner user domain name, CRM replication code
    SELECT @CrmId = [T].[ReplicationCode],
            @OwnerUserDomainName = [O].[Account], 
            @CreatedByUserDomainName = [C].[Account], 
            @ModifiedByUserDomainName = [M].[Account]
    FROM [Activity].[TaskBase] AS [T]
	    LEFT OUTER JOIN [Security].[Users] AS [O] ON [O].[Id] = CASE WHEN ([T].[OwnerCode] > 0) THEN [T].[OwnerCode] ELSE [T].[CreatedBy] END
        LEFT OUTER JOIN [Security].[Users] AS [C] ON [C].[Id] = [T].[CreatedBy]
        LEFT OUTER JOIN [Security].[Users] AS [M] ON [M].[Id] = [T].[ModifiedBy]
    WHERE [T].[Id] = @Id;

    -- get owner user CRM UserId
    SELECT 
	    @OwnerUserId = [SystemUserId],
	    @OwnerUserBusinessUnitId = [BusinessUnitId]
    FROM [DoubleGis_MSCRM].[dbo].[SystemUserBase]
    WHERE [DomainName] LIKE N'%\' + @OwnerUserDomainName;

    -- get CreatedBy user CRM UserId
    IF (@CreatedByUserDomainName IS NOT NULL)
	    SELECT @CreatedByUserId = [SystemUserId]
	    FROM [DoubleGis_MSCRM].[dbo].[SystemUserBase]
	    WHERE [DomainName] LIKE N'%\' + @CreatedByUserDomainName;

    -- get ModifiedBy user CRM UserId
    IF (@CreatedByUserDomainName IS NOT NULL)
	    SELECT @ModifiedByUserId = [SystemUserId]
	    FROM [DoubleGis_MSCRM].[dbo].[SystemUserBase]
	    WHERE [DomainName] LIKE N'%\' + @ModifiedByUserDomainName;

    DECLARE @RegardingObjectId UNIQUEIDENTIFIER;
	DECLARE @RegardingObjectTypeCode INT;
    DECLARE @RegardingObjectIdName NVARCHAR(400);

	-- get deal regarding object id
	SELECT @RegardingObjectId = deals.ReplicationCode, @RegardingObjectTypeCode = 3, @RegardingObjectIdName = deals.Name
	FROM [Activity].[TaskReferences] refs
		LEFT OUTER JOIN [Billing].[Deals] deals ON refs.ReferencedObjectId = deals.Id
	WHERE refs.Reference = 1 AND refs.ReferencedType = 199 AND refs.TaskId = @Id;

	-- get contact regarding object id
	IF (@RegardingObjectId IS NULL)
		SELECT @RegardingObjectId = contacts.ReplicationCode, @RegardingObjectTypeCode = 2, @RegardingObjectIdName = contacts.FullName
		FROM [Activity].[TaskReferences] refs
			LEFT OUTER JOIN [Billing].[Contacts] contacts ON refs.ReferencedObjectId = contacts.Id
		WHERE refs.Reference = 1 AND refs.ReferencedType = 206 AND refs.TaskId = @Id;

	-- get client regarding object id
	IF (@RegardingObjectId IS NULL)
		SELECT @RegardingObjectId = clients.ReplicationCode, @RegardingObjectTypeCode = 1, @RegardingObjectIdName = clients.Name
		FROM [Activity].[TaskReferences] refs
			LEFT OUTER JOIN [Billing].[Clients] clients ON refs.ReferencedObjectId = clients.Id
		WHERE refs.Reference = 1 AND refs.ReferencedType = 200 AND refs.TaskId = @Id;
	

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
			, [StatusCode]						-- (Open: 2 - Not Started, 3 - In Progress, 4 - Waiting on someone else, 7 - Deferred; Completed: 5 - Completed; Canceled: 6 - Canceled)

			, [RegardingObjectId]				
			, [RegardingObjectTypeCode]
			, [RegardingObjectIdDsc]
			, [RegardingObjectIdName]
			, [RegardingObjectIdYomiName]

			, [PriorityCode]					
			)
		SELECT
			  [ReplicationCode]
			, 4212								-- task
		    , [CreatedOn]
		    , @CreatedByUserId
		    , [ModifiedOn]
		    , @ModifiedByUserId
			  
			, [Subject]
			, [Description]
			, NULL
			, [ActualEnd]
			, DATEDIFF(minute, [ScheduledStart], [ScheduledEnd]) -- schedule diff is actual duration in CRM
			, [ScheduledStart]					
			, [ScheduledEnd]					
			, NULL

		    , @OwnerUserBusinessUnitId
			, @OwnerUserId

			, CASE WHEN ([Status] = 1) THEN 0 WHEN ([Status] = 2) THEN 1 WHEN ([Status] = 3) THEN 2 ELSE NULL END
			, CASE WHEN ([Status] = 1) THEN 3 WHEN ([Status] = 2) THEN 5 WHEN ([Status] = 3) THEN 6 ELSE NULL END

			, @RegardingObjectId
			, @RegardingObjectTypeCode
			, 0
			, @RegardingObjectIdName
			, NULL

			, CASE WHEN ([Priority] = 1) THEN 0 WHEN ([Priority] = 2) THEN 1 WHEN ([Priority] = 3) THEN 2 ELSE 1 END

	    FROM [Activity].[TaskBase]
	    WHERE [Id] = @Id;

		INSERT INTO [DoubleGis_MSCRM].[dbo].[TaskBase]
			( [ActivityId]
			, [Category]
			, [Subcategory]
			, [PercentComplete]
			, [SubscriptionId]
			, [ImportSequenceNumber]
			, [OverriddenCreatedOn]
			)
		SELECT
			  @CrmId
			, NULL
			, NULL
			, NULL
			, NULL
			, NULL
			, NULL
	    FROM [Activity].[TaskBase]
	    WHERE [Id] = @Id;

		INSERT INTO [DoubleGis_MSCRM].[dbo].[TaskExtensionBase]
			( [ActivityId]
			, [Dg_type]
			)
		SELECT
			  @CrmId
			, [TaskType]
	    FROM [Activity].[TaskBase]
	    WHERE [Id] = @Id;

    END

    -------------------------------------------------------
    -- there is already saved record => update existing one
    -------------------------------------------------------
    ELSE
    BEGIN
		
	    UPDATE [CRMAP]
		    SET 
				  [DeletionStateCode] = CASE WHEN [T].[IsDeleted] = 1 THEN 2 ELSE 0 END
				, [ModifiedBy] = ISNULL(@ModifiedByUserId, @CreatedByUserId)
				, [ModifiedOn] = [T].[ModifiedOn]

				, [Subject]	= [T].[Subject]
				, [Description] = [T].[Description]
				, [ActualStart]	= NULL
				, [ActualEnd] = [T].[ActualEnd]
				, [ActualDurationMinutes] = DATEDIFF(minute, [T].[ScheduledStart], [T].[ScheduledEnd]) -- schedule diff is actual duration in CRM
				, [ScheduledStart] = [T].[ScheduledStart]
				, [ScheduledEnd] = [T].[ScheduledEnd]
				, [ScheduledDurationMinutes] = NULL

				, [OwningBusinessUnit] = @OwnerUserBusinessUnitId
				, [OwningUser] = @OwnerUserId

				, [StateCode] = CASE WHEN ([T].[Status] = 1) THEN 0 WHEN ([T].[Status] = 2) THEN 1 WHEN ([T].[Status] = 3) THEN 2 ELSE NULL END
				, [StatusCode] = CASE WHEN ([T].[Status] = 1) THEN 3 WHEN ([T].[Status] = 2) THEN 5 WHEN ([T].[Status] = 3) THEN 6 ELSE NULL END

				, [RegardingObjectId] = @RegardingObjectId
				, [RegardingObjectTypeCode] = @RegardingObjectTypeCode
				, [RegardingObjectIdDsc] = CASE WHEN (@RegardingObjectId IS NOT NULL) THEN 0 ELSE NULL END
				, [RegardingObjectIdName] = @RegardingObjectIdName

				, [PriorityCode] = CASE WHEN ([Priority] = 1) THEN 0 WHEN ([Priority] = 2) THEN 1 WHEN ([Priority] = 3) THEN 2 ELSE 1 END
	    FROM [DoubleGis_MSCRM].[dbo].[ActivityPointerBase] [CRMAP]
		    INNER JOIN [Activity].[TaskBase] [T] ON [CRMAP].[ActivityId] = [T].[ReplicationCode] AND [T].[Id] = @Id;

	    UPDATE [CRMTEB]
		   SET [Dg_type] = [T].[TaskType]
	    FROM [DoubleGis_MSCRM].[dbo].[TaskExtensionBase] [CRMTEB]
		    INNER JOIN [Activity].[TaskBase] [T] ON [CRMTEB].[ActivityId] = [T].[ReplicationCode] AND [T].[Id] = @Id;

    END;
	
    RETURN 1;

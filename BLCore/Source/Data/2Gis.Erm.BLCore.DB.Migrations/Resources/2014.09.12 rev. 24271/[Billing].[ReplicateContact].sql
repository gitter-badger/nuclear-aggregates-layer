-- changes
--   24.06.2013, a.rechkalov: замена int -> bigint
--   11.09.2014, a.tukaev: выпиливаем like при поиске пользователя по account
ALTER PROCEDURE [Billing].[ReplicateContact]
	@Id bigint = NULL
AS
	SET NOCOUNT ON;
	
	IF @Id IS NULL
		RETURN 0;
		
	SET XACT_ABORT ON;

	DECLARE @CrmId UNIQUEIDENTIFIER;
    DECLARE @CreatedByUserId UNIQUEIDENTIFIER;
    DECLARE @CreatedByUserDomainName NVARCHAR(250);
    DECLARE @ModifiedByUserId UNIQUEIDENTIFIER;
    DECLARE @ModifiedByUserDomainName NVARCHAR(250);
	DECLARE @OwnerUserDomainName NVARCHAR(250);
	DECLARE @OwnerUserId UNIQUEIDENTIFIER;
	DECLARE @OwnerUserBusinessUnitId UNIQUEIDENTIFIER;

	-- get owner user domain name, CRM replication code
	SELECT @CrmId = [TBL].[ReplicationCode],
           @OwnerUserDomainName = [O].[Account], 
           @CreatedByUserDomainName = [C].[Account], 
           @ModifiedByUserDomainName = [M].[Account]
	FROM [Billing].[Contacts] AS [TBL]
		LEFT OUTER JOIN [Security].[Users] AS [O] ON [O].[Id] = CASE WHEN ([TBL].[OwnerCode] > 0) THEN [TBL].[OwnerCode] ELSE [TBL].[CreatedBy] END
        LEFT OUTER JOIN [Security].[Users] AS [C] ON [C].[Id] = [TBL].[CreatedBy]
        LEFT OUTER JOIN [Security].[Users] AS [M] ON [M].[Id] = [TBL].[ModifiedBy]
    WHERE [TBL].[Id] = @Id;

	-- get owner user CRM UserId
	SELECT 
		@OwnerUserId = [SystemUserId],
		@OwnerUserBusinessUnitId = [BusinessUnitId]
	FROM [DoubleGis_MSCRM].[dbo].[SystemUserErmView] WITH (NOEXPAND)
	WHERE [ErmUserAccount] = @OwnerUserDomainName;

    -- get CreatedBy user CRM UserId
    IF (@CreatedByUserDomainName IS NOT NULL)
	    SELECT @CreatedByUserId = [SystemUserId]
	    FROM [DoubleGis_MSCRM].[dbo].[SystemUserErmView] WITH (NOEXPAND)
	    WHERE [ErmUserAccount] = @CreatedByUserDomainName;

    -- get ModifiedBy user CRM UserId
    IF (@CreatedByUserDomainName IS NOT NULL)
	    SELECT @ModifiedByUserId = [SystemUserId]
	    FROM [DoubleGis_MSCRM].[dbo].[SystemUserErmView] WITH (NOEXPAND)
	    WHERE [ErmUserAccount] = @ModifiedByUserDomainName;


	--------------------------------------------------------------------------
	-- there is no such a record in the CRM database => insert a brand new one
	--------------------------------------------------------------------------
	IF NOT EXISTS (SELECT 1 FROM [DoubleGis_MSCRM].[dbo].[ContactBase] WHERE [ContactId] = @CrmId)
	BEGIN

		INSERT INTO [DoubleGis_MSCRM].[dbo].[ContactBase]
           ([CreatedBy],
            [CreatedOn],
            [DeletionStateCode],
            [ContactId],
            [ImportSequenceNumber],
            [ModifiedBy],
            [ModifiedOn],
            [OverriddenCreatedOn],
            [OwningBusinessUnit],
		    [OwningUser],
            [statecode],
            [statuscode],

			[FullName],
			[FirstName],
			[MiddleName],
			[LastName],
			[Salutation],
			[GenderCode],
			[FamilyStatusCode],
			[Telephone1], -- рабочий (основной) телефон
			[Telephone2], -- домашний телефон
			[Telephone3], -- телефон 3
			[MobilePhone],
			[Fax],
			[EMailAddress1], -- рабочая электронная почта
			[EMailAddress2], -- домашняя электронная почта
			[WebSiteUrl],
			[AccountId],
			[JobTitle],
			[AccountRoleCode],
			[Department],
			[Description],
			[BirthDate]
			)

		SELECT
			@CreatedByUserId,
			[Contacts].[CreatedOn],
			CASE WHEN [Contacts].[IsDeleted] = 1 THEN 2 ELSE 0 END,
			[Contacts].[ReplicationCode],
			NULL,
			ISNULL(@ModifiedByUserId, @CreatedByUserId),
			[Contacts].[ModifiedOn],
			NULL,
			@OwnerUserBusinessUnitId,
			@OwnerUserId,
			CASE WHEN [Contacts].[IsActive] = 1 THEN 0 ELSE 1 END,
			CASE WHEN [Contacts].[IsActive] = 1 THEN 1 ELSE 2 END,

			[Contacts].[FullName],
			[Contacts].[FirstName],
			[Contacts].[MiddleName],
			[Contacts].[LastName],
			[Contacts].[Salutation],
			[Contacts].[GenderCode],
			[Contacts].[FamilyStatusCode],
			[Contacts].[MainPhoneNumber],
			[Contacts].[HomePhoneNumber],
			[Contacts].[AdditionalPhoneNumber],
			[Contacts].[MobilePhoneNumber],
			[Contacts].[Fax],
			[Contacts].[WorkEmail],
			[Contacts].[HomeEmail],
			[Contacts].[Website],
			(SELECT [Clients].[ReplicationCode] FROM [Billing].[Clients] WHERE [Clients].Id = [Contacts].[ClientId]),
			[Contacts].[JobTitle],
			[Contacts].[AccountRole],
			[Contacts].[Department],
			[Contacts].[Comment],
			[BirthDate]

		FROM [Billing].[Contacts]
		WHERE [Contacts].[Id] = @Id;
		
		
		INSERT INTO [DoubleGis_MSCRM].[dbo].[ContactExtensionBase]
           ([ContactId],
		    [Dg_workaddress],		-- MS CRM nvarchar limits to 450 symbols max, ERM field has 512
		    [Dg_homeaddress],		-- MS CRM nvarchar limits to 450 symbols max, ERM field has 512
			[Dg_imidentifier],
			[Dg_isfired]
            )
		SELECT
			[ReplicationCode],
			[WorkAddress],
			[HomeAddress],
			[ImIdentifier],
			[IsFired]
		FROM [Billing].[Contacts]
		WHERE [Id] = @Id;

	END

	-------------------------------------------------------
	-- there is already saved record => update existing one
	-------------------------------------------------------
	ELSE
	BEGIN
		
		UPDATE [ContactBase]
			SET
			[DeletionStateCode] = CASE WHEN [Contacts].[IsDeleted] = 1 THEN 2 ELSE 0 END,
			[ModifiedBy] = ISNULL(@ModifiedByUserId, @CreatedByUserId),
			[ModifiedOn] = [Contacts].[ModifiedOn],
			[statecode]  = CASE WHEN [Contacts].[IsActive] = 1 THEN 0 ELSE 1 END,
			[statuscode] = CASE WHEN [Contacts].[IsActive] = 1 THEN 1 ELSE 2 END,
			[OwningBusinessUnit] = @OwnerUserBusinessUnitId,
			[OwningUser] = @OwnerUserId,

			[FullName] = [Contacts].[FullName],
			[FirstName] = [Contacts].[FirstName],
			[MiddleName] = [Contacts].[MiddleName],
			[LastName] = [Contacts].[LastName],
			[Salutation] = [Contacts].[Salutation],
			[GenderCode] = [Contacts].[GenderCode],
			[FamilyStatusCode] = [Contacts].[FamilyStatusCode],
			[Telephone1] = [Contacts].[MainPhoneNumber],
			[Telephone2] = [Contacts].[HomePhoneNumber],
			[Telephone3] = [Contacts].[AdditionalPhoneNumber],
			[MobilePhone] = [Contacts].[MobilePhoneNumber],
			[Fax] = [Contacts].[Fax],
			[EMailAddress1] = [Contacts].[WorkEmail],
			[EMailAddress2] = [Contacts].[HomeEmail],
			[WebSiteUrl] = [Contacts].[Website],
			[AccountId] = (SELECT [Clients].[ReplicationCode] FROM [Billing].[Clients] WHERE [Clients].Id = [Contacts].[ClientId]),
			[JobTitle] = [Contacts].[JobTitle],
			[AccountRoleCode] = [Contacts].[AccountRole],
			[Department] = [Contacts].[Department],
			[Description] = [Contacts].[Comment],
			[BirthDate] = [Contacts].[BirthDate]

		FROM [DoubleGis_MSCRM].[dbo].[ContactBase] AS [ContactBase]
			INNER JOIN [Billing].[Contacts] AS [Contacts] ON [ContactBase].[ContactId] = [Contacts].[ReplicationCode] AND [Contacts].[Id] = @Id;
		
		UPDATE [ContactExtensionBase]
		SET 
			[ContactId] = [Contacts].[ReplicationCode],
			[Dg_workaddress] = [Contacts].[WorkAddress],		-- MS CRM nvarchar limits to 450 symbols max, ERM field has 512
			[Dg_homeaddress] = [Contacts].[HomeAddress],		-- MS CRM nvarchar limits to 450 symbols max, ERM field has 512
			[Dg_imidentifier] = [Contacts].[ImIdentifier],
			[Dg_isfired] = [Contacts].[IsFired]
		FROM [DoubleGis_MSCRM].[dbo].[ContactExtensionBase] AS [ContactExtensionBase]
			INNER JOIN [Billing].[Contacts] AS [Contacts] ON [ContactExtensionBase].[ContactId] = [Contacts].[ReplicationCode] AND [Contacts].[Id] = @Id;
	END;
	
	RETURN 1;



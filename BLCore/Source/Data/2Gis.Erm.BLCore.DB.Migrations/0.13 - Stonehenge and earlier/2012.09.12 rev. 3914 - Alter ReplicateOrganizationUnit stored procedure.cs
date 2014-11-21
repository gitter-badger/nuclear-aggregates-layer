using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(3914, "Изменение хранимой процедуры ReplicateOrganizationUnit - добавлены в репликацию колонки DgppId и ElectronicMedia")]
    public sealed class Migration3914 : TransactedMigration
    {
        #region SP Text

        private const string CreateReplicateOrganizationUnitSPTemplate = @"
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
	DECLARE @OwnerUserOrganizationId UNIQUEIDENTIFIER;

	-- get owner user domain name, CRM replication code
	SELECT @CrmId = [TBL].[ReplicationCode],
           @OwnerUserDomainName = [O].[Account], 
           @CreatedByUserDomainName = [C].[Account], 
           @ModifiedByUserDomainName = [M].[Account]
	FROM [Billing].[OrganizationUnits] AS [TBL]
		LEFT OUTER JOIN [Security].[Users] AS [O] ON [O].[Id] = CASE WHEN ([TBL].[OwnerCode] > 0) THEN [TBL].[OwnerCode] ELSE [TBL].[CreatedBy] END
        LEFT OUTER JOIN [Security].[Users] AS [C] ON [C].[Id] = [TBL].[CreatedBy]
        LEFT OUTER JOIN [Security].[Users] AS [M] ON [M].[Id] = [TBL].[ModifiedBy]
    WHERE [TBL].[Id] = @Id;

	-- get owner user CRM UserId
	SELECT 
		@OwnerUserId = [SystemUserId],
		@OwnerUserOrganizationId = [OrganizationId]
	FROM [{0}].[dbo].[SystemUserBase]
	WHERE [DomainName] LIKE N'%\' + @OwnerUserDomainName;

    -- get CreatedBy user CRM UserId
    IF (@CreatedByUserDomainName IS NOT NULL)
	    SELECT @CreatedByUserId = [SystemUserId]
	    FROM [{0}].[dbo].[SystemUserBase]
	    WHERE [DomainName] LIKE N'%\' + @CreatedByUserDomainName;

    -- get ModifiedBy user CRM UserId
    IF (@CreatedByUserDomainName IS NOT NULL)
	    SELECT @ModifiedByUserId = [SystemUserId]
	    FROM [{0}].[dbo].[SystemUserBase]
	    WHERE [DomainName] LIKE N'%\' + @ModifiedByUserDomainName;


	IF NOT EXISTS (SELECT 1 FROM [{0}].[dbo].[Dg_organizationunitBase] WHERE [Dg_organizationunitId] = @CrmId)
	--------------------------------------------------------------------------
	-- there is no such a record in the CRM database => insert a brand new one
	--------------------------------------------------------------------------
	BEGIN

		INSERT INTO [{0}].[dbo].[Dg_organizationunitBase]
           ([CreatedBy]
           ,[CreatedOn]
           ,[DeletionStateCode]
           ,[Dg_organizationunitId]
           ,[ImportSequenceNumber]
           ,[ModifiedBy]
           ,[ModifiedOn]
           ,[OrganizationId]
           ,[OverriddenCreatedOn]
           ,[statecode]
           ,[statuscode]
           ,[TimeZoneRuleVersionNumber]
           ,[UTCConversionTimeZoneCode])
		SELECT
			@CreatedByUserId,				-- <CreatedBy, uniqueidentifier,>
			[OU].[CreatedOn],			-- <CreatedOn, datetime,>
			CASE WHEN [OU].[IsDeleted] = 1 THEN 2 ELSE 0 END, -- <DeletionStateCode, int,>
			[OU].[ReplicationCode],		-- <Dg_organizationunitId, uniqueidentifier,>
			NULL,						-- <ImportSequenceNumber, int,>
			ISNULL(@ModifiedByUserId, @CreatedByUserId),				-- <ModifiedBy, uniqueidentifier,>
			[OU].[ModifiedOn],			-- <ModifiedOn, datetime,>
			@OwnerUserOrganizationId,	-- <OrganizationId, uniqueidentifier,>
			NULL,						-- <OverriddenCreatedOn, datetime,>
			CASE WHEN [OU].[IsActive] = 1 THEN 0 ELSE 1 END, -- <statecode, int,>
			CASE WHEN [OU].[IsActive] = 1 THEN 1 ELSE 2 END, -- <statuscode, int,>
			NULL,						-- <TimeZoneRuleVersionNumber, int,>
			NULL						-- <UTCConversionTimeZoneCode, int,>
		FROM [Billing].[OrganizationUnits] AS [OU]
		WHERE [OU].[Id] = @Id;
		
		
		INSERT INTO [{0}].[dbo].[Dg_organizationunitExtensionBase]
           ([Dg_name]
           ,[Dg_organizationunitId]
		   ,[Dg_dgppid]
		   ,[Dg_ElectronicMedia])
		SELECT
			 [OU].[Name]				-- <Dg_name, NVARCHAR(100),>
			,[OU].[ReplicationCode]		-- <Dg_organizationunitId, uniqueidentifier,>
			,[OU].[DgppId]				-- <Dg_dgppid, int,>
			,[OU].[ElectronicMedia]		-- <Dg_ElectronicMedia, NVARCHAR(50),>
		FROM [Billing].[OrganizationUnits] AS [OU]
		WHERE [OU].[Id] = @Id;

	END

	-------------------------------------------------------
	-- there is already saved record => update existing one
	-------------------------------------------------------
	ELSE
	BEGIN
		
		UPDATE [CRMOU]
			SET 
			  [DeletionStateCode] = CASE WHEN [OU].[IsDeleted] = 1 THEN 2 ELSE 0 END
			  --,[ImportSequenceNumber] = <ImportSequenceNumber, int,>
			  ,[ModifiedBy] = ISNULL(@ModifiedByUserId, @CreatedByUserId)
			  ,[ModifiedOn] = [OU].[ModifiedOn]
			  --,[OverriddenCreatedOn] = <OverriddenCreatedOn, datetime,>
			  ,[statecode]  = CASE WHEN [OU].[IsActive] = 1 THEN 0 ELSE 1 END
			  ,[statuscode] = CASE WHEN [OU].[IsActive] = 1 THEN 1 ELSE 2 END
			  --,[TimeZoneRuleVersionNumber] = <TimeZoneRuleVersionNumber, int,>
			  --,[UTCConversionTimeZoneCode] = <UTCConversionTimeZoneCode, int,>
		FROM [{0}].[dbo].[Dg_organizationunitBase] AS [CRMOU]
			INNER JOIN [Billing].[OrganizationUnits] AS [OU] ON [CRMOU].[Dg_organizationunitId] = [OU].[ReplicationCode] AND [OU].[Id] = @Id;
		
		UPDATE [CRMOU]
		SET 
		      [Dg_name] = [OU].[Name]
			  ,[Dg_dgppid] = [OU].[DgppId]
			  ,[Dg_ElectronicMedia] = [OU].[ElectronicMedia]
		FROM [{0}].[dbo].[Dg_organizationunitExtensionBase] AS [CRMOU]
			INNER JOIN [Billing].[OrganizationUnits] AS [OU] ON [CRMOU].[Dg_organizationunitId] = [OU].[ReplicationCode] AND [OU].[Id] = @Id;
	END;
	
	RETURN 1;";

        #endregion

        protected override void ApplyOverride(IMigrationContext context)
        {
            if (context.CrmDatabaseName == null)
            {
                return;
            }

            var spTextBody = string.Format(CreateReplicateOrganizationUnitSPTemplate, context.CrmDatabaseName);
            ReplicationHelper.UpdateOrCreateReplicationSP(context, ErmStoredProcedures.ReplicateOrganizationUnit, spTextBody);
        }
    }
}

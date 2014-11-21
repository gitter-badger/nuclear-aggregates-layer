using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._1._3
{
    // 2+: BL\Source\Data\2Gis.Erm.BLCore.DB.Migrations\1.2\2013.11.15 rev. 13385 - Alter client replication .cs
    [Migration(13385, "меняем хранимку репликации клиента", "y.baranihin")]
    public sealed class Migration13385 : TransactedMigration, INonDefaultDatabaseMigration
    {
        private const string AlterStatement = @"-- changes
--   24.06.2013, a.rechkalov: замена int -> bigint
ALTER PROCEDURE [Billing].[ReplicateClient]
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
	FROM [Billing].[Clients] AS [TBL]
		LEFT OUTER JOIN [Security].[Users] AS [O] ON [O].[Id] = CASE WHEN ([TBL].[OwnerCode] > 0) THEN [TBL].[OwnerCode] ELSE [TBL].[CreatedBy] END
        LEFT OUTER JOIN [Security].[Users] AS [C] ON [C].[Id] = [TBL].[CreatedBy]
        LEFT OUTER JOIN [Security].[Users] AS [M] ON [M].[Id] = [TBL].[ModifiedBy]
    WHERE [TBL].[Id] = @Id;

	-- get owner user CRM UserId
	SELECT 
		@OwnerUserId = [SystemUserId],
		@OwnerUserBusinessUnitId = [BusinessUnitId]
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


	IF NOT EXISTS (SELECT 1 FROM [DoubleGis{0}_MSCRM].[dbo].[AccountBase] WHERE [AccountId] = @CrmId)
	--------------------------------------------------------------------------
	-- there is no such a record in the CRM database => insert a brand new one
	--------------------------------------------------------------------------
	BEGIN

		INSERT INTO [{0}].[dbo].[AccountBase]
           ([CreatedBy]
           ,[CreatedOn]
           ,[DeletionStateCode]
           ,[AccountId]
           ,[ImportSequenceNumber]
           ,[ModifiedBy]
           ,[ModifiedOn]
           ,[OverriddenCreatedOn]
           ,[OwningBusinessUnit]
           ,[statecode]
           ,[statuscode]
           ,[TimeZoneRuleVersionNumber]
           ,[UTCConversionTimeZoneCode]
           ,[OwningUser]
		   ,[Name]
		   ,[Telephone1]
		   ,[EMailAddress1])
		SELECT
			@CreatedByUserId,		-- <CreatedBy, uniqueidentifier,>
			[CL].[CreatedOn],	-- <CreatedOn, datetime,>
			CASE WHEN [CL].[IsDeleted] = 1 THEN 2 ELSE 0 END, -- <DeletionStateCode, int,>
			[CL].[ReplicationCode],			-- <AccountId, uniqueidentifier,>
			NULL,				-- <ImportSequenceNumber, int,>
			ISNULL(@ModifiedByUserId, @CreatedByUserId),		-- <ModifiedBy, uniqueidentifier,>
			[CL].[ModifiedOn],	-- <ModifiedOn, datetime,>
			NULL,				-- <OverriddenCreatedOn, datetime,>
			@OwnerUserBusinessUnitId,-- <OwningBusinessUnit, uniqueidentifier,>
			CASE WHEN [CL].[IsActive] = 1 THEN 0 ELSE 1 END, -- <statecode, int,>
			CASE WHEN [CL].[IsActive] = 1 THEN 1 ELSE 2 END, -- <statuscode, int,>
			NULL,				-- <TimeZoneRuleVersionNumber, int,>
			NULL,				-- <UTCConversionTimeZoneCode, int,>
			@OwnerUserId,		-- <OwningUser, uniqueidentifier,>
			[CL].Name,
			[CL].MainPhoneNumber,
			[CL].Email
		FROM [Billing].[Clients] AS [CL]
		WHERE [CL].[Id] = @Id;
		
		
		INSERT INTO [{0}].[dbo].[AccountExtensionBase]
           ( [dg_mainaddress]
		    ,[dg_mainfirm]
			,[dg_lastqualifytime]
			,[dg_lastdisqualifytime]
			,[dg_source]
			,[dg_territory]
			,[accountid]
			,[dg_PromisingScore]
			,[dg_isadvertisingagency]
            )
		SELECT
			 [CL].[MainAddress]
			,(SELECT [FR].ReplicationCode FROM [BusinessDirectory].Firms AS [FR] WHERE [FR].Id = [CL].MainFirmId)
			,[CL].[LastQualifyTime]
			,[CL].[LastDisqualifyTime]
			,[CL].[InformationSource]
			,(SELECT [TR].ReplicationCode FROM [BusinessDirectory].Territories AS [TR] WHERE [TR].Id = [CL].TerritoryId)
			,[CL].[ReplicationCode]		-- <AccountId, uniqueidentifier,>
			,[CL].[PromisingValue]
			,[CL].[IsAdvertisingAgency]
		FROM [Billing].[Clients] AS [CL]
		WHERE [CL].[Id] = @Id;

	END

	-------------------------------------------------------
	-- there is already saved record => update existing one
	-------------------------------------------------------
	ELSE
	BEGIN
		
		UPDATE [CRMCL]
			SET 
			  [DeletionStateCode] = CASE WHEN [CL].[IsDeleted] = 1 THEN 2 ELSE 0 END
			  --,[ImportSequenceNumber] = <ImportSequenceNumber, int,>
			  ,[ModifiedBy] = ISNULL(@ModifiedByUserId, @CreatedByUserId)
			  ,[ModifiedOn] = [CL].[ModifiedOn]
			  --,[OverriddenCreatedOn] = <OverriddenCreatedOn, datetime,>
			  ,[OwningBusinessUnit] = @OwnerUserBusinessUnitId
			  ,[statecode]  = CASE WHEN [CL].[IsActive] = 1 THEN 0 ELSE 1 END
			  ,[statuscode] = CASE WHEN [CL].[IsActive] = 1 THEN 1 ELSE 2 END
			  --,[TimeZoneRuleVersionNumber] = <TimeZoneRuleVersionNumber, int,>
			  --,[UTCConversionTimeZoneCode] = <UTCConversionTimeZoneCode, int,>
			  ,Name = [CL].Name
			  ,[Telephone1] = [CL].MainPhoneNumber
			  ,[EMailAddress1] = [CL].Email
			  ,[OwningUser] = @OwnerUserId
		FROM [{0}].[dbo].[AccountBase] AS [CRMCL]
			INNER JOIN [Billing].[Clients] AS [CL] ON [CRMCL].[AccountId] = [CL].[ReplicationCode] AND [CL].[Id] = @Id;
		
		UPDATE [CRMCL]
		SET 
			 [dg_mainaddress] = [CL].[MainAddress]
			,[dg_mainfirm] = (SELECT [FR].ReplicationCode FROM [BusinessDirectory].Firms AS [FR] WHERE [FR].Id = [CL].MainFirmId)
			,[dg_lastqualifytime] = [CL].[LastQualifyTime]
			,[dg_lastdisqualifytime] = [CL].[LastDisqualifyTime]
			,[dg_source] = [CL].[InformationSource]
			,[dg_territory] = (SELECT [TR].ReplicationCode FROM [BusinessDirectory].Territories AS [TR] WHERE [TR].Id = [CL].TerritoryId)
			,[accountid] = [CL].[ReplicationCode]
			,[dg_PromisingScore] = [CL].[PromisingValue]
			,[dg_isadvertisingagency] = [CL].[IsAdvertisingAgency]
		FROM [{0}].[dbo].[AccountExtensionBase] AS [CRMCL]
			INNER JOIN [Billing].[Clients] AS [CL] ON [CRMCL].[AccountId] = [CL].[ReplicationCode] AND [CL].[Id] = @Id;
	END;
	
	RETURN 1;";

        protected override void ApplyOverride(IMigrationContext context)
        {
            if (context.CrmDatabaseName == null)
            {
                return;
            }

            var query = string.Format(AlterStatement, context.CrmDatabaseName);
            context.Connection.ExecuteNonQuery(query);
        }

        public ErmConnectionStringKey ConnectionStringKey
        {
            get { return ErmConnectionStringKey.Erm; }
        }
    }
}

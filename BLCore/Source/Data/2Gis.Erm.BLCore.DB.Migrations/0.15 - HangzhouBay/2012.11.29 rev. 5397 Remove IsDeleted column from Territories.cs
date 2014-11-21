using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(5397, "Удаляем колонку IsDeleted в таблице Territories")]
    public sealed class Migration5397 : TransactedMigration
    {
        const string IndexName = "IX_Territories_IsDeleted_IsActive_Incl_Id_DgppId_Name_OrgId";
        protected override void ApplyOverride(IMigrationContext context)
        {

            #region Текст запроса

            var alterViewQuery = @"ALTER VIEW [Security].[vwUserTerritoriesOrganizationUnits]
AS
SELECT DISTINCT ROW_NUMBER() OVER (ORDER BY U.id) AS Id, U.Id AS UserId, OU.Id AS OrganizationUnitId, T.Id AS TerritoryId
                            FROM Security.Users U
                            INNER JOIN Security.UserOrganizationUnits UOU ON UOU.UserId = U.Id
                            INNER JOIN Billing.OrganizationUnits OU ON OU.Id = UOU.OrganizationUnitId AND OU.IsDeleted = 0 AND OU.IsActive = 1
                            LEFT JOIN Security.UserTerritories UT ON UT.UserId = U.Id AND UT.IsDeleted = 0
                            LEFT JOIN BusinessDirectory.Territories T ON T.Id = UT.TerritoryId AND T.IsActive = 1
                            WHERE
                            (U.IsDeleted = 0 AND U.IsActive = 1)";

            const string query = @"ALTER PROCEDURE [BusinessDirectory].[ReplicateTerritory]
	@Id INT = NULL
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
	DECLARE @OwnerUserOrganizationId UNIQUEIDENTIFIER;

	-- get owner user domain name, CRM replication code
	SELECT @CrmId = [TBL].[ReplicationCode],
           @OwnerUserDomainName = [O].[Account], 
           @CreatedByUserDomainName = [C].[Account], 
           @ModifiedByUserDomainName = [M].[Account]
	FROM [BusinessDirectory].[Territories] AS [TBL]
		LEFT OUTER JOIN [Security].[Users] AS [O] ON [O].[Id] = CASE WHEN ([TBL].[OwnerCode] > 0) THEN [TBL].[OwnerCode] ELSE [TBL].[CreatedBy] END
        LEFT OUTER JOIN [Security].[Users] AS [C] ON [C].[Id] = [TBL].[CreatedBy]
        LEFT OUTER JOIN [Security].[Users] AS [M] ON [M].[Id] = [TBL].[ModifiedBy]
    WHERE [TBL].[Id] = @Id;
    
	-- get owner user CRM UserId
	SELECT 
		@OwnerUserId = [SystemUserId],
		@OwnerUserOrganizationId = [OrganizationId]
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


	IF NOT EXISTS (SELECT 1 FROM [DoubleGis_MSCRM].[dbo].[Dg_territoryBase] WHERE [Dg_territoryId] = @CrmId)
	--------------------------------------------------------------------------
	-- there is no such a record in the CRM database => insert a brand new one
	--------------------------------------------------------------------------
	BEGIN

		INSERT INTO [DoubleGis_MSCRM].[dbo].[Dg_territoryBase]
           ([CreatedBy]
           ,[CreatedOn]
           ,[DeletionStateCode]
           ,[Dg_territoryId]
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
			[T].[CreatedOn],			-- <CreatedOn, datetime,>
			CASE WHEN [T].[IsActive] = 0 THEN 2 ELSE 0 END, -- <DeletionStateCode, int,>
			[T].[ReplicationCode],		-- <Dg_territoryId, uniqueidentifier,>
			NULL,						-- <ImportSequenceNumber, int,>
			ISNULL(@ModifiedByUserId, @CreatedByUserId),				-- <ModifiedBy, uniqueidentifier,>
			[T].[ModifiedOn],			-- <ModifiedOn, datetime,>
			@OwnerUserOrganizationId,	-- <OrganizationId, uniqueidentifier,>
			NULL,						-- <OverriddenCreatedOn, datetime,>
			CASE WHEN [T].[IsActive] = 1 THEN 0 ELSE 1 END, -- <statecode, int,>
			CASE WHEN [T].[IsActive] = 1 THEN 1 ELSE 2 END, -- <statuscode, int,>				
			NULL,						-- <TimeZoneRuleVersionNumber, int,>
			NULL						-- <UTCConversionTimeZoneCode, int,>
		FROM [BusinessDirectory].[Territories] AS [T]
		WHERE [T].[Id] = @Id;
		
		
		INSERT INTO [DoubleGis_MSCRM].[dbo].[Dg_territoryExtensionBase]
           ([Dg_name]
           ,[Dg_territoryId]
           ,[dg_organizationunit])
		SELECT
			 [T].[Name]
			,[T].[ReplicationCode]
			,(SELECT [OU].[ReplicationCode] FROM [Billing].[OrganizationUnits] AS [OU] WHERE [OU].[Id] = [T].[OrganizationUnitId])
		FROM [BusinessDirectory].[Territories] AS [T]
		WHERE [T].[Id] = @Id;

	END

	-------------------------------------------------------
	-- there is already saved record => update existing one
	-------------------------------------------------------
	ELSE
	BEGIN
		
		UPDATE [CRMT]
			SET 
			  [DeletionStateCode] = CASE WHEN [T].[IsDeleted] = 1 THEN 2 ELSE 0 END
			  --,[ImportSequenceNumber] = <ImportSequenceNumber, int,>
			  ,[ModifiedBy] = ISNULL(@ModifiedByUserId, @CreatedByUserId)
			  ,[ModifiedOn] = [T].[ModifiedOn]
			  --,[OverriddenCreatedOn] = <OverriddenCreatedOn, datetime,>
			  ,[statecode] = CASE WHEN [T].[IsActive] = 1 THEN 0 ELSE 1 END
			  ,[statuscode] = CASE WHEN [T].[IsActive] = 1 THEN 1 ELSE 2 END
			  --,[TimeZoneRuleVersionNumber] = <TimeZoneRuleVersionNumber, int,>
			  --,[UTCConversionTimeZoneCode] = <UTCConversionTimeZoneCode, int,>
		FROM [DoubleGis_MSCRM].[dbo].[Dg_territoryBase] AS [CRMT]
			INNER JOIN [BusinessDirectory].[Territories] AS [T] ON [CRMT].[Dg_territoryId] = [T].[ReplicationCode] AND [T].[Id] = @Id;
		
		
		UPDATE [CRMT]
		   SET 
		       [Dg_name] = [T].[Name]
			  ,[Dg_organizationunit] = (SELECT [OU].[ReplicationCode] FROM [Billing].[OrganizationUnits] AS [OU] WHERE [OU].[Id] = [T].[OrganizationUnitId])
		FROM [DoubleGis_MSCRM].[dbo].[Dg_territoryExtensionBase] AS [CRMT]
			INNER JOIN [BusinessDirectory].[Territories] AS [T] ON [CRMT].[Dg_territoryId] = [T].[ReplicationCode] AND [T].[Id] = @Id;
	END;
	
	RETURN 1;";
            #endregion

            var table = context.Database.Tables["Territories", ErmSchemas.BusinessDirectory];
            var column = table.Columns["IsDeleted"];

            if (column == null)
                return;

            DeleteIndex(table, IndexName);

            context.Connection.ExecuteNonQuery(alterViewQuery);
            context.Connection.ExecuteNonQuery(query);

            context.Database.ExecuteNonQuery("UPDATE BusinessDirectory.Territories SET IsActive = 0 WHERE IsDeleted = 1");
            column.Drop();
        }

        private void DeleteIndex(Table table, string indexName)
        {
            var index = table.Indexes[indexName];
            if (index == null)
            {
                return;
            }
            index.Drop();
        }
    }
}

using System;
using System.Text;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(1422, "Обновление процедуры репликации договора. Предварительно необходимо обновить объект 'dg_bargain' в CRM.")]
// ReSharper disable InconsistentNaming
    public class Migration_1422 : TransactedMigration
// ReSharper restore InconsistentNaming
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var targetSP = ErmStoredProcedures.ReplicateBargain;
            var sp = context.Database.StoredProcedures[targetSP.Name, targetSP.Schema];

            StringBuilder spText = new StringBuilder(ReplicateBargainsText);
            var msCrmDatabaseName = context.CrmDatabaseName;
            if (msCrmDatabaseName == null)
            {
                return;
            }

            spText.Replace("$(DoubleGis_MSCRM)", msCrmDatabaseName);
            sp.TextBody = spText.ToString();
            sp.Alter();
        }

        private const String ReplicateBargainsText =
        @"	SET NOCOUNT ON;
	
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
FROM [Billing].[Bargains] AS [TBL]
	LEFT OUTER JOIN [Security].[Users] AS [O] ON [O].[Id] = CASE WHEN ([TBL].[OwnerCode] > 0) THEN [TBL].[OwnerCode] ELSE [TBL].[CreatedBy] END
    LEFT OUTER JOIN [Security].[Users] AS [C] ON [C].[Id] = [TBL].[CreatedBy]
    LEFT OUTER JOIN [Security].[Users] AS [M] ON [C].[Id] = [TBL].[ModifiedBy]
WHERE [TBL].[Id] = @Id;

-- get owner user CRM UserId
SELECT 
	@OwnerUserId = [SystemUserId],
	@OwnerUserBusinessUnitId = [BusinessUnitId]
FROM [$(DoubleGis_MSCRM)].[dbo].[SystemUserBase]
WHERE [DomainName] LIKE N'%\' + @OwnerUserDomainName;

-- get CreatedBy user CRM UserId
IF (@CreatedByUserDomainName IS NOT NULL)
	SELECT @CreatedByUserId = [SystemUserId]
	FROM [$(DoubleGis_MSCRM)].[dbo].[SystemUserBase]
	WHERE [DomainName] LIKE N'%\' + @CreatedByUserDomainName;

-- get ModifiedBy user CRM UserId
IF (@CreatedByUserDomainName IS NOT NULL)
	SELECT @ModifiedByUserId = [SystemUserId]
	FROM [$(DoubleGis_MSCRM)].[dbo].[SystemUserBase]
	WHERE [DomainName] LIKE N'%\' + @ModifiedByUserDomainName;


IF NOT EXISTS (SELECT 1 FROM [$(DoubleGis_MSCRM)].[dbo].[Dg_bargainBase] WHERE [Dg_bargainId] = @CrmId)
--------------------------------------------------------------------------
-- there is no such a record in the CRM database => insert a brand new one
--------------------------------------------------------------------------
BEGIN

	INSERT INTO [$(DoubleGis_MSCRM)].[dbo].[Dg_bargainBase]
        ([CreatedBy]
        ,[CreatedOn]
        ,[DeletionStateCode]
        ,[Dg_bargainId]
        ,[ImportSequenceNumber]
        ,[ModifiedBy]
        ,[ModifiedOn]
        ,[OverriddenCreatedOn]
        ,[OwningBusinessUnit]
        ,[statecode]
        ,[statuscode]
        ,[TimeZoneRuleVersionNumber]
        ,[UTCConversionTimeZoneCode]
        ,[OwningUser])
	SELECT
		@CreatedByUserId,		-- <CreatedBy, uniqueidentifier,>
		[BG].[CreatedOn],	-- <CreatedOn, datetime,>
		CASE WHEN [BG].[IsDeleted] = 1 THEN 2 ELSE 0 END, -- <DeletionStateCode, int,>
		[BG].[ReplicationCode],			-- <Dg_bargainId, uniqueidentifier,>
		NULL,				-- <ImportSequenceNumber, int,>
		ISNULL(@ModifiedByUserId, @CreatedByUserId),		-- <ModifiedBy, uniqueidentifier,>
		[BG].[ModifiedOn],	-- <ModifiedOn, datetime,>
		NULL,				-- <OverriddenCreatedOn, datetime,>
		@OwnerUserBusinessUnitId,-- <OwningBusinessUnit, uniqueidentifier,>
		CASE WHEN [BG].[IsActive] = 1 THEN 0 ELSE 1 END, -- <statecode, int,>
		CASE WHEN [BG].[IsActive] = 1 THEN 1 ELSE 2 END, -- <statuscode, int,>
		NULL,				-- <TimeZoneRuleVersionNumber, int,>
		NULL,				-- <UTCConversionTimeZoneCode, int,>
		@OwnerUserId		-- <OwningUser, uniqueidentifier,>
	FROM [Billing].[Bargains] AS [BG]
	WHERE [BG].[Id] = @Id;
		
		
	INSERT INTO [$(DoubleGis_MSCRM)].[dbo].[Dg_bargainExtensionBase]
        ([Dg_number]
        ,[Dg_comment]
        ,[Dg_signedon]
        ,[Dg_legalperson]
        ,[Dg_branchoffice]
        ,[Dg_bargainId]
		,[Dg_closedon])
	SELECT
			[BG].[Number]				-- <Dg_name, NVARCHAR(100),>
		,[BG].[Comment]
		,[BG].[SignedOn]
		,(SELECT [LP].[ReplicationCode] FROM [Billing].[LegalPersons] AS [LP] WHERE [LP].[Id] = [BG].[CustomerLegalPersonId])
		,(SELECT [BO].[ReplicationCode] FROM [Billing].[BranchOffices] AS [BO] WHERE [BO].[Id] = [BG].[ExecutorBranchOfficeId])
		,[BG].[ReplicationCode]		-- <Dg_bargainId, uniqueidentifier,>
		,[BG].[ClosedOn]
	FROM [Billing].[Bargains] AS [BG]
	WHERE [BG].[Id] = @Id;

END

-------------------------------------------------------
-- there is already saved record => update existing one
-------------------------------------------------------
ELSE
BEGIN
		
	UPDATE [CRMBG]
		SET 
			[DeletionStateCode] = CASE WHEN [BG].[IsDeleted] = 1 THEN 2 ELSE 0 END
			--,[ImportSequenceNumber] = <ImportSequenceNumber, int,>
			,[ModifiedBy] = ISNULL(@ModifiedByUserId, @CreatedByUserId)
			,[ModifiedOn] = [BG].[ModifiedOn]
			--,[OverriddenCreatedOn] = <OverriddenCreatedOn, datetime,>
			,[statecode]  = CASE WHEN [BG].[IsActive] = 1 THEN 0 ELSE 1 END
			,[statuscode] = CASE WHEN [BG].[IsActive] = 1 THEN 1 ELSE 2 END
			--,[TimeZoneRuleVersionNumber] = <TimeZoneRuleVersionNumber, int,>
			--,[UTCConversionTimeZoneCode] = <UTCConversionTimeZoneCode, int,>
	FROM [$(DoubleGis_MSCRM)].[dbo].[Dg_bargainBase] AS [CRMBG]
		INNER JOIN [Billing].[Bargains] AS [BG] ON [CRMBG].[Dg_bargainId] = [BG].[ReplicationCode] AND [BG].[Id] = @Id;
		
	UPDATE [CRMBG]
	SET 
		    [Dg_number] = [BG].[Number],
		    [Dg_comment] = [BG].[Comment],
		    [Dg_signedon] = [BG].[SignedOn],
		    [Dg_legalperson] = (SELECT [LP].[ReplicationCode] FROM [Billing].[LegalPersons] AS [LP] WHERE [LP].[Id] = [BG].[CustomerLegalPersonId]),
		    [Dg_branchoffice] = (SELECT [BOOU].[ReplicationCode] FROM [Billing].[BranchOffices] AS [BOOU] WHERE [BOOU].[Id] = [BG].[ExecutorBranchOfficeId]),
			[Dg_closedon] = [BG].[ClosedOn]
	FROM [$(DoubleGis_MSCRM)].[dbo].[Dg_bargainExtensionBase] AS [CRMBG]
		INNER JOIN [Billing].[Bargains] AS [BG] ON [CRMBG].[Dg_bargainId] = [BG].[ReplicationCode] AND [BG].[Id] = @Id;
END;
	
RETURN 1;";
    }
}

using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(6993, "Правка функции репликации контактов")]
    public sealed class Migration6993 : TransactedMigration
    {
        #region Текст запроса
        private const string Query = @"

ALTER PROCEDURE [Billing].[ReplicateContacts]
	@Ids [Shared].[Int32IdsTableType] readonly
AS

    
	SET NOCOUNT ON;
	
	IF Not Exists (SELECt * FROM @Ids)
		RETURN 0;
		
	SET XACT_ABORT ON;

	DECLARE @ReferenceInfo Table (
	 CrmId UNIQUEIDENTIFIER NULL,
	 CreatedByUserId UNIQUEIDENTIFIER NULL, 
	 CreatedByUserDomainName NVARCHAR(250) NULL, 
	 ModifiedByUserId UNIQUEIDENTIFIER NULL,
	 ModifiedByUserDomainName NVARCHAR(250) NULL,
	 OwnerUserDomainName NVARCHAR(250) NULL,
	 OwnerUserId UNIQUEIDENTIFIER NULL,
	 OwnerUserBusinessUnitId UNIQUEIDENTIFIER NULL,
	 ContactId int NOT NULL,
	 AlreadyCreated bit)


	INSERT INTO @ReferenceInfo (CrmId, OwnerUserDomainName, CreatedByUserDomainName, ModifiedByUserDomainName, ContactId, AlreadyCreated)
	SELECT [TBL].[ReplicationCode],
           [O].[Account], 
           [C].[Account], 
           [M].[Account],
		   [TBL].[Id],
		   0
	FROM [Billing].[Contacts] AS [TBL]
		LEFT OUTER JOIN [Security].[Users] AS [O] ON [O].[Id] = CASE WHEN ([TBL].[OwnerCode] > 0) THEN [TBL].[OwnerCode] ELSE [TBL].[CreatedBy] END
        LEFT OUTER JOIN [Security].[Users] AS [C] ON [C].[Id] = [TBL].[CreatedBy]
        LEFT OUTER JOIN [Security].[Users] AS [M] ON [M].[Id] = [TBL].[ModifiedBy]
    WHERE [TBL].[Id] in (SELECT Id FROM @Ids);

	-- get owner user CRM UserId
UPDATE @ReferenceInfo
    SET OwnerUserId = SystemUserId,
		OwnerUserBusinessUnitId = BusinessUnitId
    FROM [DoubleGis_MSCRM].[dbo].[SystemUserBase]
    WHERE [DomainName] LIKE N'%\' + OwnerUserDomainName
	COLLATE Cyrillic_General_CI_AI

    UPDATE @ReferenceInfo
    SET CreatedByUserId = SystemUserId
    FROM [DoubleGis_MSCRM].[dbo].[SystemUserBase]
    WHERE CreatedByUserDomainName IS NOT NULL AND [DomainName] LIKE N'%\' + CreatedByUserDomainName
	COLLATE Cyrillic_General_CI_AI

	UPDATE @ReferenceInfo
    SET ModifiedByUserId = SystemUserId
    FROM [DoubleGis_MSCRM].[dbo].[SystemUserBase]
    WHERE ModifiedByUserDomainName IS NOT NULL AND [DomainName] LIKE N'%\' + ModifiedByUserDomainName
	COLLATE Cyrillic_General_CI_AI


	 UPDATE @ReferenceInfo
    SET AlreadyCreated = 1
	FROM
    [DoubleGis_MSCRM].[dbo].[ContactBase] c
	WHERE c.[ContactId] = CrmId
	--------------------------------------------------------------------------
	-- there is no such a record in the CRM database => insert a brand new one
	--------------------------------------------------------------------------

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
			[info].CreatedByUserId,
			[Contacts].[CreatedOn],
			CASE WHEN [Contacts].[IsDeleted] = 1 THEN 2 ELSE 0 END,
			[Contacts].[ReplicationCode],
			NULL,
			ISNULL([info].ModifiedByUserId, [info].CreatedByUserId),
			[Contacts].[ModifiedOn],
			NULL,
			[info].OwnerUserBusinessUnitId,
			[info].OwnerUserId,
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
				inner join @ReferenceInfo info ON [Contacts].Id = info.ContactId 
	WHERE info.AlreadyCreated = 0
		
		
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
		WHERE [Id] in  (SELECT ContactId FROM @ReferenceInfo WHERE AlreadyCreated = 0)


	-------------------------------------------------------
	-- there is already saved record => update existing one
	-------------------------------------------------------
		
		UPDATE [ContactBase]
			SET
			[DeletionStateCode] = CASE WHEN [Contacts].[IsDeleted] = 1 THEN 2 ELSE 0 END,
			[ModifiedBy] = ISNULL([INFO].ModifiedByUserId, [INFO].CreatedByUserId),
			[ModifiedOn] = [Contacts].[ModifiedOn],
			[statecode]  = CASE WHEN [Contacts].[IsActive] = 1 THEN 0 ELSE 1 END,
			[statuscode] = CASE WHEN [Contacts].[IsActive] = 1 THEN 1 ELSE 2 END,
			[OwningBusinessUnit] = [INFO].OwnerUserBusinessUnitId,
			[OwningUser] = [INFO].OwnerUserId,

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
			INNER JOIN [Billing].[Contacts] AS [Contacts] ON [ContactBase].[ContactId] = [Contacts].[ReplicationCode]
			INNER JOIN @ReferenceInfo INFO ON [Contacts].Id = INFO.ContactId AND [INFO].[AlreadyCreated] = 1
		
		UPDATE [ContactExtensionBase]
		SET 
			[ContactId] = [Contacts].[ReplicationCode],
			[Dg_workaddress] = [Contacts].[WorkAddress],		-- MS CRM nvarchar limits to 450 symbols max, ERM field has 512
			[Dg_homeaddress] = [Contacts].[HomeAddress],		-- MS CRM nvarchar limits to 450 symbols max, ERM field has 512
			[Dg_imidentifier] = [Contacts].[ImIdentifier],
			[Dg_isfired] = [Contacts].[IsFired]
		FROM [DoubleGis_MSCRM].[dbo].[ContactExtensionBase] AS [ContactExtensionBase]
			INNER JOIN [Billing].[Contacts] AS [Contacts] ON [ContactExtensionBase].[ContactId] = [Contacts].[ReplicationCode] AND [Contacts].[Id] in (SELECT ContactId FROM @ReferenceInfo WHERE AlreadyCreated = 1);
	
	RETURN 1;

GO


ALTER PROCEDURE [Integration].[ReplicateObjectsAfterImportCards]
AS
BEGIN

SET XACT_ABORT ON;

BEGIN TRY;

-- Удалаяем дубли фирм, нуждающихся в обработке
WITH CTE AS (
	SELECT FirmId, ROW_NUMBER() OVER(PARTITION BY FirmId ORDER BY FirmId, CreatedOn DESC) rnk
	FROM Integration.FirmsForPostIntegrationActivities
	 )
DELETE FROM CTE
WHERE rnk > 1;

--Репликация объектов
BEGIN TRAN
DECLARE @FirmIds Shared.Int32IdsTableType
DECLARE @AddressIds Shared.Int32IdsTableType

INSERT INTO @FirmIds SELECT DISTINCT FirmId FROM Integration.FirmsForPostIntegrationActivities WHERE ReplicateObjects = 0
INSERT INTO @AddressIds SELECT Id FROM BusinessDirectory.FirmAddresses WHERE FirmId IN (SELECT Id FROM @FirmIds)

EXEC BusinessDirectory.ReplicateFirms @FirmIds
EXEC BusinessDirectory.ReplicateFirmAddresses @AddressIds

UPDATE Integration.FirmsForPostIntegrationActivities SET ReplicateObjects = 1
DELETE FROM Integration.FirmsForPostIntegrationActivities WHERE ExportOrders = 1 AND ReplicateObjects = 1
COMMIT TRAN

END TRY
BEGIN CATCH
		ROLLBACK TRAN
END CATCH
END

";

        #endregion

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(Query);
        }
    }
}

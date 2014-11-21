using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._1._0
{
    [Migration(12338, "Актуализируем SortingPosition у контактов фирмы")]
    public class Migration12338 : TransactedMigration
    {
        private const string UpdateStatements = @"
  Declare @AddressIds Shared.Int64IdsTableType
Declare @ContactIds Table
(
	Id bigint not null,
	SortingPosition int not null
)

INSERT INTO @AddressIds SELECT FirmAddressId FROM (

--13. Контакты фирмы [BusinessDirectory].[FirmContacts]
--13.1 (Кол-во контактов у адреса)*(Кол-во контактов у адреса + 1)/2 = ∑ BusinessDirectory.FirmContacts.SortingPosition
--Сортировка единая по всему списку контактов (как IsActive = 0, так и IsActive = 1).
-- Скрипт находит все адреса фирмы, соритирвка контактов которых не совпадает с правилом Последовательности Треугольных чисел:
SELECT 
[FirmAddressId]
,SUM(FC.SortingPosition) as SortingPosition
,count (FirmAddressId) as Count_FirmAddressId
,count (FirmAddressId)*(count (FirmAddressId) + 1)/2     as Count_FA_SP
,SUM(FC.SortingPosition) - count (FirmAddressId)*(count (FirmAddressId) + 1)/2 as Diff_Count_Sort
,max(OU.Id) OU_Id
,max(OU.DgppId) OU_DgppId
,max(OU.Name) OU_Name
,max(FC.CreatedOn) FC_CreatedOn
  FROM [BusinessDirectory].[FirmContacts] FC
  join [BusinessDirectory].[FirmAddresses] FA on FA.Id = FC.FirmAddressId
  join [BusinessDirectory].[Firms] F on F.Id = FA.FirmId
  join Billing.OrganizationUnits ou on ou.Id = F.OrganizationUnitId
group by FirmAddressId
having ABS(SUM(FC.SortingPosition) - count (FirmAddressId)*(count (FirmAddressId) + 1)/2) > 0
and (max(OU.InfoRussiaLaunchDate) > min(FC.CreatedOn) or max(OU.InfoRussiaLaunchDate) is null)) as t

DECLARE @CurrentAddressId bigint
DECLARE @CurrentContactId bigint
DECLARE @CurrentContactPosition int
SET @CurrentAddressId = (SELECT min(Id) FROM @AddressIds)

WHILE @CurrentAddressId IS NOT NULL
BEGIN
	DELETE FROM @ContactIds 
	INSERT INTO @ContactIds SELECT Id, SortingPosition FROM BusinessDirectory.FirmContacts Where FirmAddressId = @CurrentAddressId
	SET @CurrentContactPosition = 1
	SET @CurrentContactId = (SELECT Top 1 Id FROM @ContactIds ORDER BY SortingPosition ASC)
	WHILE @CurrentContactId IS NOT NULL
	BEGIN
		UPDATE BusinessDirectory.FirmContacts SET SortingPosition = @CurrentContactPosition, ModifiedBy = 1, ModifiedOn = GETUTCDATE() WHERE Id = @CurrentContactId
		DELETE FROM @ContactIds WHERE Id = @CurrentContactId
		SET @CurrentContactPosition = @CurrentContactPosition + 1 
		SET @CurrentContactId = (SELECT Top 1 Id FROM @ContactIds ORDER BY SortingPosition ASC)
	END
	SET @CurrentAddressId = (SELECT min(Id) FROM @AddressIds where Id > @CurrentAddressId)
END
";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(UpdateStatements);
        }
    }
}

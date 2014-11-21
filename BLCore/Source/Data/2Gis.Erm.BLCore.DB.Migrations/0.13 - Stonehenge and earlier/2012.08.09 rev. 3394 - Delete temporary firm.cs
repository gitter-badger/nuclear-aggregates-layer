using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(3394, "Удаляем временную фирму")]
    public sealed class Migration3394 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(@"
DECLARE @Ids TABLE (Id INT NOT NULL, DgppId BIGINT NOT NULL)

INSERT INTO @Ids
SELECT FA.Id, FA.DgppId FROM BusinessDirectory.FirmAddresses FA INNER JOIN BusinessDirectory.Firms F ON FA.FirmId = F.Id WHERE F.Name = 'Temporary Firm For Import Purposes'

DELETE FC FROM BusinessDirectory.FirmContacts FC INNER JOIN @Ids Ids ON FC.FirmAddressId = Ids.Id
DELETE CFA FROM BusinessDirectory.CategoryFirmAddresses CFA INNER JOIN @Ids Ids ON CFA.FirmAddressId = Ids.Id
DELETE CR FROM Integration.CardRelations CR INNER JOIN @Ids Ids ON CR.PosCardCode = Ids.DgppId

DELETE FA FROM BusinessDirectory.FirmAddresses FA INNER JOIN @Ids Ids ON FA.Id = Ids.Id

DELETE FROM BusinessDirectory.Firms WHERE Name = 'Temporary Firm For Import Purposes'");
        }
    }
}
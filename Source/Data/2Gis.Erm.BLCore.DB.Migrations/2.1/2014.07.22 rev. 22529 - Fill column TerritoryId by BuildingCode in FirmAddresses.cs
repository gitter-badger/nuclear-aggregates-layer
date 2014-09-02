using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(22529, "Заполнение TerritoryId в FirmAddresses по BuildingCode", "a.tukaev")]
    public class Migration22529 : TransactedMigration
    {
        private const string FillTerritoryId = @"update fa set fa.[TerritoryId] = b.[TerritoryId]
                                                 from [BusinessDirectory].[FirmAddresses] fa
                                                 join [Integration].[Buildings] b on b.[Code] = fa.[BuildingCode] and b.[IsDeleted] = 0";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(FillTerritoryId);
        }
    }
}
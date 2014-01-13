using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl._0._18
{
    [Migration(7463, "Удаляем колонку OwnerCode из таблиц, где она не нужна")]
    public class Migration7463 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var table = context.Database.GetTable(ErmTableNames.FirmAddresses);
            DropOwnerCode(table);

            table = context.Database.GetTable(ErmTableNames.Categories);
            DropOwnerCode(table);

            table = context.Database.GetTable(ErmTableNames.CategoryFirmAddresses);
            DropOwnerCode(table);

            table = context.Database.GetTable(ErmTableNames.Currencies);
            DropOwnerCode(table);

            table = context.Database.GetTable(ErmTableNames.Positions);
            DropOwnerCode(table);

            table = context.Database.GetTable(ErmTableNames.Prices);
            DropOwnerCode(table);

            table = context.Database.GetTable(ErmTableNames.Countries);
            DropOwnerCode(table);

            table = context.Database.GetTable(ErmTableNames.OrganizationUnits);
            DropOwnerCode(table);

            table = context.Database.GetTable(ErmTableNames.BargainTypes);
            DropOwnerCode(table);

            table = context.Database.GetTable(ErmTableNames.ContributionTypes);
            DropOwnerCode(table);

            table = context.Database.GetTable(ErmTableNames.BranchOffices);
            DropOwnerCode(table);

            table = context.Database.GetTable(ErmTableNames.PositionCategories);
            DropOwnerCode(table);

            table = context.Database.GetTable(ErmTableNames.Platforms);
            DropOwnerCode(table);

            table = context.Database.GetTable(ErmTableNames.OperationTypes);
            DropOwnerCode(table);

            table = context.Database.GetTable(ErmTableNames.AdvertisementTemplates);
            DropOwnerCode(table);

            table = context.Database.GetTable(ErmTableNames.AdvertisementElementTemplates);
            DropOwnerCode(table);

            table = context.Database.GetTable(ErmTableNames.AdsTemplatesAdsElementTemplates);
            DropOwnerCode(table);

            table = context.Database.GetTable(ErmTableNames.Territories);
            DropOwnerCode(table);
        }

        private void DropOwnerCode(Table table)
        {
            var column = table.Columns["OwnerCode"];

            if (column == null)
            {
                return;
            }

            column.Drop();
        }
    }
}

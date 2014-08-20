using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.BL.DB.Migrations._2._1
{
    [Migration(23210, "Изменение размера некоторых колонок", "y.baranihin")]
    public class Migration23210 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            AlterBranchOfficeOrganizationTable(context);
            AlterClientTable(context);
            AlterContactTable(context);
            AlterCurrencyTable(context);
            AlterDealTable(context);
            AlterLimitTable(context);
            AlterOrganizationUnitTable(context);
            AlterFirmAddressTable(context);
            AlterCategoryTable(context);
        }

        private void AlterBranchOfficeOrganizationTable(IMigrationContext context)
        {
            const string BranchOfficeOrganizationUnitNameColumn = "ShortLegalName";
            const string BouKppColumn = "Kpp";
            const string IndexName = "IX_BranchOfficeOrganizationUnits_ShortLegalName";

            var branchOfficeOrganizationUnitTable = context.Database.GetTable(ErmTableNames.BranchOfficeOrganizationUnits);
            var index = branchOfficeOrganizationUnitTable.Indexes[IndexName];
            if (index != null)
            {
                index.Drop();
            }

            var bouName = branchOfficeOrganizationUnitTable.Columns[BranchOfficeOrganizationUnitNameColumn];
            bouName.DataType = DataType.NVarChar(100);
            bouName.Alter();

            var bouKpp = branchOfficeOrganizationUnitTable.Columns[BouKppColumn];
            bouKpp.DataType = DataType.NVarChar(15);
            bouKpp.Alter();

            index = new Index(branchOfficeOrganizationUnitTable, IndexName);
            index.IndexedColumns.Add(new IndexedColumn(index, BranchOfficeOrganizationUnitNameColumn));
            index.IsClustered = false;
            index.Create();
        }

        private void AlterClientTable(IMigrationContext context)
        {
            const string ClientMainAddressColumn = "MainAddress";

            var client = context.Database.GetTable(ErmTableNames.Clients);
            var clientAddress = client.Columns[ClientMainAddressColumn];
            clientAddress.DataType = DataType.NVarChar(250);
            clientAddress.Alter();
        }

        private void AlterContactTable(IMigrationContext context)
        {
            const string ContactWorkAddressColumn = "WorkAddress";
            const string ContactHomeAddressColumn = "HomeAddress";

            var contact = context.Database.GetTable(ErmTableNames.Contacts);
            var contactWorkAddress = contact.Columns[ContactWorkAddressColumn];
            contactWorkAddress.DataType = DataType.NVarChar(450);
            contactWorkAddress.Alter();

            var contactHomeAddress = contact.Columns[ContactHomeAddressColumn];
            contactHomeAddress.DataType = DataType.NVarChar(450);
            contactHomeAddress.Alter();
        }

        private void AlterCurrencyTable(IMigrationContext context)
        {
            const string CurrencyNameColumn = "Name";
            const string IndexName = "IX_Currencies_Name";
            
            var currencyTable = context.Database.GetTable(ErmTableNames.Currencies);
            var index = currencyTable.Indexes[IndexName];
            if (index != null)
            {
                index.Drop();
            }

            var currencyName = currencyTable.Columns[CurrencyNameColumn];
            currencyName.DataType = DataType.NVarChar(100);
            currencyName.Alter();

            index = new Index(currencyTable, IndexName);
            index.IndexedColumns.Add(new IndexedColumn(index, CurrencyNameColumn));
            index.IsClustered = false;
            index.Create();
        }

        private void AlterDealTable(IMigrationContext context)
        {
            const string DealCloseReasonOtherColumn = "CloseReasonOther";

            var deal = context.Database.GetTable(ErmTableNames.Deals);
            var dealCloseReasonOther = deal.Columns[DealCloseReasonOtherColumn];
            dealCloseReasonOther.DataType = DataType.NVarChar(256);
            dealCloseReasonOther.Alter();
        }

        private void AlterLimitTable(IMigrationContext context)
        {
            const string LimitCommentColumn = "Comment";

            var limit = context.Database.GetTable(ErmTableNames.Limits);
            var limitComment = limit.Columns[LimitCommentColumn];
            limitComment.DataType = DataType.NVarChar(255);
            limitComment.Alter();
        }

        private void AlterOrganizationUnitTable(IMigrationContext context)
        {
            const string OrganizationUnitNameColumn = "Name";
            const string IndexName = "IX_OrganizationUnits_Name";

            var organizationUnitTable = context.Database.GetTable(ErmTableNames.OrganizationUnits);
            var index = organizationUnitTable.Indexes[IndexName];
            if (index != null)
            {
                index.Drop();
            }

            var organizationUnitName = organizationUnitTable.Columns[OrganizationUnitNameColumn];
            organizationUnitName.DataType = DataType.NVarChar(100);
            organizationUnitName.Alter();

            index = new Index(organizationUnitTable, IndexName);
            index.IndexedColumns.Add(new IndexedColumn(index, OrganizationUnitNameColumn));
            index.IsClustered = false;
            index.Create();
        }

        private void AlterFirmAddressTable(IMigrationContext context)
        {
            const string FirmAddressAddressColumn = "Address";

            var firmAddress = context.Database.GetTable(ErmTableNames.FirmAddresses);
            var firmAddressAddress = firmAddress.Columns[FirmAddressAddressColumn];
            firmAddressAddress.DataType = DataType.NVarChar(4000);
            firmAddressAddress.Alter();
        }

        private void AlterCategoryTable(IMigrationContext context)
        {
            const string CategoryNameColumn = "Name";

            var category = context.Database.GetTable(ErmTableNames.Categories);
            var categoryName = category.Columns[CategoryNameColumn];
            categoryName.DataType = DataType.NVarChar(256);
            categoryName.Alter();
        }
    }
}
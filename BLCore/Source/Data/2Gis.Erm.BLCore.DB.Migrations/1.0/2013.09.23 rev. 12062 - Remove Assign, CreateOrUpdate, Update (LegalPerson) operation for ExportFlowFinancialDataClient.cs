using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._1._0
{
    [Migration(12062, "Выгрузка клиента не должна происходить по факту смены куратора или редактирования юр. лица")]
    public class Migration12062 : TransactedMigration
    {
        private const string RemoveStatementTemplate =
            @"DELETE FROM [Shared].[BusinessOperationServices] WHERE Descriptor = {0} AND Operation = {1} AND Service = {2}";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(string.Format(RemoveStatementTemplate, 942141479, 4, 11)); // LegalPerson, Assign, ExportFlowFinancialDataClient
            context.Database.ExecuteNonQuery(string.Format(RemoveStatementTemplate, 942141479, 23, 11)); // LegalPerson, CreateOrUpdate, ExportFlowFinancialDataClient
            context.Database.ExecuteNonQuery(string.Format(RemoveStatementTemplate, 942141479, 31, 11)); // LegalPerson, Update, ExportFlowFinancialDataClient
        }
    }
}

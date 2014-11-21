using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(20952, "Alter BOS for flowNomenclatures export", "a.tukaev")]
    public class Migration20952 : TransactedMigration
    {
        private const string InsertStatementTemplate = @"
IF NOT EXISTS(SELECT 1 FROM [Shared].[BusinessOperationServices] WHERE Operation = {0} AND Descriptor = {1} AND Service = {2})
INSERT INTO [Shared].[BusinessOperationServices](Operation, Descriptor, Service)
VALUES ({0}, {1}, {2})";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(string.Format(InsertStatementTemplate, 30, -335390759, 15));
            context.Database.ExecuteNonQuery(string.Format(InsertStatementTemplate, 31, -335390759, 15));
            context.Database.ExecuteNonQuery(string.Format(InsertStatementTemplate, 2, -335390759, 15));
            context.Database.ExecuteNonQuery(string.Format(InsertStatementTemplate, 8, -335390759, 15));
            context.Database.ExecuteNonQuery(string.Format(InsertStatementTemplate, 9, -335390759, 15));

            context.Database.ExecuteNonQuery(string.Format(InsertStatementTemplate, 30, -880208161, 16));
            context.Database.ExecuteNonQuery(string.Format(InsertStatementTemplate, 31, -880208161, 16));
            context.Database.ExecuteNonQuery(string.Format(InsertStatementTemplate, 9, -880208161, 16));

            context.Database.ExecuteNonQuery(string.Format(InsertStatementTemplate, 2, -335390759, 16));
            context.Database.ExecuteNonQuery(string.Format(InsertStatementTemplate, 8, -335390759, 16));
            context.Database.ExecuteNonQuery(string.Format(InsertStatementTemplate, 9, -335390759, 16));
        }
    }
}
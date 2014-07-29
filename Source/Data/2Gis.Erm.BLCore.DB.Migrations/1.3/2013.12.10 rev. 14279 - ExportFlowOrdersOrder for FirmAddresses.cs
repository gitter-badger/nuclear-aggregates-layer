﻿using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(14279, "Create, Update, Delete FirmAddress -> ExportFlowOrdersOrder", "a.tukaev")]
    public class Migration14279 : TransactedMigration
    {
        private const string InsertStatementTemplate = @"
IF NOT EXISTS(SELECT 1 FROM [Shared].[BusinessOperationServices] WHERE Descriptor = {0} AND Operation = {1} AND Service = {2})
INSERT INTO [Shared].[BusinessOperationServices](Descriptor, Operation, Service)
VALUES ({0}, {1}, {2})";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(string.Format(InsertStatementTemplate, -1198296840, 9, 5));
            context.Database.ExecuteNonQuery(string.Format(InsertStatementTemplate, -1198296840, 30, 5));
            context.Database.ExecuteNonQuery(string.Format(InsertStatementTemplate, -1198296840, 31, 5));
        }
    }
}
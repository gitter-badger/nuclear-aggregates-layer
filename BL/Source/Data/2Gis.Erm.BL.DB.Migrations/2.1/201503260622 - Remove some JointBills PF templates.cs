using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BL.DB.Migrations._2._1
{
    [Migration(201503260622, "Удаляем ПФ единого счета для ИП и физ. лица", "y.baranihin")]
    public class Migration201503260622 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            const int JointBillBusinessmanTemplate = 41;
            const int JointBillNaturalPersonTemplate = 42;

            const string QueryTemplate = @"
Declare @FilesToDelete Table (Id bigint not null)
Insert into @FilesToDelete 
Select FileId From [Billing].[PrintFormTemplates] Where TemplateCode = {0} and FileId is not null

Delete From [Billing].[PrintFormTemplates] Where TemplateCode = {0}
Delete from [Shared].[FileBinaries] where Id in (Select * From @FilesToDelete)
Delete from [Shared].[Files] where Id in (Select * From @FilesToDelete)";

            context.Connection.ExecuteNonQuery(string.Format(QueryTemplate, JointBillBusinessmanTemplate));
            context.Connection.ExecuteNonQuery(string.Format(QueryTemplate, JointBillNaturalPersonTemplate));
        }
    }
}
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BL.DB.Migrations._2._1
{
    [Migration(201411240536, "Удаляем шаблоны ПФ уведомления о региональном расторжении заказа", "y.baranihin")]
    public class Migration201411240536 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            const int RegionalTerminationNoticeBranch2BranchTemplate = 80;
            const int RegionalTerminationNoticeNotBranch2BranchTemplate = 81;

            const string QueryTemplate = @"Delete From [Billing].[PrintFormTemplates] Where TemplateCode = {0}

                                           Delete from [Shared].[FileBinaries] where Id in 
                                              (Select FileId From [Billing].[PrintFormTemplates] Where TemplateCode = {0})

                                           Delete from [Shared].[Files] where Id in 
                                              (Select FileId From [Billing].[PrintFormTemplates] Where TemplateCode = {0})";

            context.Connection.ExecuteNonQuery(string.Format(QueryTemplate, RegionalTerminationNoticeBranch2BranchTemplate));
            context.Connection.ExecuteNonQuery(string.Format(QueryTemplate, RegionalTerminationNoticeNotBranch2BranchTemplate));
        }
    }
}
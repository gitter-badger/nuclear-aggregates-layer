using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BL.DB.Migrations._2._1
{
    [Migration(22222, "удаляем старые шаблоны печатных форм договора", "y.baranihin")]
    public class Migration22222 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            var obsoleteTemplateCodes = new[] { "10", "11", "12" };
            context.Connection.ExecuteNonQuery(string.Format("delete from [Billing].[PrintFormTemplates] where TemplateCode in ({0})",
                                                             string.Join(", ", obsoleteTemplateCodes)));
        }
    }
}
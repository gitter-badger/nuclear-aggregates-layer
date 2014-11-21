using DoubleGis.Erm.Platform.Migration.Base;

namespace DoubleGis.Erm.Platform.Migration.Sql
{
    public static class ExpressionsExtension
    {
        public static void InsertData(this IMigrationContext context, InsertDataExpression expression)
        {
            string script = expression.GenerateScript();
            context.Database.ExecuteNonQuery(script);
        }
    }
}

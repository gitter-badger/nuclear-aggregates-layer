using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._0._18
{
    [Migration(7635, "Деактивируем файлы у скрытых заказов")]
    public sealed class Migration7635 : TransactedMigration
    {
        #region Текст запроса
        private const string Query = @"UPDATE [Billing].[OrderFiles] 
SET IsActive = 0
FROM
[Billing].[OrderFiles]  files inner join [Billing].[Orders] orders 
ON files.OrderId = orders.Id AND files.IsActive = 1 AND (orders.IsActive = 0 OR orders.IsDeleted = 1)";

        #endregion

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(Query);
        }
    }
}

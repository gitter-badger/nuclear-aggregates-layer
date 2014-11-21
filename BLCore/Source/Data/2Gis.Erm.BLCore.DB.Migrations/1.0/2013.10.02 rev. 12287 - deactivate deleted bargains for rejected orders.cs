using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl._1._0
{
    [Migration(12287, "Договоры для закрытых отказом заказов должны деактивироваться, а не удаляться")]
    public class Migration12287 : TransactedMigration
    {
        private const string UpdateStatements = @"
  Declare @RejectedOrderBargainIds Shared.Int64IdsTableType 

  Insert INTO @RejectedOrderBargainIds SELECT BargainId FROM Billing.Orders Where IsActive = 0 AND BargainId IS NOT NULL

  Update Billing.Bargains Set IsActive = 0, IsDeleted = 0, ModifiedOn = GETUTCDATE(), ModifiedBy = 1 WHERE Id In (Select Id From @RejectedOrderBargainIds) And IsDeleted = 1
";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(UpdateStatements);
        }
    }
}

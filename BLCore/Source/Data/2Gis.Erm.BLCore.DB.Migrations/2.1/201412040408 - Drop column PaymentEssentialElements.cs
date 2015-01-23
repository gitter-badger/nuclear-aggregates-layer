using System;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations
{
    [Migration(201412040408, "Удаление пустой колонки AdditionalPaymentElements", "a.rechkalov")]
    public class Migration201412040408 : TransactedMigration
    {
        private const string ValidationQuery = @"select count(*) from Billing.LegalPersonProfiles where AdditionalPaymentElements is not null";

        protected override void ApplyOverride(IMigrationContext context)
        {
            var notMoved = (int)context.Connection.ExecuteScalar(ValidationQuery);
            if (notMoved > 0)
            {
                var message = "Перемещение AdditionalPaymentElements -> PaymentEssentialElements не выполнено для {0} записей.";
                throw new Exception(string.Format(message, notMoved));
            }

            var table = context.Database.Tables[ErmTableNames.LegalPersonProfiles.Name, ErmTableNames.LegalPersonProfiles.Schema];
            var columnn = table.Columns["AdditionalPaymentElements"];
            if (columnn == null)
            {
                return;
            }

            columnn.Drop();
        }
    }
}
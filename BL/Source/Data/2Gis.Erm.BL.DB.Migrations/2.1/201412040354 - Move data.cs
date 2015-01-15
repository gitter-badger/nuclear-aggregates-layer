using System;

using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BL.DB.Migrations
{
    [Migration(201412040354, "Перенос значений из AdditionalPaymentElements в PaymentEssentialElements", "a.rechkalov")]
    public class Migration201412040354 : TransactedMigration
    {
        private const string MoveCommand = @"update Billing.LegalPersonProfiles "
                                           + "set PaymentEssentialElements = AdditionalPaymentElements, AdditionalPaymentElements = null "
                                           + "where PaymentEssentialElements is null and AdditionalPaymentElements is not null";

        private const string ValidationQuery = @"select count(*) from Billing.LegalPersonProfiles where AdditionalPaymentElements is not null";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(MoveCommand);
            
            var notMoved = (int)context.Connection.ExecuteScalar(ValidationQuery);
            if (notMoved > 0)
            {
                var message = "Перемещение AdditionalPaymentElements -> PaymentEssentialElements не выполнено для {0} записей.";
                throw new Exception(string.Format(message, notMoved));
            }
        }
    }
}
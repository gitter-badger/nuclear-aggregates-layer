
using System;

using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(13433, "Управление записями в BusinessOperationServices (чистка таблицы от операций версии 0.18)")]
    public sealed class Migration13433 : TransactedMigration
    {
        private enum Identity
        {
            WithdrawalIdentity = 32,
            RevertWithdrawalIdentity = 33,
            ImportCardsFromServiceBusIdentity = 14601,
            ImportFirmIdentity = 14602,
            RemoveBargainIdentity = 15105,
            ChangeDealIdentity = 15106,
            RepairOutdatedIdentity = 15107,
            CloseWithDenialIdentity = 15108,
            SetInspectorIdentity = 15109,
            BindBargainToOrderIdentity = 19801,
        }

        private enum Descriptor
        {
            Order = -2081716307,
            WithdrawalInfo = -900885408,
            Bargain = -579290438,
            Firm = 653315513,
        }

        private enum Flow
        {
            OrdersOrder = 5,
            FinancialDataClient = 11,
        }

        protected override void ApplyOverride(IMigrationContext context)
        {
            var cleaner = new Cleaner(context);

            cleaner.DeleteRecord(Identity.ImportFirmIdentity, Descriptor.Firm, Flow.FinancialDataClient);

            cleaner.DeleteRecord(Identity.WithdrawalIdentity, Descriptor.WithdrawalInfo, Flow.OrdersOrder);
            cleaner.DeleteRecord(Identity.RevertWithdrawalIdentity, Descriptor.WithdrawalInfo, Flow.OrdersOrder);
            cleaner.DeleteRecord(Identity.ImportCardsFromServiceBusIdentity, Descriptor.Firm, Flow.OrdersOrder);
            cleaner.DeleteRecord(Identity.RemoveBargainIdentity, Descriptor.Order, Flow.OrdersOrder);
            cleaner.DeleteRecord(Identity.ChangeDealIdentity, Descriptor.Order, Flow.OrdersOrder);
            cleaner.DeleteRecord(Identity.RepairOutdatedIdentity, Descriptor.Order, Flow.OrdersOrder);
            cleaner.DeleteRecord(Identity.CloseWithDenialIdentity, Descriptor.Order, Flow.OrdersOrder);
            cleaner.DeleteRecord(Identity.SetInspectorIdentity, Descriptor.Order, Flow.OrdersOrder);
            cleaner.DeleteRecord(Identity.BindBargainToOrderIdentity, Descriptor.Bargain, Flow.OrdersOrder);
        }

        private class Cleaner
        {
            private const string DeleteTemplate = @"delete from Shared.BusinessOperationServices where Operation = {0} and Descriptor = {1} and Service = {2}";
            private const string SelectTemplate = @"select count(*) from Shared.BusinessOperationServices where Operation = {0} and Descriptor = {1} and Service = {2}";
            private readonly IMigrationContext _context;

            public Cleaner(IMigrationContext context)
            {
                _context = context;
            }

            public void DeleteRecord(Identity identity, Descriptor descriptor, Flow flow)
            {
                // Убеждаемся, что знаем, что делаем (что удаляемая запись есть)
                EnsureExists(identity, descriptor, flow);

                // Убеждаемся, что остаётся запись версии 1.0 (с нулевым дескриптором)
                EnsureExists(identity, 0, flow); 

                // Только после этого удаляем
                var statement = string.Format(DeleteTemplate, (int)identity, (int)descriptor, (int)flow);
                _context.Connection.ExecuteNonQuery(statement);
            }

            private void EnsureExists(Identity identity, Descriptor descriptor, Flow flow)
            {
                var select = string.Format(SelectTemplate, (int)identity, (int)descriptor, (int)flow);
                var count = (int)_context.Connection.ExecuteScalar(select);

                if (count == 1)
                {
                    return;
                }

                var message = string.Format("В таблице Shared.BusinessOperationServices ожидалась запись (Operation = {0}, Descriptor = {1}, Service = {2}), но её там не оказалось",
                                            (int)identity,
                                            (int)descriptor,
                                            (int)flow);

                throw new Exception(message);
            }
        }
    }
}

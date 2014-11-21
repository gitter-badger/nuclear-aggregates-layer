using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    // Миграция имеет позднюю дату, но ранний номер, поскольку должна выполниться до миграции 13433.
    // Та миграция не сможет выполниться на Праге или Лимассоле без этой.
    [Migration(13431, "Корректировка значений Shared.BusinessOperationServices из-за ошибки последовательности миграций", "a.rechkalov")]
    public sealed class Migration13431 : TransactedMigration
    {
        private const int ExportFlowOrdersOrderService = 5;
        private const int OrderDescriptor = -2081716307;
        private const int WithdrawalInfoDescriptor = -900885408;

        // Тут использовальсь старые номера операций
        private static readonly long[] Version18BusinessOperationServicesMigrations =
            {
                8806, 8874, 8984, 9001, 9089, 9124, 9342, 9345, 9360, 9361, 9433, 9861, 9934, 10245, 10302, 10305, 10348, 10723, 10742, 11076
            };

        // А после этой миграции все резко перешли на новые
        private const long BreakingChangesMigration = 10455;

        // Но "что-то пошло не так" и эти миграции выполнились после BreakingChangesMigration, хотя должны бли быть выполнены до неё.
        private static readonly long[] KnownMisappliedMigrations =
            {
                10723, 10742, 11076
            };
        
        // Поэтому при помощи этого запроса мы убедимся, что имеем дело с повреждённой базой 
        private const string SelectInvalidMigrations = "select Version, DateApplied " +
                                                       "from Shared.Migrations " +
                                                       "where Version in ({0}) " +
                                                       "and DateApplied >= (select DateApplied from Shared.Migrations where Version = {1})";

        // И заменим номера операций по следующим правилам:
        private static readonly OperationUpgrade WithdrawalOperation = new OperationUpgrade(14, 32, WithdrawalInfoDescriptor, ExportFlowOrdersOrderService);
        private static readonly OperationUpgrade RevertWithdrawalOperation = new OperationUpgrade(15, 33, WithdrawalInfoDescriptor, ExportFlowOrdersOrderService);
        private static readonly OperationUpgrade RepairOutdatedOperation = new OperationUpgrade(108, 15107, OrderDescriptor, ExportFlowOrdersOrderService);

        protected override void ApplyOverride(IMigrationContext context)
        {
            // Убеждаемся, что имеем дело с базой, на которой миграции выполнились в неправильном порядке.
            // Заодно убеждаемся, что в неправильном порядке выполнены именно те миграции, которые разработчик держит в голове, когда пишет этот код.
            var query = string.Format(SelectInvalidMigrations,
                                      string.Join(", ", Version18BusinessOperationServicesMigrations),
                                      BreakingChangesMigration);

            var misappliedMigrations = new List<long>();
            using (var migrationsReader = context.Connection.ExecuteReader(query))
            {
                while (migrationsReader.Read())
                {
                    misappliedMigrations.Add(migrationsReader.GetInt64(0));
                }
            }

            if (!misappliedMigrations.Any())
            {
                // Имеем дело с базой, на которой не возникла проблема
                return;
            }

            if (misappliedMigrations.Count == KnownMisappliedMigrations.Length && misappliedMigrations.All(l => KnownMisappliedMigrations.Contains(l)))
            {
                // Имеем дело с базой, на которой возникла именно та проблема, которую решает эта миграция
                FixMigrations(context);
                return;
            }

            // Имеем дело с базой, на которой что-то нехорошее случилось, но что - не понятно. Лчше не трогать, чтобы не сделать ещё хуже.
            var message = string.Format("Expected that we have no misapplied migrations or we have {0}. But this base has {1} misapplied migrations.",
                                        string.Join(", ", KnownMisappliedMigrations),
                                        string.Join(", ", misappliedMigrations));
            throw new Exception(message);
        }

        private void FixMigrations(IMigrationContext context)
        {
            // из миграции 10723
            Upgrade(context, WithdrawalOperation);
            Upgrade(context, RevertWithdrawalOperation);

            // из миграции 10742
            Upgrade(context, RepairOutdatedOperation);

            // миграция 11076, к счастью, оказалась не привязанной к ServiceId
        }

        private void Upgrade(IMigrationContext context, OperationUpgrade operation)
        {
            context.Connection.ExecuteNonQuery(operation.ToString());
        }

        private class OperationUpgrade
        {
            private readonly int _old;
            private readonly int _new;
            private readonly int _descriptor;
            private readonly int _service;

            public OperationUpgrade(int oldOperation, int newOperation, int descriptor, int service)
            {
                _old = oldOperation;
                _new = newOperation;
                _descriptor = descriptor;
                _service = service;
            }

            public override string ToString()
            {
                return string.Format("{0}\n{1}\n", FormatDeleteString(), FormatInsertString());
            }

            private string FormatDeleteString()
            {
                return string.Format("DELETE FROM [Shared].[BusinessOperationServices] where Operation = {0} and Descriptor = {1} and Service = {2}", _old, _descriptor, _service);
            }

            private string FormatInsertString()
            {
                return string.Format("INSERT INTO [Shared].[BusinessOperationServices]( [Operation], [Descriptor], [Service]) VALUES({0}, {1}, {2})", _new, _descriptor, _service);
            }
        }
    } 
        
}

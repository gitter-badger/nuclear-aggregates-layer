using System.Collections.Generic;

using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(25530, "Включаем отключенные проверки", "y.baranihin")]
    public class Migration25530 : TransactedMigration
    {
        private readonly Queue<long> _pregeneratedIds = new Queue<long>();

        // Ключем является код проверки, значение - группа
        private readonly Dictionary<int, int> _rulesToTurnOn = new Dictionary<int, int>
                                                                   {
                                                                       { 29, 1 },
                                                                       { 31, 4 },
                                                                       { 34, 1 },
                                                                       { 35, 1 },
                                                                       { 37, 1 },
                                                                       { 38, 1 },
                                                                       { 36, 1 },
                                                                       { 22, 2 }
                                                                   };

        protected override void ApplyOverride(IMigrationContext context)
        {
            _pregeneratedIds.Enqueue(479286177126514120);
            _pregeneratedIds.Enqueue(479286492362209992);
            _pregeneratedIds.Enqueue(479286545319515592);
            _pregeneratedIds.Enqueue(479286589452018376);
            _pregeneratedIds.Enqueue(479286637350986696);
            _pregeneratedIds.Enqueue(479286683664534728);
            _pregeneratedIds.Enqueue(479286724760339656);
            _pregeneratedIds.Enqueue(479286767894574024);

            const string QueryTemplate = @"if not exists(select * from [Billing].[OrderValidationRuleGroupDetails] where RuleCode = {0})
                                              insert into [Billing].[OrderValidationRuleGroupDetails] (RuleCode, OrderValidationGroupId, Id) 
                                                values ({0}, {1}, {2})";

            foreach (var rule in _rulesToTurnOn)
            {
                context.Connection.ExecuteNonQuery(string.Format(QueryTemplate, rule.Key, rule.Value, _pregeneratedIds.Dequeue()));
            }
        }
    }
}
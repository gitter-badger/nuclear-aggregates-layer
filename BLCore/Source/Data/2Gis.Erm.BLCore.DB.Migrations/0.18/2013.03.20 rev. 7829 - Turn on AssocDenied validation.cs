using System;

using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(7829, "Включаем проверку на сапотствующие запрещённые номенклатуры")]
    [Obsolete("Миграция неактуальна")]
    public sealed class Migration7829 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
        }
    }
}
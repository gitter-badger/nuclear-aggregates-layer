using System;

using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    // ReSharper disable InconsistentNaming
    [Migration(1717, "Переименования названий платформ")]
    public sealed class Migration_1717 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            // Refers to Platforms enum
 
            // Independent = 0,
            // Desktop = 1,
            // Mobile = 2,
            // Online = 3,

            var commandText =
                "UPDATE [Billing].[Platforms] SET [Name]=N'Версия для ПК' WHERE [DgppId]=1" + Environment.NewLine +
                "UPDATE [Billing].[Platforms] SET [Name]=N'Мобильная версия' WHERE [DgppId]=2" + Environment.NewLine +
                "UPDATE [Billing].[Platforms] SET [Name]=N'API' WHERE [DgppId]=3" + Environment.NewLine +
                "UPDATE [Billing].[Platforms] SET [Name]=N'Платформонезависимая' WHERE [DgppId]=0";

            context.Connection.ExecuteNonQuery(commandText);
        }
    }
}

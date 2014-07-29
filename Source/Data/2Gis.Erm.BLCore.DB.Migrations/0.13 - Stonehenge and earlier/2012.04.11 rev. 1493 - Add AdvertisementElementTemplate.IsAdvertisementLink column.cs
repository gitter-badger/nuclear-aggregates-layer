using System;

using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;
using DoubleGis.Erm.Platform.Migration.Sql;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(1493, "Добавление в ЭШРМ признака что текст - http-адрес.")]
    public class Migration_1493 : TransactedMigration
	{
        private readonly SchemaQualifiedObjectName _tableName = new SchemaQualifiedObjectName("Billing", "AdvertisementElementTemplates");
        private const String ColumnName = "IsAdvertisementLink";

        protected override void ApplyOverride(IMigrationContext context)
        {
            Table bargainsTable = context.Database.GetTable(_tableName);
            if (bargainsTable.Columns.Contains(ColumnName))
            {
                return;
            }

            var isAdvertisementLinkColumn = new Column(bargainsTable, ColumnName, DataType.Bit)
                                      {
                                          Nullable = false,
                                      };
            isAdvertisementLinkColumn.AddDefaultConstraint("DF_AdvertisementElementTemplates_IsAdvertisementLink").Text = "0";
            isAdvertisementLinkColumn.Create();

        }
	}
}

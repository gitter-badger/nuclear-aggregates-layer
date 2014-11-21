using System;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.Extensions;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(1284, "Создание инфраструктуры миграций.")]
    // ReSharper disable InconsistentNaming
    public class Migration_1284 : TransactedMigration
    // ReSharper restore InconsistentNaming
    {
        private readonly VersionTableMetaData _versionTableMetaData = 
            new VersionTableMetaData(ErmTableNames.Migrations, "Version");

        protected override void RevertOverride(IMigrationContext context)
        {
            throw new NotImplementedException();
        }

        protected override void ApplyOverride(IMigrationContext context)
        {
            if (context.Database.Schemas[_versionTableMetaData.TableName.Schema] == null)
            {
                Schema schema = new Schema(context.Database, _versionTableMetaData.TableName.Schema);
                schema.Create();
            }

            Table versionTable = context.Database.GetTable(_versionTableMetaData.TableName);

            if (versionTable != null)
            {
                versionTable.Drop();
            }

            versionTable = new Table(context.Database, _versionTableMetaData.TableName.Name,
                                     _versionTableMetaData.TableName.Schema);
            Column versionColumn = new Column(versionTable, _versionTableMetaData.ColumnName, DataType.BigInt) { Nullable = false };
            versionTable.Columns.Add(versionColumn);
            versionTable.Columns.Add(new Column(versionTable, "DateApplied", DataType.DateTime2(2)));
            versionTable.Create();
            Index primaryKey = new Index(versionTable, "PK_Version");
            IndexedColumn indexColumn = new IndexedColumn(primaryKey, versionColumn.Name);
            primaryKey.IndexedColumns.Add(indexColumn);
            primaryKey.Create();
        }
    }
}
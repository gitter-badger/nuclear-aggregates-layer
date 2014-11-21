using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.BLCore.DB.Migrations.Shared
{
    /// <summary>
    /// Хелперы для старых миграций, когда трава была зеленая, а айдишники сущностей - интовые.
    /// </summary>
    public static class ErmTableUtilsForOldIntKeys
    {
        public static void CreateAuditableEntityColumns(Table table)
        {
            table.Columns.Add(new Column(table, ErmTableUtils.CommonCreatedByColumnName, DataType.Int) { Nullable = false });
            table.Columns.Add(new Column(table, ErmTableUtils.CommonCreatedOnColumnName, DataType.DateTime2(2)) { Nullable = false });
            table.Columns.Add(new Column(table, ErmTableUtils.CommonModifiedByColumnName, DataType.Int) { Nullable = true });
            table.Columns.Add(new Column(table, ErmTableUtils.CommonModifiedOnColumnName, DataType.DateTime2(2)) { Nullable = true });
        }

        public static void CreateStandartColumns(Table table)
        {
            ErmTableUtils.CreateActiveableEntityColumn(table);
            ErmTableUtils.CreateDeleteableEntityColumn(table);
            CreateAuditableEntityColumns(table);
            ErmTableUtils.CreateTimestampColumn(table);
        }

        public static void CreateSecureEntityStandartColumns(Table table)
        {
            ErmTableUtils.CreateActiveableEntityColumn(table);
            ErmTableUtils.CreateDeleteableEntityColumn(table);
            CreateOwnerCodeColumn(table);
            CreateAuditableEntityColumns(table);
            ErmTableUtils.CreateTimestampColumn(table);
        }

        private static void CreateOwnerCodeColumn(this Table table)
        {
            table.Columns.Add(new Column(table, ErmTableUtils.CommonOwnerCodeColumnName, DataType.Int) { Nullable = false });
        }
    }
}
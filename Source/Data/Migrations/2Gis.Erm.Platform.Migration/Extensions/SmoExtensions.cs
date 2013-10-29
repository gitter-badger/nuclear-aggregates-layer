using DoubleGis.Erm.Platform.Migration.Sql;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.Platform.Migration.Extensions
{
    public static class SmoExtensions
    {
        public static Table CreateTable(this Database database, SchemaQualifiedObjectName tableName)
        {
            return new Table(database, tableName.Name, tableName.Schema);
        }

        public static Table GetTable(this Database database, SchemaQualifiedObjectName tableName)
        {
            return database.Tables[tableName.Name, tableName.Schema];
        }

        public static int? IndexOf(this ColumnCollection collection, string columnName)
        {
            int index = 0;
            foreach (Column column in collection)
            {
                if (string.CompareOrdinal(column.Name, columnName) == 0)
                {
                    return index;
                }

                index++;
            }

            return null;
        }

        public static void SetNonNullableColumns(this Table table, params string[] columnNames)
        {
            foreach (var columnName in columnNames)
            {
                table.Columns[columnName].Nullable = false;
            }
        }
    }
}

using DoubleGis.Erm.Platform.Migration.Sql;

namespace DoubleGis.Erm.Platform.Migration.Base
{
    public class VersionTableMetaData
    {
        public VersionTableMetaData(SchemaQualifiedObjectName tableName, string columnName)
        {
            TableName = tableName;
            ColumnName = columnName;
        }

        public VersionTableMetaData(string schemaName, string tableName, string columnName)
        {
            TableName = new SchemaQualifiedObjectName(schemaName, tableName);
            ColumnName = columnName;
        }

        public SchemaQualifiedObjectName TableName { get; private set;}
        public string ColumnName { get; private set; }
    }
}

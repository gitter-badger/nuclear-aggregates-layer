using System.Collections.Generic;

namespace DoubleGis.Erm.Platform.Migration.Sql
{
    public static class InsertDataExpressionExtension
    {
        public static InsertionDataDefinition Add(this InsertionDataDefinition insertDefinition, string columnName, object value)
        {
            insertDefinition.Add(new KeyValuePair<string, object>(columnName, value));
            return insertDefinition;
        }
    }
}

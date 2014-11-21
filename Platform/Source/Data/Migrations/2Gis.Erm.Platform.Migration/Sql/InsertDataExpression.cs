using System.Collections.Generic;

namespace DoubleGis.Erm.Platform.Migration.Sql
{
    public class InsertDataExpression
    {
        private const string InsertData = "INSERT INTO {0} ({1}) VALUES ({2})";

        private readonly List<InsertionDataDefinition> _rows = new List<InsertionDataDefinition>();
        
        public InsertDataExpression(SchemaQualifiedObjectName tableName)
        {
            TableName = tableName;
        }
        
        public SchemaQualifiedObjectName TableName { get; set; }

        public List<InsertionDataDefinition> Rows
        {
            get { return _rows; }
        }
        
        public string GenerateScript()
        {
            var columnNames = new List<string>();
            var columnValues = new List<string>();
            var insertStrings = new List<string>();

            foreach (InsertionDataDefinition row in Rows)
            {
                var quoter = new GenericQuoter();
                columnNames.Clear();
                columnValues.Clear();
                foreach (KeyValuePair<string, object> item in row)
                {
                    columnNames.Add(quoter.QuoteColumnName(item.Key));
                    columnValues.Add(quoter.QuoteValue(item.Value));
                }

                string columns = string.Join(", ", columnNames.ToArray());
                string values = string.Join(", ", columnValues.ToArray());
                insertStrings.Add(string.Format(InsertData, TableName, columns, values));
            }

            return string.Join("; ", insertStrings.ToArray());
        }
    }
}
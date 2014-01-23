using System;

using DoubleGis.Erm.Qds;

namespace DoubleGis.Erm.Elastic.Nest.Qds
{
    public class FieldValueQuery : IDocsQuery
    {
        public FieldValueQuery(string fieldName, object fieldValue)
        {
            if (fieldName == null)
            {
                throw new ArgumentNullException("fieldName");
            }

            FieldValue = fieldValue;
            FieldName = fieldName;
        }

        public string FieldName { get; private set; }
        public object FieldValue { get; private set; }
    }
}
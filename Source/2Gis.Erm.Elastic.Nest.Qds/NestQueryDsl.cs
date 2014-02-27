using System;

using DoubleGis.Erm.Qds;
using DoubleGis.Erm.Qds.Common.Extensions;

namespace DoubleGis.Erm.Elastic.Nest.Qds
{
    public class NestQueryDsl : IQueryDsl
    {
        public IDocsQuery ByFieldValue(string docFieldName, object value)
        {
            if (docFieldName == null)
            {
                throw new ArgumentNullException("docFieldName");
            }

            return new FieldValueQuery(docFieldName.ToCamelCase(), value);
        }
    }
}
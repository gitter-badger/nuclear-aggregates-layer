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

        public IDocsQuery ByNestedObjectQuery(string nestedObjectName, IDocsQuery nestedQuery)
        {
            if (nestedObjectName == null)
            {
                throw new ArgumentNullException("nestedObjectName");
            }

            if (nestedQuery == null)
            {
                throw new ArgumentNullException("nestedQuery");
            }

            return new NestedObjectQuery(nestedObjectName.ToCamelCase(), nestedQuery);
        }

        public IDocsQuery Or(IDocsQuery leftQuery, IDocsQuery rightQuery)
        {
            return new OrObjectQuery(leftQuery, rightQuery);
        }

        public IDocsQuery FieldInQuery(string textvalue, params string[] terms)
        {
            if (terms == null)
            {
                throw new ArgumentNullException("terms");
            }

            return new FieldInQuery(textvalue.ToCamelCase(), terms);
        }
    }
}
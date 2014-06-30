using System;

using DoubleGis.Erm.Qds;

namespace DoubleGis.Erm.Elastic.Nest.Qds
{
    public class FieldInQuery : IDocsQuery
    {
        public FieldInQuery(string fieldName, string[] terms)
        {
            if (fieldName == null)
            {
                throw new ArgumentNullException("fieldName");
            }
            if (terms == null)
            {
                throw new ArgumentNullException("terms");
            }

            FieldName = fieldName;
            Terms = terms;
        }

        public string FieldName { get; private set; }
        public string[] Terms { get; private set; }
    }
}
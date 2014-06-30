using System;

using DoubleGis.Erm.Qds;

namespace DoubleGis.Erm.Elastic.Nest.Qds
{
    public class NestedObjectQuery : IDocsQuery
    {
        public NestedObjectQuery(string nestedObjectName, IDocsQuery nestedQuery)
        {
            if (nestedObjectName == null)
            {
                throw new ArgumentNullException("nestedObjectName");
            }
            if (nestedQuery == null)
            {
                throw new ArgumentNullException("nestedQuery");
            }

            NestedObjectName = nestedObjectName;
            NestedQuery = nestedQuery;
        }

        public string NestedObjectName { get; private set; }
        public IDocsQuery NestedQuery { get; private set; }
    }
}
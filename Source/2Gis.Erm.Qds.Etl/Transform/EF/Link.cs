using System;

namespace DoubleGis.Erm.Qds.Etl.Transform.EF
{
    public class Link
    {
        public Link(Type partType, IDocsQueryBuilder queryBuilder)
        {
            if (partType == null)
            {
                throw new ArgumentNullException("partType");
            }

            if (queryBuilder == null)
            {
                throw new ArgumentNullException("queryBuilder");
            }

            PartType = partType;
            QueryBuilder = queryBuilder;
        }

        public Type PartType { get; private set; }
        public IDocsQueryBuilder QueryBuilder { get; private set; }
    }
}
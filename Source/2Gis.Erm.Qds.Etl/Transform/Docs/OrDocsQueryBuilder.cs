using System;

namespace DoubleGis.Erm.Qds.Etl.Transform.Docs
{
    public class OrDocsQueryBuilder : IDocsQueryBuilder
    {
        readonly IQueryDsl _queryDsl;
        readonly IDocsQueryBuilder _left;
        readonly IDocsQueryBuilder _right;

        public OrDocsQueryBuilder(IDocsQueryBuilder left, IDocsQueryBuilder right, IQueryDsl queryDsl)
        {
            if (queryDsl == null)
            {
                throw new ArgumentNullException("queryDsl");
            }
            if (left == null)
            {
                throw new ArgumentNullException("left");
            }
            if (right == null)
            {
                throw new ArgumentNullException("right");
            }
            _queryDsl = queryDsl;
            _left = left;
            _right = right;
        }

        public IDocsQuery CreateQuery(object entity)
        {
            return _queryDsl.Or(_left.CreateQuery(entity), _right.CreateQuery(entity));
        }
    }
}
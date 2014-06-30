using System;
using System.Linq.Expressions;

namespace DoubleGis.Erm.Qds.Etl.Transform.Docs
{
    public class NestedDocsQueryBuilder<TDoc> : ExpressionDocsQueryBuilder
    {
        readonly IQueryDsl _queryDsl;
        readonly string _nestedObjectName;
        readonly IDocsQueryBuilder _nestedQuery;

        public NestedDocsQueryBuilder(Expression<Func<TDoc, object>> nestedDocsField, IDocsQueryBuilder nestedQuery, IQueryDsl queryDsl)
        {
            if (nestedDocsField == null)
            {
                throw new ArgumentNullException("nestedDocsField");
            }
            if (nestedQuery == null)
            {
                throw new ArgumentNullException("nestedQuery");
            }
            if (queryDsl == null)
            {
                throw new ArgumentNullException("queryDsl");
            }

            _nestedQuery = nestedQuery;
            _queryDsl = queryDsl;
            _nestedObjectName = GetPropertyName(nestedDocsField);
        }

        public override IDocsQuery CreateQuery(object entity)
        {
            return _queryDsl.ByNestedObjectQuery(_nestedObjectName, _nestedQuery.CreateQuery(entity));
        }
    }
}
using System;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Qds.Etl.Transform.Docs
{
    public class FieldsEqualsDocsQueryBuilder<TDoc, TPart> : ExpressionDocsQueryBuilder where TPart : IEntityKey
    {
        private readonly IQueryDsl _queryDsl;
        private readonly Func<TPart, object> _getEntityFieldValueFunc;
        private readonly string _docFieldName;

        public FieldsEqualsDocsQueryBuilder(Expression<Func<TDoc, object>> docField, Expression<Func<TPart, object>> partField, IQueryDsl queryDsl)
        {
            if (docField == null)
            {
                throw new ArgumentNullException("docField");
            }

            if (partField == null)
            {
                throw new ArgumentNullException("partField");
            }

            if (queryDsl == null)
            {
                throw new ArgumentNullException("queryDsl");
            }

            _queryDsl = queryDsl;
            _getEntityFieldValueFunc = partField.Compile();

            _docFieldName = GetPropertyName(docField);
        }

        public override IDocsQuery CreateQuery(object entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            object value = GetValue(entity);

            return _queryDsl.ByFieldValue(_docFieldName, value);
        }

        object GetValue(object entity)
        {
            return _getEntityFieldValueFunc.Invoke((TPart)entity);
        }
    }
}
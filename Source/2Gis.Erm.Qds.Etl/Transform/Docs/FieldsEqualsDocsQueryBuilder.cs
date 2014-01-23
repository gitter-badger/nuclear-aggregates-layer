using System;
using System.Linq.Expressions;
using System.Reflection;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Qds.Etl.Transform.EF;

namespace DoubleGis.Erm.Qds.Etl.Transform.Docs
{
    // TODO Подумать название типа parent child?
    public class FieldsEqualsDocsQueryBuilder<TDoc, TPart> : IDocsQueryBuilder where TPart : IEntityKey
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

        public IDocsQuery CreateQuery(object entity)
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

        static string GetPropertyName<TProperty>(Expression<Func<TProperty, object>> propertyExpression)
        {
            return GetPropertyName(GetMemberExpression(propertyExpression.Body));
        }

        static string GetPropertyName(MemberExpression memberExpression)
        {
            if (memberExpression == null)
            {
                return null;
            }

            if (memberExpression.Member.MemberType != MemberTypes.Property)
            {
                return null;
            }

            var child = memberExpression.Member.Name;
            var parent = GetPropertyName(GetMemberExpression(memberExpression.Expression));

            if (parent == null)
            {
                return child;
            }
            else
            {
                return parent + "." + child;
            }
        }

        static MemberExpression GetMemberExpression(Expression expression)
        {
            var memberExpression = expression as MemberExpression;

            if (memberExpression != null)
            {
                return memberExpression;
            }

            var unaryExpression = expression as UnaryExpression;


            if (unaryExpression != null)
            {
                memberExpression = (MemberExpression)unaryExpression.Operand;

                if (memberExpression != null)
                {
                    return memberExpression;
                }

            }
            return null;
        }
    }
}
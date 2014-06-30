using System;
using System.Linq.Expressions;
using System.Reflection;

namespace DoubleGis.Erm.Qds.Etl.Transform.Docs
{
    public abstract class ExpressionDocsQueryBuilder : IDocsQueryBuilder
    {
        public abstract IDocsQuery CreateQuery(object entity);

        protected static string GetPropertyName<TProperty>(Expression<Func<TProperty, object>> propertyExpression)
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
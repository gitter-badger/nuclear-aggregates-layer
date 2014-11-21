using System;
using System.Linq.Expressions;

using FastMember;

namespace DoubleGis.Erm.Qds.Operations.Metadata
{
    public static class FastMemberExtensions
    {
        public static TemplatedObjectAccessor<TContainer> BasedOn<TContainer>(this ObjectAccessor target)
        {
            return new TemplatedObjectAccessor<TContainer>(target);
        }

        public class TemplatedObjectAccessor<TContainer>
        {
            private readonly ObjectAccessor _objectAccessor;

            public TemplatedObjectAccessor(ObjectAccessor objectAccessor)
            {
                _objectAccessor = objectAccessor;
            }

            public TProperty Get<TProperty>(Expression<Func<TContainer, TProperty>> getter)
            {
                var memberExpression = GetMemberExpression(getter);
                return (TProperty)_objectAccessor[memberExpression.Member.Name];
            }

            private static MemberExpression GetMemberExpression<TContainer, TProperty>(Expression<Func<TContainer, TProperty>> getter)
            {
                var memberExpression = getter.Body as MemberExpression;
                if (memberExpression == null)
                {
                    throw new ArgumentException("Property call expression must be implemented");
                }

                return memberExpression;
            }
        }
    }
}
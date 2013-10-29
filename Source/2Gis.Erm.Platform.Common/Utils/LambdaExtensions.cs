using System;
using System.Linq.Expressions;
using System.Reflection;

namespace DoubleGis.Erm.Platform.Common.Utils
{
    public static class LambdaExtensions
    {
        public static void SetPropertyValue<TTarget, TContainer, TProperty>(this TTarget target,
                                                                            Expression<Func<TContainer, TProperty>> getter,
                                                                            TProperty value)
        {
            var memberExpression = getter.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new ArgumentException("Property call expression must be implemented");
            }

            var propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null)
            {
                throw new ArgumentException("Property call expression must be implemented");
            }

            var targetRuntimeType = target.GetType();
            var targetPropertyInfo = targetRuntimeType.GetProperty(propertyInfo.Name);
            if (targetPropertyInfo != null)
            {
                var action = CreatePropertySetter<TTarget, TProperty>(targetRuntimeType, targetPropertyInfo);
                action(target, value);
            }
        }

        public static void SetPropertyValue<TTarget, TProperty>(this TTarget target, string propertyName, TProperty value)
        {
            var targetRuntimeType = target.GetType();
            var targetPropertyInfo = targetRuntimeType.GetProperty(propertyName);
            if (targetPropertyInfo != null)
            {
                var action = CreatePropertySetter<TTarget, TProperty>(targetRuntimeType, targetPropertyInfo);
                action(target, value);
            }
        }

        public static TProperty GetPropertyValue<TTarget, TContainer, TProperty>(this TTarget target, Expression<Func<TContainer, TProperty>> getter)
        {
            var memberExpression = getter.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new ArgumentException("Property call expression must be implemented");
            }

            var propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null)
            {
                throw new ArgumentException("Property call expression must be implemented");
            }

            var targetRuntimeType = target.GetType();
            var targetRuntimePropertyInfo = targetRuntimeType.GetProperty(propertyInfo.Name);
            if (targetRuntimePropertyInfo != null)
            {
                var action = CreatePropertyGetter<TTarget, TProperty>(targetRuntimeType, targetRuntimePropertyInfo);
                return action(target);
            }

            return default(TProperty);
        }

        public static TProperty GetPropertyValue<TTarget, TProperty>(this TTarget target, string propertyName)
        {
            var targetRuntimeType = target.GetType();
            var targetRuntimePropertyInfo = targetRuntimeType.GetProperty(propertyName);
            if (targetRuntimePropertyInfo != null)
            {
                var action = CreatePropertyGetter<TTarget, TProperty>(targetRuntimeType, targetRuntimePropertyInfo);
                return action(target);
            }

            return default(TProperty);
        }

        public static object GetPropertyValue(this object target, string propertyName)
        {
            var targetRuntimeType = target.GetType();
            var targetRuntimePropertyInfo = targetRuntimeType.GetProperty(propertyName);
            if (targetRuntimePropertyInfo != null)
            {
                var action = CreatePropertyGetter<object, object>(targetRuntimeType, targetRuntimePropertyInfo);
                return action(target);
            }

            return null;
        }

        private static Action<TTarget, TProperty> CreatePropertySetter<TTarget, TProperty>(Type targetRuntimeType, PropertyInfo propertyInfo)
        {
            var target = Expression.Parameter(typeof(TTarget), "target");
            var value = Expression.Parameter(typeof(TProperty), "value");
            var body = Expression.Assign(Expression.Property(Expression.Convert(target, targetRuntimeType), propertyInfo), value);

            var lambda = Expression.Lambda<Action<TTarget, TProperty>>(body, target, value);
            return lambda.Compile();
        }

        private static Func<TTarget, TProperty> CreatePropertyGetter<TTarget, TProperty>(Type targetRuntimeType, PropertyInfo propertyInfo)
        {
            var target = Expression.Parameter(typeof(TTarget), "target");
            var body = Expression.Convert(Expression.Property(Expression.Convert(target, targetRuntimeType), propertyInfo), typeof(TProperty));

            var lambda = Expression.Lambda<Func<TTarget, TProperty>>(body, target);
            return lambda.Compile();
        }
    }
}
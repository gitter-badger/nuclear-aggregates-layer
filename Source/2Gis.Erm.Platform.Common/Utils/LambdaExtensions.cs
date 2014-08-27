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

        public static void SetStaticPropertyValue<TProperty>(this Type type, string propertyName, TProperty value)
        {
            var targetPropertyInfo = type.GetProperty(propertyName, typeof(TProperty));
            if (targetPropertyInfo != null)
            {
                var action = CreateStaticPropertySetter<TProperty>(targetPropertyInfo);
                action(value);
            }
        }

        public static Action<TProperty> CreateStaticPropertySetter<TProperty>(this Type type, string propertyName)
        {
            var targetPropertyInfo = type.GetProperty(propertyName, typeof(TProperty));
            if (targetPropertyInfo == null)
            {
                throw new ArgumentException(string.Format("Cant't find target property with Name = {0} and Type = {1}", propertyName, typeof(TProperty)));
            }

            return CreateStaticPropertySetter<TProperty>(targetPropertyInfo);
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

        private static Action<TProperty> CreateStaticPropertySetter<TProperty>(PropertyInfo propertyInfo)
        {
            var value = Expression.Parameter(typeof(TProperty), "value");
            var body = Expression.Assign(Expression.Property(null, propertyInfo), value);

            var lambda = Expression.Lambda<Action<TProperty>>(body, value);
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
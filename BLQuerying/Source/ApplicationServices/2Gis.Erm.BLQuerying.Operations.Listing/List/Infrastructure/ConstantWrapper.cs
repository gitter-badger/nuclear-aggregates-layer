using System;
using System.Linq.Expressions;
using System.Reflection;

using DoubleGis.Erm.Platform.Common.Utils;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure
{
    internal sealed class ConstantWrapper<T>
        where T : struct
    {
        private static readonly FieldInfo ValueFieldInfo = GetFieldInfo(x => x._value);
        private static readonly FieldInfo NullableValueFieldInfo = GetFieldInfo(x => x._nullableValue);

        private readonly T _value;
        private readonly T? _nullableValue;

        private ConstantWrapper(T value)
        {
            _value = value;
            _nullableValue = value;
        }

        public static MemberExpression CreateExpression(T value, Type type)
        {
            FieldInfo fieldInfo;
            if (type == typeof(T))
            {
                fieldInfo = ValueFieldInfo;
            }
            else if (type == typeof(T?))
            {
                fieldInfo = NullableValueFieldInfo;
            }
            else
            {
                throw new ArgumentException("type");
            }

            var wrappedValue = new ConstantWrapper<T>(value);
            return Expression.Field(Expression.Constant(wrappedValue), fieldInfo);
        }


        private static FieldInfo GetFieldInfo<TField>(Expression<Func<ConstantWrapper<T>, TField>> expr)
        {
            return (FieldInfo)StaticReflection.GetMemberInfo(expr);
        }
    }
}
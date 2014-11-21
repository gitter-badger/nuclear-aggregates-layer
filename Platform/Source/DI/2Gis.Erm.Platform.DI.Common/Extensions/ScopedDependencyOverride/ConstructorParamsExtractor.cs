using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.ObjectBuilder;

namespace DoubleGis.Erm.Platform.DI.Common.Extensions.ScopedDependencyOverride
{
    /// <summary>
    /// Извлекает InjectionParameterValue (подклассом которого является ResolvedParameter) из private полей переданного в аргументах метода ExtractConstructorParams
    /// экземпляра SpecifiedConstructorSelectorPolicy, для аргумента конструктора с именем, указанным в аргументах метода ExtractConstructorParams.
    /// SpecifiedConstructorSelectorPolicy содержит описание конструктора, который нужно использовать при resolve конкретного типа:
    ///  - constructorinfo - из reflection
    ///  - набор аргументов конструктора заданных при регистрации типа (resolvedparameter и т.д.)
    /// </summary>
    public static class ConstructorParamsExtractor
    {
        private readonly static Type TargetType = typeof(SpecifiedConstructorSelectorPolicy);
        private readonly static Type ParameterValuesFieldTargetType = typeof(InjectionParameterValue[]);
        private readonly static Type ConstructorInfoFieldTargetType = typeof(ConstructorInfo);

        private const string ParameterValuesFieldName = "parameterValues";
        private const string ConstructorInfoFieldName = "ctor";

        public static readonly Lazy<Func<SpecifiedConstructorSelectorPolicy, string, InjectionParameterValue>> ExtractConstructorParams =
            new Lazy<Func<SpecifiedConstructorSelectorPolicy, string, InjectionParameterValue>>(CreateExtractor, LazyThreadSafetyMode.ExecutionAndPublication);
        private static Func<SpecifiedConstructorSelectorPolicy, InjectionParameterValue[]> _parameterValuesFieldAccessor;
        private static Func<SpecifiedConstructorSelectorPolicy, ConstructorInfo> _constructorInfoFieldAccessor;

        static ConstructorParamsExtractor()
        {
            CheckFieldUnityVersionCompatibility(ParameterValuesFieldName, ParameterValuesFieldTargetType);
            CheckFieldUnityVersionCompatibility(ConstructorInfoFieldName, ConstructorInfoFieldTargetType);
            ExtractConstructorParams = new Lazy<Func<SpecifiedConstructorSelectorPolicy, string, InjectionParameterValue>>(CreateExtractor, LazyThreadSafetyMode.ExecutionAndPublication);
        }

        private static Func<SpecifiedConstructorSelectorPolicy, String, InjectionParameterValue> CreateExtractor()
        {
            _constructorInfoFieldAccessor = GetFieldAccessor<SpecifiedConstructorSelectorPolicy, ConstructorInfo>(ConstructorInfoFieldName);
            _parameterValuesFieldAccessor = GetFieldAccessor<SpecifiedConstructorSelectorPolicy, InjectionParameterValue[]>(ParameterValuesFieldName);
            return Extractor;
        }

        private static InjectionParameterValue Extractor(SpecifiedConstructorSelectorPolicy policy, string parameterName)
        {
            var constructorInfo = _constructorInfoFieldAccessor(policy);
            var parameterValues = _parameterValuesFieldAccessor(policy);

            int? index = null;

            var parameters = constructorInfo.GetParameters();
            if (parameters.Length == 0)
            {
                return null;
            }

            if (parameters.Length != parameterValues.Length)
            {
                throw new InvalidOperationException("Count of constructor params not equal to count of constructor injection params");
            }

            for (int i = 0; i < parameters.Length; i++)
            {
                if (string.CompareOrdinal(parameters[i].Name, parameterName) == 0)
                {
                    index = i;
                    break;
                }
            }

            return index.HasValue ? parameterValues[index.Value] : null;
        }

        private static void CheckFieldUnityVersionCompatibility(string fieldName, Type fieldTargetType)
        {
            var field = TargetType.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            if (field == null)
            {
                throw new InvalidOperationException("Type " + TargetType + " does't contains required private field - " + fieldName);
            }

            if (!fieldTargetType.IsAssignableFrom(field.FieldType))
            {
                throw new InvalidOperationException("Type " + TargetType + " has field " + fieldName + " with unsupported field type " + fieldTargetType);
            }
        }

        private static Func<T, TR> GetFieldAccessor<T, TR>(string fieldName)
        {
            ParameterExpression param =
                Expression.Parameter(typeof(T), "arg");

            MemberExpression member =
                Expression.Field(param, fieldName);

            LambdaExpression lambda =
                Expression.Lambda(typeof(Func<T, TR>), member, param);

            var compiled = (Func<T, TR>)lambda.Compile();
            return compiled;
        }
    }
}
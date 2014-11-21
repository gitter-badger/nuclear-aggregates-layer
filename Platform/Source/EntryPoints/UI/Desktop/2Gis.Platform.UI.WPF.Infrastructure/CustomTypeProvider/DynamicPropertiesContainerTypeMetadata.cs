using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.CustomTypeProvider
{
    /// <summary>
    /// Подкласс System.Type, расширяющий метаданые типа
    /// Начначение данного класса расширить метаданные оборачиваемого типа, контейнера динамических свойств
    /// Т.о. если для reflection используется не реальный RuntimeType контейнера динамических свойств, а экземпляр данного класса, 
    /// то в списке свойств контейнера динамических свойств, получаемом через reflection, будут не только реальные ствойства объявленные в класса контейнера динамических свойств (CLR свойства),
    /// но и  + к ним динамические свойства (которых нет в исходном коде), а добавляются они только в runtime.
    /// </summary>
    public sealed class DynamicPropertiesContainerTypeMetadata<TDynamicPropertiesContainerType> : TypeDelegator
        where TDynamicPropertiesContainerType : IDynamicPropertiesContainer
    {
        private readonly IReadOnlyDictionary<string, DynamicPropertyInfo> _containedDynamicProperties;
        private readonly Type _dynamicPropertiesContainerType = typeof(TDynamicPropertiesContainerType);

        internal DynamicPropertiesContainerTypeMetadata(IEnumerable<DynamicPropertyInfo> containedDynamicProperties)
            : base(typeof(TDynamicPropertiesContainerType))
        {
            _containedDynamicProperties = containedDynamicProperties.ToDictionary(info => info.Name);
        }

        public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
        {
            var clrProperties = _dynamicPropertiesContainerType.GetProperties(bindingAttr);
            return clrProperties.Concat(_containedDynamicProperties.Values).ToArray();
        }

        protected override PropertyInfo GetPropertyImpl(string name,
                                                        BindingFlags bindingAttr,
                                                        Binder binder,
                                                        Type returnType,
                                                        Type[] types,
                                                        ParameterModifier[] modifiers)
        {
            DynamicPropertyInfo propertyInfo;
            if (_containedDynamicProperties.TryGetValue(name, out propertyInfo))
            {
                return propertyInfo;
            }

            return GetProperties(bindingAttr).FirstOrDefault(prop => prop.Name == name);
        }
    }
}
using System;
using System.Globalization;
using System.Linq;
using System.Reflection;

using DoubleGis.Platform.UI.WPF.Infrastructure.CustomTypeProvider;

using NuClear.ResourceUtilities;
using NuClear.ResourceUtilities.Legacy;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Localization
{
    // TODO {all, 27.11.2013}: пока обрабатываются только указанные ресурсники + смене целевой культуры происходит повторная обработка - нужно подумать над кэшированием (как внутри экземпляра - найденных ресурсников, так и самих containers)
    // TODO {all, 27.11.2013}: подумать над переходом к DynamicObject в данном конкретном случае, т.к. профит ICustomTypeProvider в его поддержки конкретных типов для динамических свойств
    // Однако, для работы со строковыми ресурсами это асболютно никчему, работа идет в режиме readonly (oneway) binding, и тип свойств и так строки (значения resourceentry для конкретной культуры)
    /// <summary>
    /// Объекты класса используются в качестве прокси-свойств в различных View
    /// Пример: <Label Content="{Binding DynamicResourceDictionary.ResourceId}"/>
    /// </summary>
    public sealed class DynamicResourceDictionary : ICustomTypeProvider, IDynamicPropertiesContainer
    {
        private readonly DynamicPropertiesContainer<DynamicResourceDictionary> _dynamicViewModelPropertiesContainer = new DynamicPropertiesContainer<DynamicResourceDictionary>();
        private readonly Type[] _resourceManagerHostTypes;
        private CultureInfo _culture;

        public DynamicResourceDictionary(CultureInfo targetInitialCulture, params Type[] resourceManagerHostTypeHostTypes)
        {
            _resourceManagerHostTypes = resourceManagerHostTypeHostTypes;
            _culture = targetInitialCulture;

            InitResourceEntriesMap(targetInitialCulture);
        }

        public CultureInfo Culture
        {
            get { return _culture; }
            set
            {
                if (!_culture.Equals(value))
                {
                    UpdateResourceEntriesMap(value);
                    _culture = value;
                }
            }
        }

        public string GetString(string name)
        {
            var resourceEntiresContainer = (IDynamicPropertiesContainer)_dynamicViewModelPropertiesContainer;
            if (!resourceEntiresContainer.ContainsDynamicProperty(name))
            {
                return name;
            }

            return (string)resourceEntiresContainer.GetDynamicPropertyValue(name);
        }

        #region IDynamicPropertiesContainer
        object IDynamicPropertiesContainer.GetDynamicPropertyValue(string propertyName)
        {
            return ((IDynamicPropertiesContainer)_dynamicViewModelPropertiesContainer).GetDynamicPropertyValue(propertyName);
        }

        void IDynamicPropertiesContainer.SetDynamicPropertyValue(string propertyName, object value)
        {
            ((IDynamicPropertiesContainer)_dynamicViewModelPropertiesContainer).SetDynamicPropertyValue(propertyName, value);
        }

        bool IDynamicPropertiesContainer.ContainsDynamicProperty(string propertyName)
        {
            return ((IDynamicPropertiesContainer)_dynamicViewModelPropertiesContainer).ContainsDynamicProperty(propertyName);
        }

        bool IDynamicPropertiesContainer.TryGetDynamicPropertyInfo(string propertyName, out PropertyInfo propertyInfo)
        {
            return ((IDynamicPropertiesContainer)_dynamicViewModelPropertiesContainer).TryGetDynamicPropertyInfo(propertyName, out propertyInfo);
        }

        PropertyInfo[] IDynamicPropertiesContainer.GetAllProperties()
        {
            return ((IDynamicPropertiesContainer)_dynamicViewModelPropertiesContainer).GetAllProperties();
        }

        PropertyInfo[] IDynamicPropertiesContainer.GetDynamicProperties()
        {
            return ((IDynamicPropertiesContainer)_dynamicViewModelPropertiesContainer).GetDynamicProperties();
        }
        #endregion

        #region ICustomTypeProvider
        Type ICustomTypeProvider.GetCustomType()
        {
            return ((ICustomTypeProvider)_dynamicViewModelPropertiesContainer).GetCustomType();
        }
        #endregion

        private void InitResourceEntriesMap(CultureInfo targetCultureInfo)
        {
            // TODO {all, 27.11.2013}: если будет переработана схема хранения использования/информации на кэширующую и т.п. - нужно будет отказаться от проверок на уникальность ресурсов в данном типе - тогда ими должны будут заниматься только на этапе конфигурирования приложения
            var configurator = (IDynamicPropertiesContainerConfigurator)_dynamicViewModelPropertiesContainer;
            var availableResources = _resourceManagerHostTypes.EvaluateAvailableResources(true, targetCultureInfo);

            string report;
            if (availableResources.TryGetDuplicatedResourceEntry(out report))
            {
                throw new InvalidOperationException("Resources usage conventions violated. Duplicated resource entries detected. " + report);
            }

            foreach (var entryInfo in availableResources)
            {
                configurator.AddProperty(entryInfo.Key, typeof(string), entryInfo.Value.Entries.Single().Value.ValuesMap[targetCultureInfo], Enumerable.Empty<Attribute>());
            }

            configurator.Lock();
        }

        private void UpdateResourceEntriesMap(CultureInfo targetCultureInfo)
        {
            var availableResources = _resourceManagerHostTypes.EvaluateAvailableResources(true, targetCultureInfo);
            var entriesContainer = (IDynamicPropertiesContainer)_dynamicViewModelPropertiesContainer;
            foreach (var entryInfo in availableResources)
            {
                entriesContainer.SetDynamicPropertyValue(entryInfo.Key, entryInfo.Value.Entries.Single().Value.ValuesMap[targetCultureInfo]);
            }
        }
    }
}

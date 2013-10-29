using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;

using DoubleGis.Platform.UI.WPF.Infrastructure.CustomTypeProvider;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Localization
{
    /// <summary>
    /// Объекты класса используются в качестве прокси-свойств в различных View
    /// Пример: <Label Content="{Binding DynamicResourceDictionary.ResourceId}"/>
    /// </summary>
    public sealed class DynamicResourceDictionary : ICustomTypeProvider, IDynamicPropertiesContainer
    {
        private readonly DynamicPropertiesContainer<DynamicResourceDictionary> _dynamicViewModelPropertiesContainer = new DynamicPropertiesContainer<DynamicResourceDictionary>();
        private readonly ResourceManager[] _resourceManagers;
        private CultureInfo _culture;

        public DynamicResourceDictionary(CultureInfo targetInitialCulture, params ResourceManager[] resourceManagers)
        {
            _resourceManagers = resourceManagers;
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
            var configurator = (IDynamicPropertiesContainerConfigurator)_dynamicViewModelPropertiesContainer;
            var allResourceEntries = GetResourceEntries(_resourceManagers, targetCultureInfo);

            string duplicatesDesciption;
            if (TryGetDuplicatedResourceEntry(allResourceEntries, out duplicatesDesciption))
            {
                throw new InvalidOperationException("Duplicated resource entries detected. " + duplicatesDesciption);
            }

            foreach (var dictionaryEntry in allResourceEntries)
            {
                configurator.AddProperty((string)dictionaryEntry.Key, typeof(string), dictionaryEntry.Value.Single().Item2.Value, Enumerable.Empty<Attribute>());
            }

            configurator.Lock();
        }

        private void UpdateResourceEntriesMap(CultureInfo targetCultureInfo)
        {
            var allResourceEntries = GetResourceEntries(_resourceManagers, targetCultureInfo);
            var entriesContainer = (IDynamicPropertiesContainer)_dynamicViewModelPropertiesContainer;
            foreach (var dictionaryEntry in allResourceEntries)
            {
                entriesContainer.SetDynamicPropertyValue((string)dictionaryEntry.Key, dictionaryEntry.Value.Single().Item2.Value);
            }
        }

        private static IReadOnlyDictionary<object, List<Tuple<ResourceManager, DictionaryEntry>>> GetResourceEntries(IEnumerable<ResourceManager> resourceManagers, CultureInfo culture)
        {
            var resourceEntriesRegistry = new Dictionary<object, List<Tuple<ResourceManager, DictionaryEntry>>>();
            foreach (var resourceManager in resourceManagers)
            {
                foreach (var entry in resourceManager.GetResourceSet(culture, true, true).Cast<DictionaryEntry>())
                {
                    List<Tuple<ResourceManager, DictionaryEntry>> entries;
                    if (!resourceEntriesRegistry.TryGetValue(entry.Key, out entries))
                    {
                        entries = new List<Tuple<ResourceManager, DictionaryEntry>>();
                        resourceEntriesRegistry.Add(entry.Key, entries);
                    }

                    entries.Add(new Tuple<ResourceManager, DictionaryEntry>(resourceManager, entry));
                }
            }

            return resourceEntriesRegistry;
        }

        private static bool TryGetDuplicatedResourceEntry(IEnumerable<KeyValuePair<object, List<Tuple<ResourceManager, DictionaryEntry>>>> entriesregistry, out string duplicatesDescription)
        {
            StringBuilder sb = null;
            foreach (var entry in entriesregistry.Where(e => e.Value.Count > 1))
            {
                sb = sb ?? new StringBuilder();
                sb.AppendFormat("ResourceEntry:{0} is duplicated in several resources: {1}\n", entry.Key,
                                string.Join(";", entry.Value.Select(b => b.Item1.BaseName)));
            }

            duplicatesDescription = sb != null ? sb.ToString() : null;
            return duplicatesDescription != null;
        }
    }
}

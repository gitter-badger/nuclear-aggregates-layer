using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Threading;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.MetaData
{
    public class LocalizedMetaDataProvider : DataAnnotationsModelMetadataProvider
    {
        private readonly Type[] _localizableViewModelIndicators =
            {
                typeof(IViewModel)
            };
        private readonly IUserContextProvider _userContextProvider;
        private readonly IEnumerable<ResourceManager> _resourceManagers;

        private readonly ConcurrentDictionary<Type, TypePropertiesMetadata> _typePropertiesMetadataMap =
            new ConcurrentDictionary<Type, TypePropertiesMetadata>();

        public LocalizedMetaDataProvider(IUserContextProvider userContextProvider, IEnumerable<ResourceManager> resourceManagers)
        {
            _userContextProvider = userContextProvider;
            _resourceManagers = resourceManagers;
        }

        private CultureInfo GetTargetCulture()
        {
            var userContext = _userContextProvider.Current;
            if (userContext == null || userContext.Profile == null || userContext.Profile.UserLocaleInfo.UserCultureInfo == null)
            {
                var currentThread = Thread.CurrentThread;
                return currentThread.CurrentCulture;
            }

            return userContext.Profile.UserLocaleInfo.UserCultureInfo;
        }

        private bool TryGetLocalizedDisplayName(Type targetType, string targetTypePropertyName, out string localizedMetadata)
        {
            localizedMetadata = null;

            var targetCulture = GetTargetCulture();

            // пытаемся получить закэшированное значение
            TypePropertiesMetadata typePropertiesLocalizedMetadata;
            PropertyCultureSpecificMetadata propertyCultureSpecificMetadata;
            if (_typePropertiesMetadataMap.TryGetValue(targetType, out typePropertiesLocalizedMetadata))
            {
                if (typePropertiesLocalizedMetadata == null)
                {   // уже пытались обрабатывать данный целевой тип, набор ресурсов найти, не удалось => дальнейшая обработка смысла не имеет
                    return false;
                }

                if (typePropertiesLocalizedMetadata.Metadata.TryGetValue(targetTypePropertyName, out propertyCultureSpecificMetadata))
                {
                    string cachedMetadata;
                    if (propertyCultureSpecificMetadata.Metadata.TryGetValue(targetCulture.LCID, out cachedMetadata))
                    {
                        localizedMetadata = cachedMetadata;
                        return localizedMetadata != null;
                    }
                }
                else
                {
                    propertyCultureSpecificMetadata = new PropertyCultureSpecificMetadata();
                    typePropertiesLocalizedMetadata.Metadata.TryAdd(targetTypePropertyName, propertyCultureSpecificMetadata);
                }
            }
            else
            {
                typePropertiesLocalizedMetadata = new TypePropertiesMetadata();
                propertyCultureSpecificMetadata = new PropertyCultureSpecificMetadata();
                typePropertiesLocalizedMetadata.Metadata.TryAdd(targetTypePropertyName, propertyCultureSpecificMetadata);
                _typePropertiesMetadataMap.TryAdd(targetType, typePropertiesLocalizedMetadata);
            }

            // Если на интересующем нас свойстве есть атрибут "DisplayNameAttribute", то вычисляем с помощью него.
            var propertyInfo = targetType.GetProperty(targetTypePropertyName);
            var localizedAttributes = propertyInfo.GetCustomAttributes(typeof(DisplayNameLocalizedAttribute), true);
            if (localizedAttributes.Length > 0)
            {
                var localizedAttribute = (DisplayNameLocalizedAttribute)localizedAttributes[0];
                localizedMetadata = localizedAttribute.GetLocalizedDisplayName(targetCulture);
                propertyCultureSpecificMetadata.Metadata.TryAdd(targetCulture.LCID, localizedMetadata);
                return true;
            }

            // Пытаемся получить ресурс основываясь на названии свойства и названии модели 
            // (т.е. тут вступают в силу соглашения по именованию ресурсов)
            var keys = EnumerateCandidateKeysForProperty(targetType, targetTypePropertyName);

            foreach (var key in keys)
            {
                var localizedPropertyNameCandidate = _resourceManagers.Select(m => m.GetString(key, targetCulture)).FirstOrDefault(l => l != null);
                if (!string.IsNullOrEmpty(localizedPropertyNameCandidate))
                {
                    localizedMetadata = localizedPropertyNameCandidate;
                    propertyCultureSpecificMetadata.Metadata.TryAdd(targetCulture.LCID, localizedPropertyNameCandidate);
                    return true;
                }
            }

            // Ничего не нашли, печаль.
            propertyCultureSpecificMetadata.Metadata.TryAdd(targetCulture.LCID, null);
            return false;
        }

        protected override ModelMetadata CreateMetadata(
            IEnumerable<Attribute> attributes,
            Type containerType,
            Func<object> modelAccessor,
            Type modelType,
            string propertyName)
        {
            // CultureInfo culture = Thread.CurrentThread.CurrentUICulture;
            var meta = base.CreateMetadata(attributes, containerType, modelAccessor, modelType, propertyName);
            if (!_localizableViewModelIndicators.Any(i => i.IsAssignableFrom(containerType)) || string.IsNullOrEmpty(propertyName))
            {
                return meta;
            }

            string displayName;
            if (!TryGetLocalizedDisplayName(meta.ContainerType, propertyName, out displayName))
            {
                return meta;
            }

            meta.DisplayName = displayName;

            if (string.IsNullOrEmpty(meta.DisplayName))
            {
                meta.DisplayName = string.Format("[[{0}]]", propertyName);
            }

            return meta;
        }

        private static Type EntityViewModelBaseType = typeof(EntityViewModelBase);

        private static IEnumerable<string> EnumerateCandidateKeysForProperty(Type targetType, string targetTypePropertyName)
        {
            yield return targetType.Name + "_" + targetTypePropertyName;

            Type currentType = targetType.BaseType;

            while (currentType != null &&
                currentType != EntityViewModelBaseType
                && currentType != typeof(object))
            {
                string viewModeName = currentType.Name.Contains("`") ? currentType.Name.Remove(currentType.Name.IndexOf("`")) : currentType.Name;
                yield return viewModeName + "_" + targetTypePropertyName;
                currentType = currentType.BaseType;
            }

            yield return targetTypePropertyName;
        }

        #region nested types
        private class TypePropertiesMetadata
        {
            private readonly ConcurrentDictionary<string, PropertyCultureSpecificMetadata> _metadata = new ConcurrentDictionary<string, PropertyCultureSpecificMetadata>();
            public ConcurrentDictionary<string, PropertyCultureSpecificMetadata> Metadata
            {
                get
                {
                    return _metadata;
                }
            }
        }

        private class PropertyCultureSpecificMetadata
        {
            private readonly ConcurrentDictionary<int, string> _metadata = new ConcurrentDictionary<int, string>();
            public ConcurrentDictionary<int, string> Metadata
            {
                get
                {
                    return _metadata;
                }
            }
        }
        #endregion
    }
}

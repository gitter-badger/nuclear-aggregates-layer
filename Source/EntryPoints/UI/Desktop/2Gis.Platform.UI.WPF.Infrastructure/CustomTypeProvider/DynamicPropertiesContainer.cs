using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.CustomTypeProvider
{
    //// Original source at http://msmvps.com/blogs/matthieu/archive/2011/09/12/sl5-icustomtypeprovider.aspx
    public sealed class DynamicPropertiesContainer<T> : IDynamicPropertiesContainer, IDynamicPropertiesContainerConfigurator, ICustomTypeProvider 
        where T : IDynamicPropertiesContainer
    {
        private sealed class  DynamicPropertyBucket
        {
            public DynamicPropertyInfo Info { get; set; }
            public object Value { get; set; }
        }

        private readonly Dictionary<string, DynamicPropertyBucket> _dynamicPropertiesStorage = new Dictionary<string, DynamicPropertyBucket>();
        private readonly Lazy<DynamicPropertiesContainerTypeMetadata<DynamicPropertiesContainer<T>>> _typeMetadata;

        private readonly Type _dynamicPropertiesHostType = typeof(T);
        private readonly Dictionary<string, PropertyInfo> _dynamicPropertiesHostClrProperties;

        private readonly object _lockSync = new object();
        private bool _isLocked;

        public DynamicPropertiesContainer()
        {
            _typeMetadata = new Lazy<DynamicPropertiesContainerTypeMetadata<DynamicPropertiesContainer<T>>>(CreateTypeMetadata);
            _dynamicPropertiesHostClrProperties = _dynamicPropertiesHostType.GetProperties().ToDictionary(info => info.Name);
        }

        #region IDynamicPropertiesContainerConfigurator
        void IDynamicPropertiesContainerConfigurator.AddProperty(string name, Type type)
        {
            CheckLockStatus(false);
            CheckIfNameExists(name);

            _dynamicPropertiesStorage.Add(name, new DynamicPropertyBucket { Info = new DynamicPropertyInfo(name, type) });
        }

        void IDynamicPropertiesContainerConfigurator.AddProperty(string name, Type type, object value, IEnumerable<Attribute> attributes)
        {
            CheckLockStatus(false);
            CheckIfNameExists(name);
            
            _dynamicPropertiesStorage.Add(name, new DynamicPropertyBucket { Info = new DynamicPropertyInfo(name, type), Value = value });
        }

        void IDynamicPropertiesContainerConfigurator.Lock()
        {
            IsLocked = true;
        }

        #endregion

        #region IDynamicPropertiesContainer
        object IDynamicPropertiesContainer.GetDynamicPropertyValue(string propertyName)
        {
            DynamicPropertyBucket propertyBucket;
            if (_dynamicPropertiesStorage.TryGetValue(propertyName, out propertyBucket))
            {
                return propertyBucket.Value ?? propertyBucket.Info.GetDefaultValue(this);
            }

            throw new InvalidOperationException("There is no dynamic property " + propertyName);
        }

        void IDynamicPropertiesContainer.SetDynamicPropertyValue(string propertyName, object value)
        {
            DynamicPropertyBucket propertyBucket;
            if (!_dynamicPropertiesStorage.TryGetValue(propertyName, out propertyBucket))
            {
                throw new InvalidOperationException("There is no dynamic property " + propertyName);
            }

            if (!ValidateValueType(value, propertyBucket.Info.PropertyType))
            {
                throw new ArgumentException("Value is of the wrong type or null for a non-nullable type.");
            }

            propertyBucket.Value = value;
        }

        bool IDynamicPropertiesContainer.ContainsDynamicProperty(string propertyName)
        {
            return _dynamicPropertiesStorage.ContainsKey(propertyName);
        }

        bool IDynamicPropertiesContainer.TryGetDynamicPropertyInfo(string propertyName, out PropertyInfo propertyInfo)
        {
            propertyInfo = null;

            DynamicPropertyBucket propertyBucket;
            if (!_dynamicPropertiesStorage.TryGetValue(propertyName, out propertyBucket))
            {
                return false;
            }
            
            propertyInfo = propertyBucket.Info;
            return true;
        }

        PropertyInfo[] IDynamicPropertiesContainer.GetAllProperties()
        {
            return _typeMetadata.Value.GetProperties();
        }

        PropertyInfo[] IDynamicPropertiesContainer.GetDynamicProperties()
        {
            return _dynamicPropertiesStorage.Select(p => (PropertyInfo)p.Value.Info).ToArray();
        }
        #endregion

        #region IDynamicPropertiesContainerConfigurator
        Type ICustomTypeProvider.GetCustomType()
        {
            if (!IsLocked)
            {
                throw new InvalidOperationException("Properties container have to be locked on changes before using");
            }

            return _typeMetadata.Value;
        }
        #endregion

        private bool IsLocked 
        {
            get
            {
                lock (_lockSync)
                {
                    return _isLocked;
                }
            }
            set
            {
                lock (_lockSync)
                {
                    _isLocked = value;
                }
            }
        }

        private DynamicPropertiesContainerTypeMetadata<DynamicPropertiesContainer<T>> CreateTypeMetadata()
        {
            return new DynamicPropertiesContainerTypeMetadata<DynamicPropertiesContainer<T>>(_dynamicPropertiesStorage.Select(p => p.Value.Info));
        }

        private static bool ValidateValueType(object value, Type type)
        {
            if (value == null)
            {
                if (!type.IsValueType)
                {
                    return true;
                }

                return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
            }

            return type.IsInstanceOfType(value);
        }

        private void CheckIfNameExists(string name)
        {
            if (_dynamicPropertiesStorage.ContainsKey(name))
            {
                throw new InvalidOperationException("Dynamic property with the same name \"" + name + "\" already exists");
            }

            if (_dynamicPropertiesHostClrProperties.ContainsKey(name))
            {
                throw new InvalidOperationException("CLR property with the same name \"" + name + "\" already exists in dynamic properties host type " + _dynamicPropertiesHostType);
            }
        }

        private void CheckLockStatus(bool admissibleStatus)
        {
            if (IsLocked != admissibleStatus)
            {
                var status = admissibleStatus ? "non" : string.Empty;
                throw new InvalidOperationException("Can't execute operation in " + status + " locked dynamic properties container");
            }
        }
    }
}
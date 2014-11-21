using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Platform.UI.WPF.Infrastructure.CustomTypeProvider;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel
{
    public abstract class DynamicViewModel : 
        IDynamicPropertiesContainer,
        IDynamicPropertiesContainerConfigurator,
        ICustomTypeProvider,
        IDynamicViewModel,
        INotifyPropertyChanged
    {
        private readonly DynamicPropertiesContainer<DynamicViewModel> _dynamicViewModelPropertiesContainer = new DynamicPropertiesContainer<DynamicViewModel>();

        public event PropertyChangedEventHandler PropertyChanged;
        
        public abstract IViewModelIdentity Identity { get; }

        #region ICustomTypeProvider
        Type ICustomTypeProvider.GetCustomType()
        {
            return ((ICustomTypeProvider)_dynamicViewModelPropertiesContainer).GetCustomType();
        }
        #endregion

        #region IDynamicPropertiesContainerConfigurator
        void IDynamicPropertiesContainerConfigurator.AddProperty(string name, Type type)
        {
            ((IDynamicPropertiesContainerConfigurator)_dynamicViewModelPropertiesContainer).AddProperty(name, type);
        }

        void IDynamicPropertiesContainerConfigurator.AddProperty(string name, Type type, object value, IEnumerable<Attribute> attributes)
        {
            ((IDynamicPropertiesContainerConfigurator)_dynamicViewModelPropertiesContainer).AddProperty(name, type, value, attributes);
        }

        void IDynamicPropertiesContainerConfigurator.Lock()
        {
            ((IDynamicPropertiesContainerConfigurator)_dynamicViewModelPropertiesContainer).Lock();
        }
        #endregion

        #region IDynamicPropertiesContainer
        object IDynamicPropertiesContainer.GetDynamicPropertyValue(string propertyName)
        {
            return ((IDynamicPropertiesContainer)_dynamicViewModelPropertiesContainer).GetDynamicPropertyValue(propertyName);
        }

        void IDynamicPropertiesContainer.SetDynamicPropertyValue(string propertyName, object value)
        {
            ((IDynamicPropertiesContainer)_dynamicViewModelPropertiesContainer).SetDynamicPropertyValue(propertyName, value);
            OnPropertyChanged(propertyName);
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

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
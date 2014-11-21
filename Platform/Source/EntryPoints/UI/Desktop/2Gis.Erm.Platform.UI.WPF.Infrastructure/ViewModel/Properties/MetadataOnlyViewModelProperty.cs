using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Metadata.Entities;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Properties
{
    /// <summary>
    /// Свойство viewmodel, содержащее просто описание необходимого свойства
    /// </summary>
    public sealed class MetadataOnlyViewModelProperty : IViewModelProperty
    {
        private readonly string _name;
        private readonly Type _propertyType;
        private readonly IEnumerable<IPropertyFeature> _features;

        public MetadataOnlyViewModelProperty(string name, Type propertyType, IEnumerable<IPropertyFeature> features)
        {
            _name = name;
            _propertyType = propertyType;
            _features = features ?? Enumerable.Empty<IPropertyFeature>();
        }

        public string Name 
        {
            get
            {
                return _name;
            }
        }

        public Type PropertyType
        {
            get
            {
                return _propertyType;
            }
        }

        public IEnumerable<IPropertyFeature> Features
        {
            get { return _features; }
        }
    }
}
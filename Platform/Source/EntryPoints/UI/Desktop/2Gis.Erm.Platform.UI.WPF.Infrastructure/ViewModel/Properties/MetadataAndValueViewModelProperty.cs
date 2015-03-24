using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.UI.Metadata.Indicators;

using NuClear.Metamodeling.Domain.Entities;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Properties
{
    /// <summary>
    /// Свойство viewmodel, содержащее метаданные + уже вычисленное значение
    /// </summary>
    public sealed class MetadataAndValueViewModelProperty : IViewModelProperty
    {
        private readonly string _name;
        private readonly Type _propertyType;
        private readonly object _value;
        private readonly IEnumerable<IPropertyFeature> _features;

        public MetadataAndValueViewModelProperty(string name, Type propertyType, object value, IEnumerable<IPropertyFeature> features)
        {
            _name = name;
            _propertyType = propertyType;
            _value = value;
            _features = features ?? Enumerable.Empty<IPropertyFeature>();
        }

        public string Name
        {
            get { return _name; }
        }

        public Type PropertyType
        {
            get { return _propertyType; }
        }

        public object Value
        {
            get { return _value; }
        }

        public IEnumerable<IPropertyFeature> Features
        {
            get { return _features; }
        }
    }
}
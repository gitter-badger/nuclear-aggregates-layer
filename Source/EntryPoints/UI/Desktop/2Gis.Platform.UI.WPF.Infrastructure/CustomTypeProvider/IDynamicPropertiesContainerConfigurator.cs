using System;
using System.Collections.Generic;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.CustomTypeProvider
{
    public interface IDynamicPropertiesContainerConfigurator
    {
        void AddProperty(string name, Type type);
        void AddProperty(string name, Type type, object value, IEnumerable<Attribute> attributes);
        void Lock();
    }
}
using System;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Common.Utils;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features
{
    public class PropertyDescriptor : IPropertyDescriptor
    {
        public PropertyDescriptor(Type type, string propertyName)
        {
            PropertyName = propertyName;
            Type = type;
        }

        public string PropertyName { get; private set; }
        public Type Type { get; private set; }
        
        public static PropertyDescriptor Create<T>(Expression<Func<T, object>> propertyNameExpression)
        {
            var propertyName = StaticReflection.GetMemberName(propertyNameExpression);
            return new PropertyDescriptor(typeof(T), propertyName);
        }
    }
}
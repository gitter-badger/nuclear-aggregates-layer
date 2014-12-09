using System;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Common.Utils;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features
{
    public class PropertyDescriptor<T> : IPropertyDescriptor
    {
        public PropertyDescriptor(Expression<Func<T, object>> propertyNameExpression)
        {
            PropertyName = StaticReflection.GetMemberName(propertyNameExpression);
            Type = typeof(T);
            PropertyFunc = propertyNameExpression.Compile();
        }

        public string PropertyName { get; private set; }
        public Type Type { get; private set; }
        private Func<T, object> PropertyFunc { get; set; }

        public bool TryGetValue(object container, out object result)
        {
            result = null;
            if (!(container is T))
            {
                return false;
            }

            result = PropertyFunc.Invoke((T)container);
            return true;
        }

        public string ResourceKeyToString()
        {
            return PropertyName;
        }
    }
}
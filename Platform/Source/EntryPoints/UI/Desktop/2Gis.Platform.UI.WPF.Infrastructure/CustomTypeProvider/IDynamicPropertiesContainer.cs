using System.Reflection;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.CustomTypeProvider
{
    public interface IDynamicPropertiesContainer
    {
        object GetDynamicPropertyValue(string propertyName);
        void SetDynamicPropertyValue(string propertyName, object value);
        bool ContainsDynamicProperty(string propertyName);
        bool TryGetDynamicPropertyInfo(string propertyName, out PropertyInfo propertyInfo);
        PropertyInfo[] GetAllProperties();
        PropertyInfo[] GetDynamicProperties();
    }
}
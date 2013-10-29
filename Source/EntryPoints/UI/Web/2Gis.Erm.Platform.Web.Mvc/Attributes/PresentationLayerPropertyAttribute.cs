using System;

namespace DoubleGis.Erm.Platform.Web.Mvc.Attributes
{
    /// <summary>
    /// Маркерный атрибут, показывает, что свойство ViewModel помеченное им, не отображается на какие-то данные в хранилище данных, 
    /// т.е. это свойство необходимо только для использования в PresentationLayer ViewModel для настроек UI
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class PresentationLayerPropertyAttribute : Attribute
    {
    }
}

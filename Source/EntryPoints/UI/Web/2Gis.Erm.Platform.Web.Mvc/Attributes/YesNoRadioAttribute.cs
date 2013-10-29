using System;

namespace DoubleGis.Erm.Platform.Web.Mvc.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class YesNoRadioAttribute: Attribute
    {
    }
}
using System;

using DoubleGis.Erm.Platform.Model.Metadata.Enums;

namespace DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public sealed class DependencyAttribute : Attribute
    {
        public DependencyAttribute(DependencyType dependencyType, string dependentFieldName, string dependencyExpression)
        {
            DependencyType = dependencyType;
            DependentFieldName = dependentFieldName;
            DependencyExpression = dependencyExpression;
        }

        public string DependentFieldName { get; private set; }
        public string DependencyExpression { get; private set; }
        public DependencyType DependencyType { get; private set; }
    }
}
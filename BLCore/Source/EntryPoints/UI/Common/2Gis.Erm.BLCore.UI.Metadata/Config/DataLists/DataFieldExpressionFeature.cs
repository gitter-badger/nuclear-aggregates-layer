using System;

using NuClear.Metamodeling.Elements.Aspects.Features;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.DataLists
{
    public sealed class DataFieldExpressionFeature : IMetadataFeature
    {
        private readonly string _propertyName;
        private readonly Type _propertyType;
        private readonly string _expression;

        public DataFieldExpressionFeature(string propertyName, Type propertyType, string expression)
        {
            _propertyName = propertyName;
            _propertyType = propertyType;
            _expression = expression;
        }

        public string PropertyName
        {
            get { return _propertyName; }
        }

        public Type PropertyType
        {
            get { return _propertyType; }
        }

        public string Expression
        {
            get { return _expression; }
        }
    }
}
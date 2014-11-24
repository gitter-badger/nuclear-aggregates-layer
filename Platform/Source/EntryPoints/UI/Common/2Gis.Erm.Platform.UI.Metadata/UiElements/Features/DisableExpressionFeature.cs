using System;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features;

namespace DoubleGis.Erm.Platform.UI.Metadata.UiElements.Features
{
    public sealed class DisableExpressionFeature : IMetadataFeature
    {
        public DisableExpressionFeature(Expression<Func<object, bool>> expression)
        {
            Expression = expression;
        }

        public Expression<Func<object, bool>> Expression { get; private set; }
    }
}
using System;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;

namespace DoubleGis.Erm.Platform.UI.Metadata.UiElements.Features
{
    public sealed class DisableExpressionFeature : IMetadataFeature
    {
        public DisableExpressionFeature(Expression<Func<IViewModelBase, bool>> expression)
        {
            Expression = expression;
        }

        public Expression<Func<IViewModelBase, bool>> Expression { get; private set; }
    }
}

using System;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Model.Aspects;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements.Features;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.ViewModel
{
    public sealed class MessageExpressionFeature<T> : ExpressionFeatureAbstract<T>, IMessageExpressionFeature
        where T : IAspect
    {
        public MessageExpressionFeature(Expression<Func<T, bool>> expression, IStringResourceDescriptor messageDescriptor, MessageType messageType)
            : base(expression)
        {
            MessageType = messageType;
            MessageDescriptor = messageDescriptor;
        }

        public IStringResourceDescriptor MessageDescriptor { get; private set; }
        public MessageType MessageType { get; private set; }
    }
}
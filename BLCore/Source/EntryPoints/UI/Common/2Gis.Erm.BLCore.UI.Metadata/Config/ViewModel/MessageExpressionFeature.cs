using System;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Model.Aspects;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.ViewModel
{
    public sealed class MessageExpressionFeature<T> : IMessageExpressionFeature
        where T : IAspect
    {
        public MessageExpressionFeature(Expression<Func<T, bool>> expression, IStringResourceDescriptor messageDescriptor, MessageType messageType)
        {
            MessageType = messageType;
            MessageDescriptor = messageDescriptor;
            Expression = expression;
        }

        public IStringResourceDescriptor MessageDescriptor { get; private set; }
        public MessageType MessageType { get; private set; }
        public LambdaExpression Expression { get; private set; }
    }
}
using System.Linq.Expressions;

using NuClear.Metamodeling.UI.Elements.Aspects.Features.Resources;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements.Features.Expressions;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.ViewModel
{
    public sealed class MessageExpressionsFeature : IMessageExpressionsFeature
    {
        public MessageExpressionsFeature(LogicalOperation expressionsCombination,
                                         LambdaExpression[] expressions,
                                         IStringResourceDescriptor messageDescriptor,
                                         MessageType messageType)
        {
            LogicalOperation = expressionsCombination;
            MessageType = messageType;
            MessageDescriptor = messageDescriptor;
            Expressions = expressions;
        }

        public IStringResourceDescriptor MessageDescriptor { get; private set; }
        public MessageType MessageType { get; private set; }
        public LogicalOperation LogicalOperation { get; private set; }
        public LambdaExpression[] Expressions { get; private set; }
    }
}
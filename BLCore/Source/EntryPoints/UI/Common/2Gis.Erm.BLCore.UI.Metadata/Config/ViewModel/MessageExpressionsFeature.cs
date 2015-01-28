using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Common.Utils.Resources;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.ViewModel
{
    public sealed class MessageExpressionsFeature : IMessageExpressionsFeature
    {
        public MessageExpressionsFeature(ExpressionsCombination expressionsCombination,
                                         LambdaExpression[] expressions,
                                         IStringResourceDescriptor messageDescriptor,
                                         MessageType messageType)
        {
            ExpressionsCombination = expressionsCombination;
            MessageType = messageType;
            MessageDescriptor = messageDescriptor;
            Expressions = expressions;
        }

        public IStringResourceDescriptor MessageDescriptor { get; private set; }
        public MessageType MessageType { get; private set; }
        public ExpressionsCombination ExpressionsCombination { get; private set; }
        public LambdaExpression[] Expressions { get; private set; }
    }
}
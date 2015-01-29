﻿using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources;
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
            ExpressionsCombination = expressionsCombination;
            MessageType = messageType;
            MessageDescriptor = messageDescriptor;
            Expressions = expressions;
        }

        public IStringResourceDescriptor MessageDescriptor { get; private set; }
        public MessageType MessageType { get; private set; }
        public LogicalOperation ExpressionsCombination { get; private set; }
        public LambdaExpression[] Expressions { get; private set; }
    }
}
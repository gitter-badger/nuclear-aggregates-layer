using System.Linq;

using DoubleGis.Erm.Platform.Common.Utils.Resources;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements.Features.Expressions;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.ViewModel
{
    public class MessageExpressionsFeatureAspect<TBuilder, TMetadataElement> : ExpressionsFeatureAspect<TBuilder, TMetadataElement>
        where TBuilder : MetadataElementBuilder<TBuilder, TMetadataElement>, new()
        where TMetadataElement : MetadataElement
    {
        private readonly IStringResourceDescriptor _messageDescriptor;
        private readonly MessageType _messageType;
        public MessageExpressionsFeatureAspect(TBuilder aspectHostBuilder, IStringResourceDescriptor messageDescriptor, MessageType messageType) : base(aspectHostBuilder)
        {
            _messageDescriptor = messageDescriptor;
            _messageType = messageType;
        }

        public override TBuilder Combine(ExpressionsCombination combination)
        {
            AspectHostBuilder.WithFeatures(new MessageExpressionsFeature(combination, Expressions.ToArray(), _messageDescriptor, _messageType));
            return AspectHostBuilder;
        }
    }
}
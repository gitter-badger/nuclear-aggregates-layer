using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.ViewModel
{
    public interface IMessageFeature : IMetadataFeature
    {
        IStringResourceDescriptor MessageDescriptor { get; }
        MessageType MessageType { get; }
    }
}
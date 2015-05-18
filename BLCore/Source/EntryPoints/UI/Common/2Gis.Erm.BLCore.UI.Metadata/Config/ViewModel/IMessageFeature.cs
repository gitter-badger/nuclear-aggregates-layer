using DoubleGis.Erm.Platform.UI.Metadata.UIElements;

using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Metamodeling.UI.Elements.Aspects.Features.Resources;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.ViewModel
{
    public interface IMessageFeature : IMetadataFeature
    {
        IStringResourceDescriptor MessageDescriptor { get; }
        MessageType MessageType { get; }
    }
}
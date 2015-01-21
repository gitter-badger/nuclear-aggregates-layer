using DoubleGis.Erm.Platform.Common.Utils.Resources;

using NuClear.Metamodeling.Elements;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.Parts
{
    public interface IPartsContainerElement : IMetadataElementAspect
    {
        bool HasParts { get; }
        ResourceEntryKey[] Parts { get; }
    }
}

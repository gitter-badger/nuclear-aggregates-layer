using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.UI.Utils.Resources;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.Parts
{
    public interface IPartsContainerElement : IMetadataElementAspect
    {
        bool HasParts { get; }
        ResourceEntryKey[] Parts { get; }
    }
}

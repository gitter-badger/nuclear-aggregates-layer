using DoubleGis.Erm.Platform.Common.Utils.Resources;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.Parts
{
    public interface IPartsContainerElement : IMetadataElementAspect
    {
        bool HasParts { get; }
        ResourceEntryKey[] Parts { get; }
    }
}
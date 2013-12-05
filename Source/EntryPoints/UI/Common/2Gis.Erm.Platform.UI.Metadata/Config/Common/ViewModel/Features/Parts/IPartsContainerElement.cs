using DoubleGis.Erm.Platform.Common.Utils.Resources;
using DoubleGis.Erm.Platform.Model.Metadata.Common;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.Parts
{
    public interface IPartsContainerElement : IConfigElementAspect
    {
        bool HasParts { get; }
        ResourceEntryKey[] Parts { get; }
    }
}

using DoubleGis.Erm.Platform.Model.Metadata.Common;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.Parts
{
    public interface IViewModelPartsFeature : IViewModelFeature
    {
        ResourceEntryKey[] PartKeys { get; }
    }
}

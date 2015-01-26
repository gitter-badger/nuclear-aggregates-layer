using NuClear.Metamodeling.Utils.Resources;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.Parts
{
    public interface IViewModelPartsFeature : IViewModelFeature
    {
        ResourceEntryKey[] PartKeys { get; }
    }
}

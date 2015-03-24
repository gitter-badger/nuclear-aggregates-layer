using NuClear.Metamodeling.UI.Utils.Resources;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.Parts
{
    public sealed class ViewModelPartsFeature : IViewModelPartsFeature
    {
        private readonly ResourceEntryKey[] _partKeys;

        public ViewModelPartsFeature(params ResourceEntryKey[] partKeys)
        {
            _partKeys = partKeys;
        }

        public ResourceEntryKey[] PartKeys
        {
            get
            {
                return _partKeys;
            }
        }
    }
}
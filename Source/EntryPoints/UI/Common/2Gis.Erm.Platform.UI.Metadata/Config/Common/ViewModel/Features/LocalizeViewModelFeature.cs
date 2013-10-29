using System.Resources;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features
{
    public sealed class LocalizeViewModelFeature : IViewModelFeature
    {
        public ResourceManager[] ResourceManagers { get; set; }
    }
}
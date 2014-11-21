using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.ResourceInfrastructure;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Localization
{
    public sealed class NullTitleProvider : ITitleProvider
    {
        public string Title
        {
            get
            {
                return "Title.Empty";
            }
        }
    }
}
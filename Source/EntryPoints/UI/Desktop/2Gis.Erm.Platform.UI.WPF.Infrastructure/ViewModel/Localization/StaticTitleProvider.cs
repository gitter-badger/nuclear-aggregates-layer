using DoubleGis.Erm.Platform.Model.Metadata.Common.Features.Titles;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.ResourceInfrastructure;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Localization
{
    public sealed class StaticTitleProvider : ITitleProvider
    {
        private readonly StaticTitleDescriptor _descriptor;

        public StaticTitleProvider(StaticTitleDescriptor descriptor)
        {
            _descriptor = descriptor;
        }

        public string Title
        {
            get
            {
                return _descriptor.Title;
            }
        }
    }
}
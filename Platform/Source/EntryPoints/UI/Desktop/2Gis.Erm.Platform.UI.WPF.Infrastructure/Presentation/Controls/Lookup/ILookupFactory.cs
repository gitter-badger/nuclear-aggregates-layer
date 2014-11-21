using DoubleGis.Erm.Platform.Model.Metadata.Entities.PropertyFeatures;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Lookup
{
    public interface ILookupFactory
    {
        LookupViewModel Create(IUseCase useCase, IViewModelIdentity hostViewModelIdentity, LookupPropertyFeature settings);
    }
}

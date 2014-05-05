using System.Collections.Generic;

using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel
{
    public interface IMetadata2ViewModelPropertiesMapper
    {
        bool CanMap(IUseCase useCase, IViewModelMetadata metadata, IViewModelIdentity targetViewModelIdentity);
        IEnumerable<IViewModelProperty> GetProperties(IUseCase useCase, IViewModelMetadata metadata, IViewModelIdentity targetViewModelIdentity);
    }
}
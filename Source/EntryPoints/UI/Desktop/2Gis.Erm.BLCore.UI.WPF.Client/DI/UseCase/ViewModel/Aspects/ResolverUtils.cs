using System.Collections.Generic;

using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.DI.UseCase.ViewModel.Aspects
{
    public static class ResolverUtils
    {
        public static IEnumerable<IViewModelProperty> ResolveViewModelProperties(
            this IMetadata2ViewModelPropertiesMapper[] mappers,
            IUseCase useCase,
            IViewModelStructure structure,
            IViewModelIdentity resolvingViewModelIdentity)
        {
            var viewModelProperties = new List<IViewModelProperty>();
            foreach (var mapper in mappers)
            {
                if (!mapper.CanMap(useCase, structure, resolvingViewModelIdentity))
                {
                    continue;
                }

                viewModelProperties.AddRange(mapper.GetProperties(useCase, structure, resolvingViewModelIdentity));
            }

            return viewModelProperties;
        }
    }
}

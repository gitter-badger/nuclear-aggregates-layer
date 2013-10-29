using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel
{
    public abstract class AbstractMetadata2ViewModelPropertiesMapper<TStructure> : IMetadata2ViewModelPropertiesMapper
        where TStructure : IViewModelStructure
    {
        private readonly Type _supportedViewModelDescriptor = typeof(TStructure);

        public bool CanMap(IUseCase useCase, IViewModelStructure structure, IViewModelIdentity targetViewModelIdentity)
        {
            return structure.GetType() == _supportedViewModelDescriptor;
        }

        public IEnumerable<IViewModelProperty> GetProperties(IUseCase useCase, IViewModelStructure structure, IViewModelIdentity targetViewModelIdentity)
        {
            return GetViewModelProperties(useCase, (TStructure)structure, targetViewModelIdentity);
        }

        protected abstract IEnumerable<IViewModelProperty> GetViewModelProperties(IUseCase useCase, TStructure structure, IViewModelIdentity targetViewModelIdentity);
    }
}
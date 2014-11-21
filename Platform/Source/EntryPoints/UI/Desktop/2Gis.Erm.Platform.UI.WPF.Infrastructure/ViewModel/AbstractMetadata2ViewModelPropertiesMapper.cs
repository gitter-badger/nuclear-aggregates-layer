using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel
{
    public abstract class AbstractMetadata2ViewModelPropertiesMapper<TStructure> : IMetadata2ViewModelPropertiesMapper
        where TStructure : IViewModelMetadata
    {
        private readonly Type _supportedViewModelDescriptor = typeof(TStructure);

        public bool CanMap(IUseCase useCase, IViewModelMetadata metadata, IViewModelIdentity targetViewModelIdentity)
        {
            return metadata.GetType() == _supportedViewModelDescriptor;
        }

        public IEnumerable<IViewModelProperty> GetProperties(IUseCase useCase, IViewModelMetadata metadata, IViewModelIdentity targetViewModelIdentity)
        {
            return GetViewModelProperties(useCase, (TStructure)metadata, targetViewModelIdentity);
        }

        protected abstract IEnumerable<IViewModelProperty> GetViewModelProperties(IUseCase useCase, TStructure metadata, IViewModelIdentity targetViewModelIdentity);
    }
}
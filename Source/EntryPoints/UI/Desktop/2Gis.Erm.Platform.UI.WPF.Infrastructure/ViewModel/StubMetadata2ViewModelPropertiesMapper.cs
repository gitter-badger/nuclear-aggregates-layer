using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel
{
    public sealed class StubMetadata2ViewModelPropertiesMapper : AbstractMetadata2ViewModelPropertiesMapper<IViewModelMetadata>
    {
        public StubMetadata2ViewModelPropertiesMapper(/*I_не_card_ViewModelPropertiesProvider propertiesProvider*/)
        {
        }

        protected override IEnumerable<IViewModelProperty> GetViewModelProperties(IUseCase useCase, IViewModelMetadata metadata, IViewModelIdentity targetViewModelIdentity)
        {
            throw new InvalidOperationException("Don't use stub Mapper");
        }
    }
}

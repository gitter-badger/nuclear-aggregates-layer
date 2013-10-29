﻿using DoubleGis.Erm.Platform.Model.Metadata.Common.Features.Titles;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.ResourceInfrastructure;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Localization
{
    public interface ITitleProviderFactory
    {
        ITitleProvider Create(ITitleDescriptor descriptor);
    }
}

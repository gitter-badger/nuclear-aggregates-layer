﻿using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts
{
    public interface IDealViewModel : IEntityViewModelAbstract<Deal>
    {
        string Name { get; set; }
    }
}
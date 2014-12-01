﻿using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts
{
    public interface IFirmViewModel : IEntityViewModelAbstract<Firm>
    {
        string Name { get; set; }
    }
}
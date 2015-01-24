﻿using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;

namespace DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts
{
    public interface IOperationViewModel : IViewModelAbstract
    {
        BusinessOperation Type { get; set; }
    }
}
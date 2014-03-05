﻿using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.AdvertisementElement;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.AdvertisementElements
{
    public interface INotifyAboutAdvertisementElementFileChangedOperationService : IOperation<NotifyAboutAdvertisementElementFileChangedIdentity>
    {
        void Notify(long advertisementElementId);
    }
}

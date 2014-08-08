using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.AdvertisementElements;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.AdvertisementElements.Workflow
{
    public sealed class NullAdvertisementElementStatusChangingStrategy : IAdvertisementElementStatusChangingStrategy
    {
        private static readonly Lazy<NullAdvertisementElementStatusChangingStrategy> Strategy =
            new Lazy<NullAdvertisementElementStatusChangingStrategy>(() => new NullAdvertisementElementStatusChangingStrategy());

        private NullAdvertisementElementStatusChangingStrategy()
        {
        }

        public static NullAdvertisementElementStatusChangingStrategy Instance
        {
            get { return Strategy.Value; }
        }

        public void Validate(AdvertisementElementStatus currentStatus, IEnumerable<AdvertisementElementDenialReason> denialReasons)
        {
        }

        public void Process(AdvertisementElementStatus currentStatus, IEnumerable<AdvertisementElementDenialReason> denialReasons)
        {
        }
    }
}
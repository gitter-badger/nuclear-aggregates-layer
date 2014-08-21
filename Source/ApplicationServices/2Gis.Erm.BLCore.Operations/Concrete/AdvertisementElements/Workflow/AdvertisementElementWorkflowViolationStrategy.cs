using System.Collections.Generic;
using System.Globalization;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.AdvertisementElements;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.AdvertisementElements.Workflow
{
    public sealed class AdvertisementElementWorkflowViolationStrategy : IAdvertisementElementStatusChangingStrategy
    {
        private readonly AdvertisementElementStatusValue _newStatus;

        public AdvertisementElementWorkflowViolationStrategy(AdvertisementElementStatusValue newStatus)
        {
            _newStatus = newStatus;
        }

        public void Validate(AdvertisementElementStatus currentStatus, IEnumerable<AdvertisementElementDenialReason> denialReasons)
        {
        }

        public void Process(AdvertisementElementStatus currentStatus, IEnumerable<AdvertisementElementDenialReason> denialReasons)
        {
            var currentStatusValue = (AdvertisementElementStatusValue)currentStatus.Status;
            throw new AdvertisementElementValidationWorkflowIsViolatedException(
                string.Format(BLResources.StatusTransitionIsNotAllowedForAdvElement,
                              currentStatusValue.ToStringLocalized(EnumResources.ResourceManager, CultureInfo.CurrentCulture),
                              _newStatus.ToStringLocalized(EnumResources.ResourceManager, CultureInfo.CurrentCulture)));
        }
    }
}
using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.AdvertisementElements;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.AdvertisementElements.Workflow
{
    public abstract class AdvertisementElementStatusBehaviour
    {
        public IEnumerable<IAdvertisementElementStatusChangingStrategy> GetStatusChangersTo(AdvertisementElementStatusValue desiredState)
        {
            switch (desiredState)
            {
                case AdvertisementElementStatusValue.ReadyForValidation:
                    return ChangeToReadyForValidation();
                case AdvertisementElementStatusValue.Valid:
                    return ChangeToValid();
                case AdvertisementElementStatusValue.Invalid:
                    return ChangeToInvalid();
                case AdvertisementElementStatusValue.Draft:
                    return ChangeToDraft();
                default:
                    throw new ArgumentOutOfRangeException("desiredState");
            }
        }

        protected virtual IEnumerable<IAdvertisementElementStatusChangingStrategy> ChangeToReadyForValidation()
        {
            return new[] { new AdvertisementElementWorkflowViolationStrategy(AdvertisementElementStatusValue.ReadyForValidation) };
        }

        protected virtual IEnumerable<IAdvertisementElementStatusChangingStrategy> ChangeToValid()
        {
            return new[] { new AdvertisementElementWorkflowViolationStrategy(AdvertisementElementStatusValue.Valid) };
        }

        protected virtual IEnumerable<IAdvertisementElementStatusChangingStrategy> ChangeToInvalid()
        {
            return new[] { new AdvertisementElementWorkflowViolationStrategy(AdvertisementElementStatusValue.Invalid) };
        }

        protected virtual IEnumerable<IAdvertisementElementStatusChangingStrategy> ChangeToDraft()
        {
            return new[] { new AdvertisementElementWorkflowViolationStrategy(AdvertisementElementStatusValue.Draft) };
        }
    }
}
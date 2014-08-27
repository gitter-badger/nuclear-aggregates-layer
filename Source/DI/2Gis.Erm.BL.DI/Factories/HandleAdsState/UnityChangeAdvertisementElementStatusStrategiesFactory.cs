using System;
using System.Collections.Generic;
using DoubleGis.Erm.BL.Operations.Concrete.AdvertisementElements.Workflow;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.AdvertisementElements;
using DoubleGis.Erm.BLCore.Operations.Concrete.AdvertisementElements.Workflow;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BL.DI.Factories.HandleAdsState
{
    public class UnityChangeAdvertisementElementStatusStrategiesFactory : IChangeAdvertisementElementStatusStrategiesFactory
    {
        private readonly IUnityContainer _unityContainer;
        
        public UnityChangeAdvertisementElementStatusStrategiesFactory(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }

        public IEnumerable<IAdvertisementElementStatusChangingStrategy> EvaluateProcessingStrategies(AdvertisementElementStatusValue currentStatus,
                                                                                                     AdvertisementElementStatusValue newStatus)
        {
            return GetCurrentStatusBehaviour(currentStatus).GetStatusChangersTo(newStatus);
        }

        private AdvertisementElementStatusBehaviour GetCurrentStatusBehaviour(AdvertisementElementStatusValue currentStatus)
        {
            switch (currentStatus)
            {
                case AdvertisementElementStatusValue.ReadyForValidation:
                    return (AdvertisementElementStatusBehaviour)_unityContainer.Resolve(typeof(AdvertisementElementStatusBehaviourReadyForValidation));
                case AdvertisementElementStatusValue.Valid:
                    return (AdvertisementElementStatusBehaviour)_unityContainer.Resolve(typeof(AdvertisementElementStatusBehaviourValid));
                case AdvertisementElementStatusValue.Invalid:
                    return (AdvertisementElementStatusBehaviour)_unityContainer.Resolve(typeof(AdvertisementElementStatusBehaviourInvalid));
                case AdvertisementElementStatusValue.Draft:
                    return (AdvertisementElementStatusBehaviour)_unityContainer.Resolve(typeof(AdvertisementElementStatusBehaviourDraft));
                default:
                    throw new ArgumentOutOfRangeException("currentStatus");
            }
        }
    }
}
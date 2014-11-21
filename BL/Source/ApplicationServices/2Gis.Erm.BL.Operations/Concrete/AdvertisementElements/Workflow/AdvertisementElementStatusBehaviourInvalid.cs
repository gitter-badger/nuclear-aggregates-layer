using System.Collections.Generic;
using DoubleGis.Erm.BL.API.Operations.Concrete.AdvertisementElements;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.AdvertisementElements;
using DoubleGis.Erm.BLCore.Operations.Concrete.AdvertisementElements.Workflow;

namespace DoubleGis.Erm.BL.Operations.Concrete.AdvertisementElements.Workflow
{
    public class AdvertisementElementStatusBehaviourInvalid : AdvertisementElementStatusBehaviour
    {
        private readonly IResetAdvertisementElementToDraftOperationService _resetAdvertisementElementToDraftOperationService;
        private readonly IApproveAdvertisementElementOperationService _approveAdvertisementElementOperationService;
        private readonly IDenyAdvertisementElementOperationService _denyAdvertisementElementOperationService;

        public AdvertisementElementStatusBehaviourInvalid(
            IResetAdvertisementElementToDraftOperationService resetAdvertisementElementToDraftOperationService,
            IApproveAdvertisementElementOperationService approveAdvertisementElementOperationService,
            IDenyAdvertisementElementOperationService denyAdvertisementElementOperationService)
        {
            _resetAdvertisementElementToDraftOperationService = resetAdvertisementElementToDraftOperationService;
            _approveAdvertisementElementOperationService = approveAdvertisementElementOperationService;
            _denyAdvertisementElementOperationService = denyAdvertisementElementOperationService;
        }

        protected override IEnumerable<IAdvertisementElementStatusChangingStrategy> ChangeToDraft()
        {
            return new[] { _resetAdvertisementElementToDraftOperationService };
        }

        protected override IEnumerable<IAdvertisementElementStatusChangingStrategy> ChangeToInvalid()
        {
            return new[] { _denyAdvertisementElementOperationService };
        }

        protected override IEnumerable<IAdvertisementElementStatusChangingStrategy> ChangeToValid()
        {
            return new[] { _approveAdvertisementElementOperationService };
        }
    }
}
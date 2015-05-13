using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Advertisements;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Old.Advertisements
{
    public class SelectAdvertisementToWhiteListHandlerTest : UseModelEntityHandlerTestBase<Firm, SelectAdvertisementToWhiteListRequest, EmptyResponse>
    {
        private readonly IAppropriateEntityProvider<Advertisement> _advertisementEntityProvider;

        public SelectAdvertisementToWhiteListHandlerTest(IPublicService publicService,
                                                         IAppropriateEntityProvider<Firm> appropriateEntityProvider,
                                                         IAppropriateEntityProvider<Advertisement> advertisementEntityProvider)
            : base(publicService, appropriateEntityProvider)
        {
            _advertisementEntityProvider = advertisementEntityProvider;
        }

        protected override bool TryCreateRequest(Firm modelEntity, out SelectAdvertisementToWhiteListRequest request)
        {
            request = null;
            var advertisement =
                _advertisementEntityProvider.Get(new FindSpecification<Advertisement>(x => x.FirmId == modelEntity.Id && !x.IsSelectedToWhiteList));

            if (advertisement == null)
            {
                return false;
            }

            request = new SelectAdvertisementToWhiteListRequest
                {
                    FirmId = modelEntity.Id,
                    AdvertisementId = advertisement.Id
                };

            return true;
        }

        protected override FindSpecification<Firm> ModelEntitySpec
        {
            get { return Specs.Find.ActiveAndNotDeleted<Firm>() && new FindSpecification<Firm>(f => f.Advertisements.Count(a => !a.IsDeleted) > 1); }
        }
    }
}
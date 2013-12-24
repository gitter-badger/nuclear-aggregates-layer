using System.Linq;

using DoubleGis.Erm.BL.API.Operations.Concrete.Old.Orders;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Old.LegalPersonProfiles
{
    public class SetLegalPersonProfileForOrderHandlerTest : UseModelEntityHandlerTestBase<Order, ChangeOrderLegalPersonProfileRequest, EmptyResponse>
    {
        private readonly IAppropriateEntityProvider<LegalPersonProfile> _legalPersonProfileAppropriateEntityProvider;

        public SetLegalPersonProfileForOrderHandlerTest(IPublicService publicService,
                                                        IAppropriateEntityProvider<Order> appropriateEntityProvider,
                                                        IAppropriateEntityProvider<LegalPersonProfile> legalPersonProfileAppropriateEntityProvider)
            : base(publicService, appropriateEntityProvider)
        {
            _legalPersonProfileAppropriateEntityProvider = legalPersonProfileAppropriateEntityProvider;
        }

        protected override FindSpecification<Order> ModelEntitySpec
        {
            get
            {
                return Specs.Find.ActiveAndNotDeleted<Order>() &&
                       new FindSpecification<Order>(
                           o => o.LegalPersonProfileId == null && o.LegalPerson.LegalPersonProfiles.Count(lpp => lpp.IsActive && !lpp.IsDeleted) > 1);
            }
        }

        protected override bool TryCreateRequest(Order modelEntity, out ChangeOrderLegalPersonProfileRequest request)
        {
            var legalPersonProfile = _legalPersonProfileAppropriateEntityProvider.Get(new FindSpecification<LegalPersonProfile>(x => x.LegalPersonId == modelEntity.LegalPersonId));

            request = new ChangeOrderLegalPersonProfileRequest
                {
                    LegalPersonProfileId = legalPersonProfile.Id,
                    OrderId = modelEntity.Id
                };

            return true;
        }
    }
}
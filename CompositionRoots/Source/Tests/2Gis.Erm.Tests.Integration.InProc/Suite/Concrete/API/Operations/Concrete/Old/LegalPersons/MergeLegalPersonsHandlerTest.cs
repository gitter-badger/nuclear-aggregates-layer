using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.LegalPersons;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Old.LegalPersons
{
    public class MergeLegalPersonsHandlerTest :
        UseModelEntityHandlerTestBase<LegalPerson, MergeLegalPersonsRequest, EmptyResponse>
    {
        private readonly IAppropriateEntityProvider<LegalPerson> _appropriateEntityProvider;

        public MergeLegalPersonsHandlerTest(IPublicService publicService, IAppropriateEntityProvider<LegalPerson> appropriateEntityProvider)
            : base(publicService, appropriateEntityProvider)
        {
            _appropriateEntityProvider = appropriateEntityProvider;
        }

        protected override FindSpecification<LegalPerson> ModelEntitySpec
        {
            get
            {
                return Specs.Find.ActiveAndNotDeleted<LegalPerson>() &&
                       new FindSpecification<LegalPerson>(lp => lp.LegalPersonTypeEnum == LegalPersonType.Businessman && !string.IsNullOrEmpty(lp.Inn));
            }
        }

        protected override bool TryCreateRequest(LegalPerson modelEntity, out MergeLegalPersonsRequest request)
        {
            request = null;

            var appendedLp = _appropriateEntityProvider.Get(Specs.Find.ActiveAndNotDeleted<LegalPerson>() &&
                                                            new FindSpecification<LegalPerson>(
                                                                lp =>
                                                                lp.Id != modelEntity.Id && lp.LegalPersonTypeEnum == LegalPersonType.Businessman &&
                                                                lp.Inn == modelEntity.Inn));

            if (appendedLp == null)
            {
                return false;
            }

            request = new MergeLegalPersonsRequest
                {
                    AppendedLegalPersonId = appendedLp.Id,
                    MainLegalPersonId = modelEntity.Id
                };

            return true;
        }
    }
}
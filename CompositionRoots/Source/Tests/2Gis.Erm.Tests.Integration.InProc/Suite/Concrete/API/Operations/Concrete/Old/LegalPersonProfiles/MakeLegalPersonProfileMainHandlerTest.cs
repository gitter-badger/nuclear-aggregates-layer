using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.LegalPersonProfiles;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Old.LegalPersonProfiles
{
    public class MakeLegalPersonProfileMainHandlerTest : UseModelEntityHandlerTestBase<LegalPersonProfile, MakeLegalPersonProfileMainRequest, EmptyResponse>
    {
        public MakeLegalPersonProfileMainHandlerTest(IPublicService publicService, IAppropriateEntityProvider<LegalPersonProfile> appropriateEntityProvider)
            : base(publicService, appropriateEntityProvider)
        {
        }

        protected override FindSpecification<LegalPersonProfile> ModelEntitySpec
        {
            get { return Specs.Find.ActiveAndNotDeleted<LegalPersonProfile>() && new FindSpecification<LegalPersonProfile>(lpp => !lpp.IsMainProfile); }
        }

        protected override bool TryCreateRequest(LegalPersonProfile modelEntity, out MakeLegalPersonProfileMainRequest request)
        {
            request = new MakeLegalPersonProfileMainRequest
                {
                    LegalPersonProfileId = modelEntity.Id
                };

            return true;
        }
    }
}
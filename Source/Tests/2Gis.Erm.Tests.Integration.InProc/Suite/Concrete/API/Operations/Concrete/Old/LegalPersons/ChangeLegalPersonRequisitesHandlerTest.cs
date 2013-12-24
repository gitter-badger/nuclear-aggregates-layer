using DoubleGis.Erm.BLFlex.API.Operations.Global.Russia.Operations.Concrete.Old.LegalPersons;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Old.LegalPersons
{
    public class ChangeLegalPersonRequisitesHandlerTest : UseModelEntityHandlerTestBase<LegalPerson, ChangeLegalPersonRequisitesRequest, EmptyResponse>
    {
        public ChangeLegalPersonRequisitesHandlerTest(IPublicService publicService, IAppropriateEntityProvider<LegalPerson> appropriateEntityProvider)
            : base(publicService, appropriateEntityProvider)
        {
        }

        protected override bool TryCreateRequest(LegalPerson modelEntity, out ChangeLegalPersonRequisitesRequest request)
        {
            request = new ChangeLegalPersonRequisitesRequest
                {
                    LegalPersonId = modelEntity.Id,
                    Inn = modelEntity.Inn,
                    Kpp = modelEntity.Kpp,
                    LegalAddress = modelEntity.LegalAddress,
                    LegalName = modelEntity.LegalName,
                    LegalPersonType = (LegalPersonType)modelEntity.LegalPersonTypeEnum,
                    PassportNumber = modelEntity.PassportNumber,
                    PassportSeries = modelEntity.PassportSeries,
                    RegistrationAddress = modelEntity.RegistrationAddress,
                    ShortName = modelEntity.ShortName
                };

            return true;
        }
    }
}
using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLFlex.API.Operations.Global.Russia.Operations.Concrete.Old.LegalPersons;
using DoubleGis.Erm.BLFlex.API.Operations.Global.Ukraine.Operations.Concrete.Old.LegalPersons;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Old.LegalPersons
{
    public class ChangeLegalPersonRequisitesHandlerTest : UseModelEntityHandlerTestBase<LegalPerson, Request, EmptyResponse>
    {
        private readonly IReadOnlyDictionary<Type, Func<LegalPerson, Request>> _requestFactories = 
            new Dictionary<Type, Func<LegalPerson, Request>>
                {
                    {typeof(IRussiaAdapted), CreateGenericRequest},
                    {typeof(IUkraineAdapted), CreateUkraineRequest}
                };

        private readonly IBusinessModelSettings _businessModelSettings;

        public ChangeLegalPersonRequisitesHandlerTest(
            IBusinessModelSettings businessModelSettings,
            IPublicService publicService, 
            IAppropriateEntityProvider<LegalPerson> appropriateEntityProvider)
            : base(publicService, appropriateEntityProvider)
        {
            _businessModelSettings = businessModelSettings;
        }

        protected override bool TryCreateRequest(LegalPerson modelEntity, out Request request)
        {
            request = null;
            Func<LegalPerson, Request> requestFactory;
            if (!_requestFactories.TryGetValue(_businessModelSettings.BusinessModelIndicator, out requestFactory))
            {
                return false;
            }

            request = requestFactory(modelEntity);
            return true;
        }

        private static ChangeLegalPersonRequisitesRequest CreateGenericRequest(LegalPerson modelEntity)
        {
            return new ChangeLegalPersonRequisitesRequest
            {
                LegalPersonId = modelEntity.Id,
                Inn = modelEntity.Inn,
                Kpp = modelEntity.Kpp,
                LegalAddress = modelEntity.LegalAddress,
                LegalName = modelEntity.LegalName,
                LegalPersonType = modelEntity.LegalPersonTypeEnum,
                PassportNumber = modelEntity.PassportNumber,
                PassportSeries = modelEntity.PassportSeries,
                RegistrationAddress = modelEntity.RegistrationAddress,
                ShortName = modelEntity.ShortName
            };
        }

        private static UkraineChangeLegalPersonRequisitesRequest CreateUkraineRequest(LegalPerson modelEntity)
        {
            return new UkraineChangeLegalPersonRequisitesRequest
            {
                LegalPersonId = modelEntity.Id,
                Ipn = modelEntity.Inn,
                Egrpou = "123652569852",
                LegalAddress = modelEntity.LegalAddress,
                LegalName = modelEntity.LegalName,
                LegalPersonType = LegalPersonType.LegalPerson,
                TaxationType = TaxationType.WithoutVat
            };
        }
    }
}
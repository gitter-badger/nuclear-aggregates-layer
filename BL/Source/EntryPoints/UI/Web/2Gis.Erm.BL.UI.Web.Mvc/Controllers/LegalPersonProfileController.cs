using System.Web.Mvc;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.LegalPersonProfiles;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Settings;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
using NuClear.Security.API.UserContext;

using NuClear.Tracing.API;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Controllers
{
    public sealed class LegalPersonProfileController : ControllerBase
    {
        private readonly IPublicService _publicService;

        public LegalPersonProfileController(IMsCrmSettings msCrmSettings,
                                            IAPIOperationsServiceSettings operationsServiceSettings,
                                            IAPISpecialOperationsServiceSettings specialOperationsServiceSettings,
                                            IAPIIdentityServiceSettings identityServiceSettings,
                                            IUserContext userContext,
                                            ITracer tracer,
                                            IGetBaseCurrencyService getBaseCurrencyService,
                                            IPublicService publicService)
            : base(msCrmSettings, operationsServiceSettings, specialOperationsServiceSettings, identityServiceSettings, userContext, tracer, getBaseCurrencyService)
        {
            _publicService = publicService;
        }

        [HttpPost]
        public void MakeProfileMain(long profileId)
        {
            _publicService.Handle(new MakeLegalPersonProfileMainRequest { LegalPersonProfileId = profileId });
        }
    }
}

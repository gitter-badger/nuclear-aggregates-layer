﻿using System.Web.Mvc;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.LegalPersonProfiles;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Settings;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Controllers
{
    public sealed class LegalPersonProfileController : ControllerBase
    {
        private readonly IPublicService _publicService;

        public LegalPersonProfileController(IMsCrmSettings msCrmSettings,
                                            IUserContext userContext,
                                            ICommonLog logger,
                                            IPublicService publicService,
                                            IAPIOperationsServiceSettings operationsServiceSettings,
                                            IAPISpecialOperationsServiceSettings specialOperationsServiceSettings,
                                            IGetBaseCurrencyService getBaseCurrencyService)
            : base(msCrmSettings,
                   userContext,
                   logger,
                   operationsServiceSettings,
                   specialOperationsServiceSettings,
                   getBaseCurrencyService)
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

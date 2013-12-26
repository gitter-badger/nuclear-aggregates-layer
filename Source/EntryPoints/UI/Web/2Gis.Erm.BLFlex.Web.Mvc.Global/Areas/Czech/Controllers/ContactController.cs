using System;

using DoubleGis.Erm.BL.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Web.Mvc.Utils;
using DoubleGis.Erm.UI.Web.Mvc.Controllers.Base;

namespace DoubleGis.Erm.BLFlex.Web.Mvc.Global.Areas.Czech.Controllers
{
    // TODO {all, 25.12.2013}: странно все выглядит - похоже на эмуляцию локализованного enum + 2 разных entry в ресурсниках для фактически одного и тоже бизнес значения
    public sealed class ContactController : ControllerBase
    {
        public ContactController(IMsCrmSettings msCrmSettings,
                                      IUserContext userContext,
                                      ICommonLog logger,
                                      IAPIOperationsServiceSettings operationsServiceSettings,
                                      IGetBaseCurrencyService getBaseCurrencyService)
            : base(msCrmSettings,
                   userContext,
                   logger,
                   operationsServiceSettings,
                   getBaseCurrencyService)
        {
        }

        public JsonNetResult GetSalutations(string gender)
        {
            Gender g;
            string[] result;
            if (Enum.TryParse(gender, true, out g))
            {
                if (g == Gender.Male)
                {
                    result = new[] { BLResources.SalutationToMale, BLResources.SalutationToMale2 };
                }
                else if (g == Gender.Female)
                {
                    result = new[] { BLResources.SalutationToFemale, BLResources.SalutationToFemale2 };
                }
                else
                {
                    result = new string[0];
                }
            }
            else
            {
                result = new string[0];
            }

            return new JsonNetResult(result);
        }
    }
}
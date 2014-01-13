using System;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Areas.Cyprus.Controllers
{
    // FIXME {all, 25.12.2013}: странно все выглядит - похоже на эмуляцию локализованного enum + 2 разных entry в ресурсниках для фактически одного и тоже бизнес значения
    // нужно переделать избавившись от ContactController во всех areas, от ajax request при redndering карточки - значения которые возвращает этот контроллер, 
    // лучше передавать прямо во ViewModel, в виде map и использовать в js без дополнительных запросов на сервер
    [Obsolete]
    public sealed class ContactController : ControllerBase
    {
        public ContactController(
            IMsCrmSettings msCrmSettings,
            IUserContext userContext,
            ICommonLog logger, 
            IAPIOperationsServiceSettings operationsServiceSettings,
            IGetBaseCurrencyService getBaseCurrencyService)
        : base(
            msCrmSettings,
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
                // TODO: когда будет локализация, в зависимости от культуры подтягивать 
                // откуда-то список обращений для каждого пола.
                if (g == Gender.Male)
                {
                    result = new[] { BLResources.SalutationToMale };
                }
                else if (g == Gender.Female)
                {
                    result = new[] { BLResources.SalutationToFemale };
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
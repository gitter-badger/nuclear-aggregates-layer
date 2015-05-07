using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;

using DoubleGis.Erm.BLCore.API.Operations.Special.Dial;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Dial;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Common.Utils.Resources;

using NuClear.Security.API.UserContext;
using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLCore.WCF.Operations.Special.Dial
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
    public class DialApplicationService : IDialApplicationRestService 
    {
        private readonly ITracer _tracer;

        private readonly IDialOperationService _dialOperationService;

        public DialApplicationService(
            ITracer tracer,
            IUserContext userContext,
            IDialOperationService dialOperationService,
            IResourceGroupManager resourceGroupManager)
        {
            _tracer = tracer;
            _dialOperationService = dialOperationService;
            resourceGroupManager.SetCulture(userContext.Profile.UserLocaleInfo.UserCultureInfo);
        }

        public void Dial(string phone)
        {
            try
            {
                if (string.IsNullOrEmpty(phone))
                {
                    throw new ArgumentException(BLResources.IncorrectPhoneNumber);
                }

                _dialOperationService.Dial(phone);
            }
            catch (Exception ex)
            {
                _tracer.ErrorFormat(ex, "Error has occurred in {0}.", GetType().Name);
                throw new WebFaultException<DialErrorDescription>(new DialErrorDescription(ex.Message), HttpStatusCode.BadRequest);
            }
        }
    }
}

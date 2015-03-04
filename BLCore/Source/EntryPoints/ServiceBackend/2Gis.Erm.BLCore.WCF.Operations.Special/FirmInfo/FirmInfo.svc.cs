using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;

using DoubleGis.Erm.BLCore.API.Operations.Special.FirmInfo;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.FirmInfo;
using DoubleGis.Erm.Platform.API.Core.Exceptions;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.WCF.Operations.Special.FirmInfo
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
    public class FirmInfoApplicationService : IFirmInfoApplicationRestService
    {
        private readonly IGetFirmInfoService _getFirmInfoService;
        private readonly ITracer _tracer;

        public FirmInfoApplicationService(IGetFirmInfoService getFirmInfoService, ITracer tracer)
        {
            _getFirmInfoService = getFirmInfoService;
            _tracer = tracer;
        }

        public IEnumerable<FirmInfoDto> Execute(IEnumerable<FirmGuidDto> firmIds)
        {
            try
            {
                return _getFirmInfoService.GetFirmInfosByCrmIds(firmIds.Select(x => x.Id));
            }
            catch (BusinessLogicException ex)
            {
                _tracer.ErrorFormat(ex, "Error has occured in {0}", GetType().Name);
                throw new WebFaultException<FirmInfoOperationErrorDescription>(new FirmInfoOperationErrorDescription(ex.Message), HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                _tracer.ErrorFormat(ex, "Error has occured in {0}", GetType().Name);
                throw new WebFaultException<FirmInfoOperationErrorDescription>(new FirmInfoOperationErrorDescription(ex.Message), HttpStatusCode.InternalServerError);
            }
        }
    }
}
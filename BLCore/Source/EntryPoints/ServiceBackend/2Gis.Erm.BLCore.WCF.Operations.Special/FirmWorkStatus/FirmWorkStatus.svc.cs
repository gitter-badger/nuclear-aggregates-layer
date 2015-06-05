using System;
using System.ServiceModel;

using DoubleGis.Erm.BLCore.API.Operations.Special.FirmInfo;
using DoubleGis.Erm.BLCore.API.Operations.Special.FirmWorkStatus;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.FirmWorkStatus;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLCore.WCF.Operations.Special.FirmWorkStatus
{
    public class FirmWorkStatusApplicationService : IFirmWorkStatusApplicationService
    {
        private readonly IGetFirmWorkStatusOperationService _firmWorkStatusOperationService;
        private readonly ITracer _tracer;

        public FirmWorkStatusApplicationService(IGetFirmWorkStatusOperationService firmWorkStatusOperationService, ITracer tracer)
        {
            _firmWorkStatusOperationService = firmWorkStatusOperationService;
            _tracer = tracer;
        }

        public API.Operations.Special.FirmWorkStatus.FirmWorkStatus GetFirmWorkStatus(long firmId)
        {
            try
            {
                return _firmWorkStatusOperationService.GetFirmWorkStatus(firmId);
            }
            catch (Exception e)
            {
                _tracer.ErrorFormat(e, "An error has occured in {0}", GetType().Name);
                throw new FaultException<FirmWorkStatusErrorDescription>(new FirmWorkStatusErrorDescription
                                                                             {
                                                                                 Message = e.Message
                                                                             });
                
            }
        }
    }
}
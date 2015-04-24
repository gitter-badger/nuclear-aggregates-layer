using System;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;

using DoubleGis.Erm.BLCore.API.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Remote.DownloadBinary;

using NuClear.Model.Common.Entities;
using NuClear.ResourceUtilities;
using NuClear.Security.API.UserContext;
using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLCore.WCF.Operations
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
    public class DownloadBinaryApplicationService : IDownloadBinaryApplicationRestService
    {
        private readonly ITracer _tracer;
        private readonly IOperationServicesManager _operationServicesManager;

        public DownloadBinaryApplicationService(ITracer tracer,
                                                IOperationServicesManager operationServicesManager,
                                                IUserContext userContext,
                                                IResourceGroupManager resourceGroupManager)
        {
            _tracer = tracer;
            _operationServicesManager = operationServicesManager;

            resourceGroupManager.SetCulture(userContext.Profile.UserLocaleInfo.UserCultureInfo);
        }

        public Stream Execute(string specifiedEntityName, string specifiedBinaryId)
        {
            IEntityType entityName = EntityType.Instance.None();
            try
            {
                if (!EntityType.Instance.TryParse(specifiedEntityName, out entityName))
                {
                    throw new ArgumentException("Entity Name cannot be parsed");
                }

                long binaryId;
                if (!long.TryParse(specifiedBinaryId, out binaryId))
                {
                    throw new ArgumentException("Binary Id cannot be parsed");
                }

                var downloadFileService = _operationServicesManager.GetDownloadFileService(entityName);
                var streamResponse = downloadFileService.DownloadFile(binaryId);

                WebOperationContext.Current.SetBinaryResponseProperties(streamResponse);
                return streamResponse.Stream;
            }
            catch (Exception ex)
            {
                _tracer.ErrorFormat(ex, "Error has occured in {0}", GetType().Name);
                throw new WebFaultException<DownloadBinaryOperationErrorDescription>(new DownloadBinaryOperationErrorDescription(entityName, ex.Message),
                                                                                     HttpStatusCode.BadRequest);
            }
        }
    }
}

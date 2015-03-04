using System;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;

using DoubleGis.Erm.BLCore.API.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Remote.DownloadBinary;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils.Resources;
using DoubleGis.Erm.Platform.Model.Entities;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.WCF.Operations
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
    public class DownloadBinaryApplicationService : IDownloadBinaryApplicationRestService
    {
        private readonly ITracer _logger;
        private readonly IOperationServicesManager _operationServicesManager;

        public DownloadBinaryApplicationService(ITracer logger,
                                                IOperationServicesManager operationServicesManager,
                                                IUserContext userContext,
                                                IResourceGroupManager resourceGroupManager)
        {
            _logger = logger;
            _operationServicesManager = operationServicesManager;

            resourceGroupManager.SetCulture(userContext.Profile.UserLocaleInfo.UserCultureInfo);
        }

        public Stream Execute(string specifiedEntityName, string specifiedBinaryId)
        {
            var entityName = EntityName.None;
            try
            {
                if (!Enum.TryParse(specifiedEntityName, out entityName))
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
                _logger.ErrorFormat(ex, "Error has occured in {0}", GetType().Name);
                throw new WebFaultException<DownloadBinaryOperationErrorDescription>(new DownloadBinaryOperationErrorDescription(entityName, ex.Message),
                                                                                     HttpStatusCode.BadRequest);
            }
        }
    }
}

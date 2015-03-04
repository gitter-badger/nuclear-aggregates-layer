using System;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;

using DoubleGis.Erm.BLCore.API.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Generic.File;
using DoubleGis.Erm.BLCore.API.Operations.Remote.UploadBinary;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils.Resources;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.WCF.Infrastructure;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.WCF.Operations
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
    public class UploadBinaryApplicationService : IUploadBinaryApplicationRestService
    {
        private readonly ICommonLog _logger;
        private readonly IOperationServicesManager _operationServicesManager;

        public UploadBinaryApplicationService(ICommonLog logger,
                                              IOperationServicesManager operationServicesManager,
                                              IUserContext userContext,
                                              IResourceGroupManager resourceGroupManager)
        {
            _logger = logger;
            _operationServicesManager = operationServicesManager;

            resourceGroupManager.SetCulture(userContext.Profile.UserLocaleInfo.UserCultureInfo);
        }

        public UploadFileResult Execute(string specifiedEntityName, string specifiedEntityId, string specifiedBinaryId, Stream multipartStream)
        {
            var entityName = EntityName.None;
            try
            {
                if (!Enum.TryParse(specifiedEntityName, out entityName))
                {
                    throw new ArgumentException("Entity Name cannot be parsed");
                }

                long entityId;
                if (!long.TryParse(specifiedEntityId, out entityId))
                {
                    entityId = 0;
                }

                long binaryId;
                if (!long.TryParse(specifiedBinaryId, out binaryId))
                {
                    binaryId = 0;
                }

                var streamWrapper = new MultipartStreamWrapper(multipartStream);
                if (!streamWrapper.IsParsedSuccessfully)
                {
                    throw new ArgumentException("Multipart stream cannot be read");
                }

                var uploadFileParams = new UploadFileParams
                    {
                        EntityId = entityId,
                        FileId = binaryId,
                        FileName = streamWrapper.Filename,
                        Content = new MemoryStream(streamWrapper.BinaryContents),
                        ContentType = streamWrapper.ContentType,
                        ContentLength = streamWrapper.BinaryContents.Length
                    };

                var uploadFileService = _operationServicesManager.GetUploadFileService(entityName);
                var uploadFileResult = uploadFileService.UploadFile(uploadFileParams);

                return uploadFileResult;
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "Error has occured in {0}", GetType().Name);
                throw new WebFaultException<UploadBinaryOperationErrorDescription>(new UploadBinaryOperationErrorDescription(entityName, ex.Message), HttpStatusCode.BadRequest);
            }
        }
    }
}

using System.IO;

using DoubleGis.Erm.BL.API.Operations.Concrete.AdvertisementElements;
using DoubleGis.Erm.BLCore.API.Aggregates.Advertisements;
using DoubleGis.Erm.BLCore.API.Aggregates.Advertisements.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Advertisements.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.File;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.BL.Operations.Generic.File.AdvertisementElements
{
    public sealed class UploadAdvertisementElementFileOperationService : IUploadFileGenericService<AdvertisementElement>
    {
        private readonly IAdvertisementReadModel _advertisementReadModel;
        private readonly INotifyAboutAdvertisementElementFileChangedOperationService _notifyAboutAdvertisementElementFileChangedOperationService;
        private readonly IAdvertisementUploadElementFileAggregateService _uploadElementFileAggregateService;
        private readonly IUploadingAdvertisementElementValidator _uploadingAdvertisementElementValidator;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IUserContext _userContext;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ITracer _tracer;

        public UploadAdvertisementElementFileOperationService(
            IAdvertisementReadModel advertisementReadModel,
            INotifyAboutAdvertisementElementFileChangedOperationService notifyAboutAdvertisementElementFileChangedOperationService,
            IAdvertisementUploadElementFileAggregateService uploadElementFileAggregateService,
            IUploadingAdvertisementElementValidator uploadingAdvertisementElementValidator,
            ISecurityServiceFunctionalAccess functionalAccessService,
            IUserContext userContext,
            IOperationScopeFactory scopeFactory,
            ITracer tracer)
        {
            _advertisementReadModel = advertisementReadModel;
            _notifyAboutAdvertisementElementFileChangedOperationService = notifyAboutAdvertisementElementFileChangedOperationService;
            _uploadElementFileAggregateService = uploadElementFileAggregateService;
            _uploadingAdvertisementElementValidator = uploadingAdvertisementElementValidator;
            _functionalAccessService = functionalAccessService;
            _userContext = userContext;
            _scopeFactory = scopeFactory;
            _tracer = tracer;
        }

        public UploadFileResult UploadFile(UploadFileParams uploadFileParams)
        {
            if (uploadFileParams.EntityId == 0)
            {
                // TODO {all, 04.03.2014}: Выбрасывать очень конкретные бизнес-значимые исключения, а их логику их объединения и отображения (локализации) реализовать в слое представления
                throw new BusinessLogicException(BLResources.CantUploadFileForNewAdvertisementElement);
            }

            using (var scope = _scopeFactory.CreateSpecificFor<UploadIdentity, AdvertisementElement>())
            {
                var validationState = _advertisementReadModel.GetAdvertisementElementValidationState(uploadFileParams.EntityId);
                if (validationState.NeedsValidation &&
                    (AdvertisementElementStatusValue)validationState.CurrentStatus.Status != AdvertisementElementStatusValue.Draft &&
                    !_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.AdvertisementVerification, _userContext.Identity.Code))
                {
                    throw new EditingNotDraftAdvertisementElementException(string.Format(BLResources.NonDraftAdvertisementElementEditing,
                                                                                         uploadFileParams.EntityId));
                }

                var advertisementInfo = _advertisementReadModel.GetAdvertisementInfoForElement(uploadFileParams.EntityId);
                if (advertisementInfo.IsDummy)
                {
                    if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.EditDummyAdvertisement, _userContext.Identity.Code))
                    {
                        // TODO {all, 04.03.2014}: Выбрасывать очень конкретные бизнес-значимые исключения, а их логику их объединения и отображения (локализации) реализовать в слое представления
                        throw new BusinessLogicException(BLResources.YouHaveNoPrivelegeToEditDummyAdvertisement);
                    }

                    if (advertisementInfo.IsPublished)
                    {
                        // TODO {all, 04.03.2014}: Выбрасывать очень конкретные бизнес-значимые исключения, а их логику их объединения и отображения (локализации) реализовать в слое представления
                        throw new BusinessLogicException(BLResources.CantEditDummyAdvertisementWithPublishedTemplate);
                    }
                }

                uploadFileParams.FileName = Path.GetFileName(uploadFileParams.FileName);
                _uploadingAdvertisementElementValidator.Validate(advertisementInfo.ElementTemplate, uploadFileParams.FileName, uploadFileParams.Content);

                var result = _uploadElementFileAggregateService.UploadFile(advertisementInfo.Element,
                                                                           new UploadFileParams<AdvertisementElement>(uploadFileParams));
                if (!advertisementInfo.IsDummy)
                {
                    _notifyAboutAdvertisementElementFileChangedOperationService.Notify(uploadFileParams.EntityId);
                }
                else
                {
                    // Отредактирована заглушка РМ. Уведомление не отправляем
                    _tracer.Info("Отредактирована заглушка РМ. Уведомление не отправляем");
                }

                scope.Updated<AdvertisementElement>(uploadFileParams.EntityId)
                     .Complete();

                return result;
            }
        }
    }
}
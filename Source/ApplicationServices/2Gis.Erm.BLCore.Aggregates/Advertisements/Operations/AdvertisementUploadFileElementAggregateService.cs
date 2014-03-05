using System;
using System.IO;

using DoubleGis.Erm.BLCore.API.Operations.Generic.File;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Advertisements.Operations
{
    public sealed class AdvertisementUploadFileElementAggregateService : IAdvertisementUploadElementFileAggregateService
    {
        private readonly ISecureRepository<AdvertisementElement> _secureAdvertisementElementRepository;
        private readonly IRepository<FileWithContent> _fileRepository;
        private readonly IIdentityProvider _identityProvider;
        private readonly IUserContext _userContext;
        private readonly IOperationScopeFactory _scopeFactory;

        public AdvertisementUploadFileElementAggregateService(
            ISecureRepository<AdvertisementElement> secureAdvertisementElementRepository,
            IRepository<FileWithContent> fileRepository,
            IIdentityProvider identityProvider,
            IUserContext userContext,
            IOperationScopeFactory scopeFactory)
        {
            _secureAdvertisementElementRepository = secureAdvertisementElementRepository;
            _fileRepository = fileRepository;
            _identityProvider = identityProvider;
            _userContext = userContext;
            _scopeFactory = scopeFactory;
        }

        public UploadFileResult UploadFile(AdvertisementElement advertisementElement, UploadFileParams<AdvertisementElement> uploadFileParams)
        {
            var file = new FileWithContent
            {
                Id = uploadFileParams.FileId,
                ContentType = uploadFileParams.ContentType,
                ContentLength = uploadFileParams.ContentLength,
                Content = uploadFileParams.Content,
                FileName = Path.GetFileName(uploadFileParams.FileName)
            };

            using (var scope = _scopeFactory.CreateOrUpdateOperationFor(file))
            {
                if (file.IsNew())
                {
                    _identityProvider.SetFor(file);
                    _fileRepository.Add(file);
                    scope.Added<FileWithContent>(file.Id);
                }
                else
                {
                    _fileRepository.Update(file);
                    scope.Updated<FileWithContent>(file.Id);
                }

                _fileRepository.Save();

                if (advertisementElement != null)
                {
                    advertisementElement.ModifiedOn = DateTime.UtcNow;
                    advertisementElement.ModifiedBy = _userContext.Identity.Code;
                    advertisementElement.Status = (int)AdvertisementElementStatus.NotValidated;
                    
                    _secureAdvertisementElementRepository.Update(advertisementElement);
                    scope.Updated<AdvertisementElement>(advertisementElement.Id);

                    _secureAdvertisementElementRepository.Save();
                }

                scope.Complete();
            }

            return new UploadFileResult
            {
                ContentType = file.ContentType,
                ContentLength = file.ContentLength,
                FileName = file.FileName,
                FileId = file.Id
            };
        }
    }
}

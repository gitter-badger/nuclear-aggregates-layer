using System;
using System.IO;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Generic.File;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;

using NuClear.Security.API.UserContext;

using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.Readings;
using NuClear.Storage.Writings;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.BLCore.Aggregates.Orders
// ReSharper restore CheckNamespace
{
    public sealed class BargainRepository : IBargainRepository
    {
        private readonly IFinder _finder;
        private readonly IRepository<BargainFile> _bargainFileGenericRepository;
        private readonly IRepository<FileWithContent> _fileRepository;
        private readonly IUserContext _userContext;
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IFileContentFinder _fileContentFinder;

        public BargainRepository(
            IFinder finder,
            IRepository<BargainFile> bargainFileGenericRepository, 
            IRepository<FileWithContent> fileRepository, 
            IUserContext userContext, 
            IIdentityProvider identityProvider, 
            IFileContentFinder fileContentFinder, 
            IOperationScopeFactory scopeFactory)
        {
            _fileRepository = fileRepository;
            _userContext = userContext;
            _identityProvider = identityProvider;
            _scopeFactory = scopeFactory;
            _fileContentFinder = fileContentFinder;
            _finder = finder;
            _bargainFileGenericRepository = bargainFileGenericRepository;
        }

        public void CreateOrUpdate(BargainFile entity)
        {
            using (var scope = _scopeFactory.CreateOrUpdateOperationFor(entity))
            {
                if (entity.IsNew())
                {
                    _identityProvider.SetFor(entity);
                    _bargainFileGenericRepository.Add(entity);
                    scope.Added<BargainFile>(entity.Id);
                }
                else
                {
                    _bargainFileGenericRepository.Update(entity);
                    scope.Updated<BargainFile>(entity.Id);
                }

                _bargainFileGenericRepository.Save();
                scope.Complete();
            }
        }

        StreamResponse IDownloadFileAggregateRepository<BargainFile>.DownloadFile(DownloadFileParams<BargainFile> downloadFileParams)
        {
            var file = _fileContentFinder.Find(Specs.Find.ById<FileWithContent>(downloadFileParams.FileId)).Single();
            return new StreamResponse { FileName = file.FileName, ContentType = file.ContentType, Stream = file.Content };
        }

        UploadFileResult IUploadFileAggregateRepository<BargainFile>.UploadFile(UploadFileParams<BargainFile> uploadFileParams)
        {
            if (uploadFileParams.Content != null && uploadFileParams.Content.Length > 10485760)
            {
                throw new BusinessLogicException(BLResources.FileSizeMustBeLessThan10MB);
            }

            var file = new FileWithContent
            {
                Id = uploadFileParams.FileId,
                ContentType = uploadFileParams.ContentType,
                ContentLength = uploadFileParams.ContentLength,
                Content = uploadFileParams.Content,
                FileName = Path.GetFileName(uploadFileParams.FileName)
            };

            using (var operationScope = _scopeFactory.CreateOrUpdateOperationFor(file))
            {
                if (file.IsNew())
                {
                    _identityProvider.SetFor(file);
                    _fileRepository.Add(file);
                    operationScope.Added<FileWithContent>(file.Id);
                }
                else
                {
                    _fileRepository.Update(file);
                    operationScope.Updated<FileWithContent>(file.Id);
                }

                var bargainFile = _finder.Find(Specs.Find.ByFileId<BargainFile>(uploadFileParams.FileId)).Top();
                if (bargainFile != null)
                {
                    bargainFile.ModifiedOn = DateTime.UtcNow;
                    bargainFile.ModifiedBy = _userContext.Identity.Code;

                    _bargainFileGenericRepository.Update(bargainFile);
                    _bargainFileGenericRepository.Save();
                    operationScope.Updated<BargainFile>(bargainFile.Id);
                }

                operationScope.Complete();
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

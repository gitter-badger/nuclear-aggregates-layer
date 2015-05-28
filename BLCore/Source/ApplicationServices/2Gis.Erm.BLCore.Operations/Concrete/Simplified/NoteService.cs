using System;
using System.IO;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified;
using DoubleGis.Erm.BLCore.API.Operations.Generic.File;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Storage;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Simplified
{
    public class NoteService : INoteService
    {
        private readonly IFinder _finder;
        private readonly IFileContentFinder _fileContentFinder;

        private readonly IRepository<Note> _noteGenericRepository;
        private readonly IRepository<FileWithContent> _fileRepository;
        private readonly IUserContext _userContext;
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _scopeFactory;

        public NoteService(
            IFinder finder,
            IFileContentFinder fileContentFinder,
            IRepository<Note> noteGenericRepository,
            IRepository<FileWithContent> fileRepository, 
            IUserContext userContext, 
            IIdentityProvider identityProvider, 
            IOperationScopeFactory scopeFactory)
        {
            _noteGenericRepository = noteGenericRepository;
            _fileRepository = fileRepository;
            _userContext = userContext;
            _identityProvider = identityProvider;
            _scopeFactory = scopeFactory;
            _fileContentFinder = fileContentFinder;
            _finder = finder;
            _fileContentFinder = fileContentFinder;
        }

        public void CreateOrUpdate(Note note, IEntityType parentEntityName, long ownerCode)
        {
            if (parentEntityName.Equals(EntityType.Instance.None()))
            {
                throw new BusinessLogicException(BLResources.EntityNameNotSpecified);
            }

            note.ParentType = parentEntityName.Id;

            if (ownerCode == 0)
            {
                throw new BusinessLogicException(BLResources.OwnerNotSpecified);
            }

            note.OwnerCode = ownerCode;

            if (note.IsNew())
            {
                _identityProvider.SetFor(note);
                _noteGenericRepository.Add(note);
            }
            else
            {
                _noteGenericRepository.Update(note);
            }

            _noteGenericRepository.Save();
        }

        public StreamResponse DownloadFile(DownloadFileParams<Note> downloadFileParams)
        {
            var file = _fileContentFinder.Find(Specs.Find.ById<FileWithContent>(downloadFileParams.FileId)).Single();
            return new StreamResponse { FileName = file.FileName, ContentType = file.ContentType, Stream = file.Content };
        }

        public UploadFileResult UploadFile(UploadFileParams<Note> uploadFileParams)
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

                var note = _finder.Find(Specs.Find.ByOptionalFileId<Note>(uploadFileParams.FileId)).Top();
                if (note != null)
                {
                    note.ModifiedOn = DateTime.UtcNow;
                    note.ModifiedBy = _userContext.Identity.Code;

                    _noteGenericRepository.Update(note);
                    _noteGenericRepository.Save();
                    operationScope.Updated<Note>(note.Id);
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
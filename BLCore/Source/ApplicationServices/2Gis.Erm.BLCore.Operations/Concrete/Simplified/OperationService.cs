using System;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Simplified
{
    public sealed class OperationService : IOperationService
    {
        private readonly IFinder _finder;
        private readonly IFileContentFinder _fileContentFinder;

        private readonly IRepository<FileWithContent> _fileRepository;
        private readonly IRepository<Operation> _operationRepository;
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public OperationService(IRepository<FileWithContent> fileRepository,
                                IRepository<Operation> operationRepository,
                                IIdentityProvider identityProvider,
                                IFinder finder,
                                IFileContentFinder fileContentFinder,
                                IOperationScopeFactory operationScopeFactory)
        {
            _fileRepository = fileRepository;
            _operationRepository = operationRepository;
            _identityProvider = identityProvider;
            _finder = finder;
            _fileContentFinder = fileContentFinder;
            _operationScopeFactory = operationScopeFactory;
        }

        public void Add(Operation operation)
        {
            _identityProvider.SetFor(operation);
            _operationRepository.Add(operation);
            _operationRepository.Save();
        }

        public void Update(Operation operation)
        {
            _operationRepository.Update(operation);
            _operationRepository.Save();
        }

        public void FinishOperation(Operation operation, byte[] logContent, string logfileName, string contentType)
        {
            using (MemoryStream ms = (logContent != null && logContent.Length > 0) ? new MemoryStream(logContent) : null)
            {
                FinishOperation(operation, ms, logfileName, contentType);
            }
        }

        public void FinishOperation(Operation operation, string operationLog, string logfileName, string contentType = MediaTypeNames.Text.Plain)
        {
            if (!string.IsNullOrEmpty(operationLog))
            {
                var stream = new MemoryStream(operationLog.Length);
                using (var sw = new StreamWriter(stream, Encoding.UTF8))
                {
                    sw.Write(operationLog);
                    sw.Flush();
                    stream.Seek(0, SeekOrigin.Begin);

                    // Этот вызов должен быть внутри using'а, т.к. StreamWriter при закрытии себя закрывает MemoryStream.
                    FinishOperation(operation, stream, logfileName, MediaTypeNames.Text.Plain);
                }
            }
            else
            {
                FinishOperation(operation, null as Stream, logfileName, MediaTypeNames.Text.Plain);
            }
        }

        public void FinishOperation(Operation operation, Stream newData, string fileName, string contentType)
        {
            if (operation.Id > 0)
            {
                using (var scope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, Operation>())
                {
                    if (newData != null)
                    {
                        operation.LogFileId = AddFileWithContent(newData, fileName, contentType);
                    }

                    Update(operation);
                    scope.Updated(operation);
                    scope.Complete();
                }
            }
            else
            {
                using (var scope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, Operation>())
                {
                    if (newData != null)
                    {
                        operation.LogFileId = AddFileWithContent(newData, fileName, contentType);
                    }

                    Add(operation);
                    scope.Added(operation);
                    scope.Complete();
                }
            }
        }

        public FileWithContent GetLogForOperation(Guid operationId)
        {
            var fileShortInfo =
                _finder.Find<Operation>(x => x.Guid == operationId).Select(x => new { x.LogFileId, x.File.ContentType, x.File.FileName }).FirstOrDefault();

            if (fileShortInfo != null && fileShortInfo.LogFileId.HasValue)
            {
                var fileWithContent = _fileContentFinder.Find(Specs.Find.ById<FileWithContent>(fileShortInfo.LogFileId.Value)).Single();
                return fileWithContent;
            }

            return null;
        }

        private long AddFileWithContent(Stream newData, string fileName, string contentType)
        {
            var fileWithContent = new FileWithContent
            {
                ContentType = contentType,
                ContentLength = newData.Length,
                FileName = fileName,
                Content = newData
            };

            using (var scope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, FileWithContent>())
            {
                _identityProvider.SetFor(fileWithContent);
                _fileRepository.Add(fileWithContent);
                scope.Added(fileWithContent);   
            }

            return fileWithContent.Id;
        }
    }
}

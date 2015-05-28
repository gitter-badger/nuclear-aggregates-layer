using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Transactions;

using DoubleGis.Erm.BLCore.API.Operations.Generic.File;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage;
using NuClear.Storage.Futures.Queryable;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLCore.Operations.Generic.File
{
    public class FileService : IFileService
    {
        private readonly IFinder _finder;
        private readonly IFileContentFinder _fileContentFinder;
        private readonly IRepository<FileWithContent> _fileContentGenericRepository;
        private readonly IIdentityProvider _identityProvider;

        public FileService(IRepository<FileWithContent> fileContentGenericRepository, IIdentityProvider identityProvider, IFinder finder, IFileContentFinder fileContentFinder)
        {
            _fileContentGenericRepository = fileContentGenericRepository;
            _identityProvider = identityProvider;
            _finder = finder;
            _fileContentFinder = fileContentFinder;
        }

        public void DeleteOrhpanFiles()
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                var orphanFilesIds = GetOrphanFilesIds();
                foreach (var id in orphanFilesIds)
                {
                    var fakeFile = new FileWithContent { Id = id };
                    _fileContentGenericRepository.Delete(fakeFile);
                }

                transaction.Complete();
            }
        }

        public FileWithContent GetFileById(long fileId)
        {
            var file = _fileContentFinder.Find(Specs.Find.ById<FileWithContent>(fileId)).FirstOrDefault();
            return file;
        }

        public Stream GetFileContent(long fileId)
        {
            var file = _fileContentFinder.Find(Specs.Find.ById<FileWithContent>(fileId)).First();
            return file.Content;
        }

        public void Add(FileWithContent file)
        {
            _identityProvider.SetFor(file);
            _fileContentGenericRepository.Add(file);
        }

        private IEnumerable<long> GetOrphanFilesIds()
        {
            return _finder.Find(new FindSpecification<DoubleGis.Erm.Platform.Model.Entities.Erm.File>(
                                    x => !x.AdvertisementElements.Any() &&
                                         !x.BargainFiles.Any() &&
                                         !x.OrderFiles.Any() &&
                                         !x.Notes.Any() &&
                                         !x.ReleaseInfos.Any() &&
                                         !x.LocalMessages.Any() &&
                                         !x.NotificationEmailsAttachments.Any() &&
                                         !x.Operations.Any() &&
                                         !x.PrintFormTemplates.Any() &&
                                         !x.ThemeTemplates.Any() &&
                                         !x.Themes.Any()))
                          .Map(q => q.Select(x => x.Id))
                          .Many();
        }
    }
}

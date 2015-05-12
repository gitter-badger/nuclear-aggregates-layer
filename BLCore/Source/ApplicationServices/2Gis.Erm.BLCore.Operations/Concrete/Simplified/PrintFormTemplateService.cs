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
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Simplified
{
    public sealed class PrintFormTemplateService : IPrintFormTemplateService
    {
        private readonly IFinder _finder;
        private readonly IFileContentFinder _fileContentFinder;
        private readonly IRepository<FileWithContent> _fileRepository;
        private readonly IRepository<PrintFormTemplate> _printFormTemplateRepository;
        
        private readonly IUserContext _userContext;
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _scopeFactory;

        public PrintFormTemplateService(
            IRepository<FileWithContent> fileRepository,
            IRepository<PrintFormTemplate> printFormTemplateRepository, 
            IUserContext userContext, 
            IFinder finder, 
            IIdentityProvider identityProvider, 
            IOperationScopeFactory scopeFactory,
            IFileContentFinder fileContentFinder)
        {
            _fileRepository = fileRepository;
            _printFormTemplateRepository = printFormTemplateRepository;
            _userContext = userContext;
            _finder = finder;
            _identityProvider = identityProvider;
            _scopeFactory = scopeFactory;
            _fileContentFinder = fileContentFinder;
        }

        public UploadFileResult UploadPrintFormTemplateFile(UploadFileParams<PrintFormTemplate> uploadFileParams)
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

                var printFormTemplate = _finder.Find(Specs.Find.ByFileId<PrintFormTemplate>(uploadFileParams.FileId)).FirstOrDefault();
                if (printFormTemplate != null)
                {
                    printFormTemplate.ModifiedOn = DateTime.UtcNow;
                    printFormTemplate.ModifiedBy = _userContext.Identity.Code;

                    _printFormTemplateRepository.Update(printFormTemplate);
                    _printFormTemplateRepository.Save();
                    operationScope.Updated<PrintFormTemplate>(printFormTemplate.Id);
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

        public StreamResponse DownloadPrintFormTemplateFile(DownloadFileParams<PrintFormTemplate> downloadFileParams)
        {
            var file = _fileContentFinder.Find(Specs.Find.ById<FileWithContent>(downloadFileParams.FileId)).Single();
            return new StreamResponse { FileName = file.FileName, ContentType = file.ContentType, Stream = file.Content };
        }

        public bool TemplateExists(PrintFormTemplate printFormTemplate)
        {
            var templates = _finder.Find(Specs.Find.ActiveAndNotDeleted<PrintFormTemplate>())
                .Where(template => template.Id != printFormTemplate.Id && template.TemplateCode == printFormTemplate.TemplateCode);

            // EF4 bug workaroud: 
            // http://data.uservoice.com/forums/72025-ado-net-entity-framework-ef-feature-suggestions/suggestions/1015361-incorrect-handling-of-null-variables-in-where-cl
            return printFormTemplate.BranchOfficeOrganizationUnitId.HasValue ?
                templates.Any(template => template.BranchOfficeOrganizationUnitId == printFormTemplate.BranchOfficeOrganizationUnitId) :
                templates.Any(template => template.BranchOfficeOrganizationUnitId == null);
        }

        public void CreateOrUpdatePrintFormTemplate(PrintFormTemplate printFormTemplate)
        {
            if (printFormTemplate.IsNew())
            {
                _identityProvider.SetFor(printFormTemplate);
                _printFormTemplateRepository.Add(printFormTemplate);
            }
            else
            {
                _printFormTemplateRepository.Update(printFormTemplate);
            }

            _printFormTemplateRepository.Save();
        }

        public long? GetPrintFormTemplateFileId(long branchOfficeOrganizationUnitId, TemplateCode templateCode)
        {
            var templates = _finder.Find<PrintFormTemplate>(x => !x.IsDeleted && x.IsActive && x.TemplateCode == templateCode)
                .OrderByDescending(x => x.Id);

            var specificTemplateId = templates
                .Where(x => x.BranchOfficeOrganizationUnitId == branchOfficeOrganizationUnitId)
                .Select(x => (long?)x.File.Id)
                .FirstOrDefault();

            if (specificTemplateId.HasValue)
            {
                return specificTemplateId;
            }

            var generalTemplate = templates
                .Where(x => x.BranchOfficeOrganizationUnitId == null)
                .Select(x => (long?)x.File.Id)
                .FirstOrDefault();

            return generalTemplate;
        }
    }
}

using System.IO;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.API.Operations.Generic.File;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Common.Utils;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Printing
{
    public sealed class PrintDocumentHandler : RequestHandler<PrintDocumentRequest, StreamResponse>
    {
        private readonly ITracer _tracer;
        private readonly IPrintFormService _printFormService;
        private readonly IFileService _fileService;
        private readonly IBranchOfficeRepository _branchOfficeRepository;

        public PrintDocumentHandler(IPrintFormService printFormService,
                                    IFileService fileService,
                                    IBranchOfficeRepository branchOfficeRepository,
                                    ITracer tracer)
        {
            _tracer = tracer;
            _printFormService = printFormService;
            _fileService = fileService;
            _branchOfficeRepository = branchOfficeRepository;
        }

        protected override StreamResponse Handle(PrintDocumentRequest request)
        {
            // Может показаться странным, заводить nullable свойство и не работать со значением null,
            // но в базе данных это поле действительно nullable и во всех хендлерах стояла проверка на null примерно такого типа.
            if (!request.BranchOfficeOrganizationUnitId.HasValue)
            {
                throw new NotificationException(BLResources.CannotChoosePrintformBecauseBranchOfficeIsNotSpecifiedForOrder);
            }

            var printFormTemplateId = _branchOfficeRepository.GetPrintFormTemplateId(request.BranchOfficeOrganizationUnitId.Value, request.TemplateCode);
            if (!printFormTemplateId.HasValue)
            {
                _tracer.WarnFormat("Для юр. лица отделения организации с id '{0}' не найден шаблон печатной формы '{1}'",
                                     request.BranchOfficeOrganizationUnitId,
                                     request.TemplateCode);
                throw new NotificationException(string.Format(BLResources.PrintFormTemplateNotFound, request.TemplateCode.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture)));
            }

            var templateFile = _fileService.GetFileById(printFormTemplateId.Value);
            if (templateFile == null)
            {
                throw new NotificationException(string.Format(BLResources.PrintformTemplateNotFoundForOrgUnitTemplate,
                                                              request.TemplateCode,
                                                              request.BranchOfficeOrganizationUnitId));
            }

            var memoryStream = new MemoryStream();
            templateFile.Content.CopyTo(memoryStream);
            _printFormService.PrintToDocx(memoryStream, request.PrintData, request.CurrencyIsoCode);

            return new StreamResponse
                   {
                       Stream = memoryStream,
                       ContentType = templateFile.ContentType,
                       FileName = ProtectFileName(request.FileName + ".docx"),
                   };
        }

        /// <summary>Заменяет в имени файла недопустимые символы подчёркиванием.</summary>
        private static string ProtectFileName(string sourceName)
        {
            return Path.GetInvalidFileNameChars().Aggregate(sourceName, (current, invalidChar) => current.Replace(invalidChar, '_'));
        }
    }
}
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified;
using DoubleGis.Erm.BLCore.API.Operations.Generic.File;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.File
{
    public class DownloadPrintFormTemplateFileService : IDownloadFileGenericService<PrintFormTemplate>
    {
        private readonly IPrintFormTemplateService _printFormTemplateService;

        public DownloadPrintFormTemplateFileService(IPrintFormTemplateService printFormTemplateService)
        {
            _printFormTemplateService = printFormTemplateService;
        }

        public StreamResponse DownloadFile(long fileId)
        {
            return _printFormTemplateService.DownloadPrintFormTemplateFile(new DownloadFileParams<PrintFormTemplate>(fileId));
        }
    }
}

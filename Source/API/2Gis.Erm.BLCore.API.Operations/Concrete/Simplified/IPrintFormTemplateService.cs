using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Generic.File;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified
{
    public interface IPrintFormTemplateService : ISimplifiedModelConsumer
    {
        UploadFileResult UploadPrintFormTemplateFile(UploadFileParams<PrintFormTemplate> uploadFileParams);
        StreamResponse DownloadPrintFormTemplateFile(DownloadFileParams<PrintFormTemplate> downloadFileParams);
        bool TemplateExists(PrintFormTemplate printFormTemplate);
        void CreateOrUpdatePrintFormTemplate(PrintFormTemplate printFormTemplate);
        long? GetPrintFormTemplateFileId(long branchOfficeOrganizationUnitId, TemplateCode templateCode);
    }
}

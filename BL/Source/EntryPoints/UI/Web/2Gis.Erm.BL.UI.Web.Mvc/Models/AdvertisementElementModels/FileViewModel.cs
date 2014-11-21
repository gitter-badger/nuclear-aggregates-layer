using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models.AdvertisementElementModels
{
    public class FileViewModel : ViewModel
    {
        public string TemplateFileExtensionRestriction { get; set; }

        public long? FileId { get; set; }
        public string FileName { get; set; }
        public string FileContentType { get; set; }
        public long FileContentLength { get; set; }

        public void LoadDomainEntityDto(IFileAdvertisementElementDomainEntityDto dto)
        {
            FileId = dto.FileId;
            FileName = dto.FileName;
            FileContentLength = dto.FileContentLength;
            FileContentType = dto.FileContentType;

            TemplateFileExtensionRestriction = dto.TemplateFileExtensionRestriction;
        }

        public void TranferToDomainEntityDto(IFileAdvertisementElementDomainEntityDto dto)
        {
            dto.FileId = FileId;
            dto.FileName = FileName;
            dto.FileContentLength = FileContentLength;
            dto.FileContentType = FileContentType;
        }
    }
}
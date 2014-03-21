using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListPrintFormTemplateDto : IOperationSpecificEntityDto
    {
        public long Id { get; set; }
        public long FileId { get; set; }
        public TemplateCode TemplateCodeEnum { get; set; }
        public string TemplateCode { get; set; }
        public string FileName { get; set; }
        public long? BranchOfficeOrganizationUnitId { get; set; }
        public string BranchOfficeOrganizationUnitName { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
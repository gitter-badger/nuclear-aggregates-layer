using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.List.DTO
{
    public sealed class ListPrintFormTemplateDto : IListItemEntityDto<PrintFormTemplate>
    {
        public long Id { get; set; }
        public long FileId { get; set; }
        public string TemplateCode { get; set; }
        public string FileName { get; set; }
        public long? BranchOfficeOrganizationUnitId { get; set; }
        public string BranchOfficeOrganizationUnitName { get; set; }
    }
}
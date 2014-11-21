using DoubleGis.Erm.Platform.API.Core.Operations;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListThemeTemplateDto : IOperationSpecificEntityDto
    {
        public long Id { get; set; }
        public string TemplateCode { get; set; }
        public string FileName { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
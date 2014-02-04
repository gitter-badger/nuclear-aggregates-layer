using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListThemeTemplateDto : IListItemEntityDto<ThemeTemplate>
    {
        public long Id { get; set; }
        public string TemplateCode { get; set; }
        public string FileName { get; set; }
    }
}
using System;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListThemeDto : IListItemEntityDto<Theme>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime BeginDistribution { get; set; }
        public DateTime EndDistribution { get; set; }
        public string TemplateCode { get; set; }
        public string Description { get; set; }
    }
}
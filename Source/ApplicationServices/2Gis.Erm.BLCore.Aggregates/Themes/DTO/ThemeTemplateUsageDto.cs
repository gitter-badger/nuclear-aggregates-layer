using System.Collections.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Themes.DTO
{
    public sealed class ThemeTemplateUsageDto
    {
        public long Id { get; set; }
        public long FileId { get; set; }

        public IEnumerable<ThemeUsageDto> Themes { get; set; }
    }
}

using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Prices.Dto
{
    public sealed class PricePositionDto
    {
        public long Id { get; set; }
        public long PositionId { get; set; }
        public string PositionName { get; set; }
        public long PriceId { get; set; }
        public IEnumerable<IEnumerable<RelatedItemDto>> Groups { get; set; }
        public IEnumerable<RelatedItemDto> DeniedPositions { get; set; }

        public sealed class RelatedItemDto
        {
            public ObjectBindingType BindingCheckMode { get; set; }
            public long PositionId { get; set; }

            public override bool Equals(object obj)
            {
                if (obj == null)
                {
                    return false;
                }

                var dto = obj as RelatedItemDto;
                if (dto == null)
                {
                    return false;
                }

                return PositionId == dto.PositionId && BindingCheckMode == dto.BindingCheckMode;
            }

            public override int GetHashCode()
            {
                return BindingCheckMode.GetHashCode() ^ PositionId.GetHashCode();
            }
        }
    }
}
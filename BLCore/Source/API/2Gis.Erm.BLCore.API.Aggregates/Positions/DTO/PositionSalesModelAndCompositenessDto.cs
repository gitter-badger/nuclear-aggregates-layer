using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Positions.DTO
{
    public sealed class PositionSalesModelAndCompositenessDto
    {
        public long PositionId { get; set; }
        public SalesModel SalesModel { get; set; }
        public bool IsComposite { get; set; }
    }
}
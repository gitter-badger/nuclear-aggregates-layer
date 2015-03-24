using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Positions.DTO
{
    public sealed class LinkingObjectsSchemaPositionDto
    {
        public long Id { get; set; }
        
        public string Name { get; set; }
        
        public long? AdvertisementTemplateId { get; set; }
        
        public long? DummyAdvertisementId { get; set; }

        public PositionBindingObjectType BindingObjectType { get; set; }
        
        public PositionsGroup PositionsGroup { get; set; }
    }
}

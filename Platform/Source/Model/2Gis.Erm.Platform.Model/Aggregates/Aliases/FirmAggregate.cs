using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.Platform.Model.Aggregates.Aliases
{
    public enum FirmAggregate
    {
        Firm = EntityName.Firm,
        FirmAddress = EntityName.FirmAddress,
        FirmContact = EntityName.FirmContact,
        Client = EntityName.Client, //
        CategoryFirmAddress = EntityName.CategoryFirmAddress,
        CityPhoneZone = EntityName.CityPhoneZone, 
        Reference = EntityName.Reference, 
        ReferenceItem = EntityName.ReferenceItem, 
        CardRelation = EntityName.CardRelation,
        Territory = EntityName.Territory,
        DepCard = EntityName.DepCard,
        HotClientRequest = EntityName.HotClientRequest
    }
}

using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.DTO.FirmInfo;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel
{
    public interface IFirmReadModel : IAggregateReadModel<Firm>
    {
        IReadOnlyDictionary<Guid, FirmWithAddressesAndProjectDto> GetFirmInfosByCrmIds(IEnumerable<Guid> crmIds);
        long GetOrderFirmId(long orderId);
        IEnumerable<long> GetFirmNonArchivedOrderIds(long firmId);
        long GetOrgUnitId(long firmId);
        bool HasFirmClient(long firmId);
        bool TryGetFirmAndClientByFirmAddress(long firmAddressCode, out FirmAndClientDto dto);
        IEnumerable<FirmAddress> GetFirmAddressesByFirm(long firmId);
        IEnumerable<FirmAddress> GetActiveOrWithSalesByFirm(long firmId);
        IEnumerable<FirmContact> GetContacts(long firmAddressId);
        IDictionary<long, IEnumerable<FirmContact>> GetFirmContactsByAddresses(long firmId);
        string GetFirmName(long firmId);
        string GetTerritoryName(long territoryId);
        bool DoesFirmBelongToClient(long firmId, long clientId);
        Firm GetFirm(long firmId);
        FirmAddress GetFirmAddress(long firmAddressId);
        CategoryFirmAddress GetCategoryFirmAddress(long categoryFirmAddressId);
        IEnumerable<CategoryFirmAddress> GetCategoryFirmAddressesByFirmAddresses(IEnumerable<long> firmAddressIds);
        IEnumerable<FirmContact> GetFirmContactsByFirmAddresses(IEnumerable<long> firmAddressIds);
        IEnumerable<FirmContact> GetFirmContactsByDepCards(IEnumerable<long> depCardIds);
        Dictionary<long, DepCard> GetDepCards(IEnumerable<long> depCardIds);
        Dictionary<long, FirmAddress> GetFirmAddresses(IEnumerable<long> firmAddressIds);
        Dictionary<long, Firm> GetFirms(IEnumerable<long> firmIds);
        IReadOnlyDictionary<int, RegionalTerritoryDto> GetRegionalTerritoriesByBranchCodes(IEnumerable<int> branchCodes, string regionalTerritoryPhrase);
        FirmContact GetFirmContact(long firmContactId);
            
        // COMMENT {all, 23.05.2014}: Телефонные коды городов, справочники и элементы справочника относятся к агрегату фирмы.
        // Это немного странно.
        // Если эти сущности уйдут из этого агрегата, например, в SimlifiedModel, то следующие методы можно будет смело переместить.
        Dictionary<long, string> GetCityPhoneZones(IEnumerable<long> phoneZoneIds);
        Dictionary<int, string> GetPhoneFormats(IEnumerable<int> phoneFormatCodes);
        Dictionary<int, string> GetPaymentMethods(IEnumerable<int> paymentMethodCodes);

        HotClientRequest GetHotClientRequest(long hotClientRequestId);
        bool IsTelesaleFirmAddress(long firmAddressId);
        IReadOnlyDictionary<long, long> GetFirmTerritories(IEnumerable<long> firmIds, string regionalTerritoryWord);
        IReadOnlyDictionary<long, CardRelation> GetCardRelationsByIds(IEnumerable<long> cardRelationIds);
        bool IsFirmInReserve(long firmId);
        long GetFirmOwnerCodeUnsecure(long firmId);
        IEnumerable<string> GetAddressesNamesWhichNotBelongToFirm(long firmId, IEnumerable<long> firmAddressIds);
    }
}
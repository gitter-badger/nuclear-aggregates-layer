using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.Aggregates.Firms.DTO;
using DoubleGis.Erm.BLCore.Aggregates.Firms.DTO.CardForErm;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Firms;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Firms
{
    public interface IFirmRepository : IAggregateRootRepository<Firm>,
                                       IQualifyAggregateRepository<Firm>,
                                       IDisqualifyAggregateRepository<Firm>,
                                       IChangeAggregateTerritoryRepository<Firm>,
                                       IChangeAggregateClientRepository<Firm>,
                                       IAssignAggregateRepository<Firm>
    {
        int Assign(Firm firm, long ownerCode);
        Firm GetFirm(long firmId);
        void Update(Firm firm);
        void Update(FirmAddress firmAddress);
        int Qualify(Firm firm, long currentUserCode, long reserveCode, long ownerCode, DateTime qualifyDate);
        Client PerformDisqualificationChecks(long firmId, long currentUserCode);
        int Disqualify(Firm firm, long currentUserCode, long reserveCode, DateTime disqualifyDate);
        Client GetFirmClient(long firmId);
        int SetFirmClient(Firm firm, long clientId);
        int ChangeTerritory(Firm firm, long territoryId);
        int ChangeTerritory(IEnumerable<Firm> firm, long territoryId);
        IEnumerable<long> GetFirmAddressesIds(long firmId);
        IEnumerable<Firm> GetFirmsByTerritory(long territoryId);

        IEnumerable<OrganizationUnitDto> ExportFirmWithActiveOrders();
        CompactFirmDto GetFirmInBrief(long firmId);

        int? GetOrganizationUnitDgppId(long organizationUnitId);
        OrganizationUnit GetOrganizationUnit(int organizationUnitDgppId);
        Territory ImportTerritory(ImportTerritoriesHeaderDto header, ImportTerritoryDto territoryDto);

        IEnumerable<long> GetTerritoriesOfOrganizationUnit(long organizationUnitId);
        IDictionary<int, long> GetOrganizationUnits();
        IEnumerable<long> GetCategoriesOfOrganizationUnit(long organizationUnitId);
        Firm ImportFirmFromDgpp(ImportFirmDto firm, FirmImportContext context);
        IEnumerable<FirmAddress> ImportFirmAddresses(Firm firm, ImportFirmDto dto, FirmImportContext context);
        void ImportAddressContacts(FirmAddress firmAddress, DTO.ImportFirmAddressDto dto);
        void ImportAddressCategories(FirmAddress firmAddress, DTO.ImportFirmAddressDto dto, FirmImportContext context);
        void DeleteFirmRelatedObjects(Firm firm);

        void ImportFirmPromisingValues(long userId);
        void UpdateFirmAddresses(IEnumerable<FirmAddress> syncFirmAddressesDtos);

        ImportFirmsResultDto ImportFirmFromServiceBus(ImportFirmServiceBusDto dto, long userId, long reserveUserId, int pregeneratedIdsAmount, string regionalTerritoryLocaleSpecificWord);
        void ImportTerritoryFromServiceBus(IEnumerable<TerritoryServiceBusDto> territoryDtos);
        void ImportBuildingFromServiceBus(IEnumerable<BuildingServiceBusDto> buildingDtos, string regionalTerritoryLocaleSpecificWord, bool enableReplication);
        void ImportCityPhoneZonesFromServiceBus(IEnumerable<CityPhoneZone> cityPhoneZones);
        long[] GetAdvertisementIds(long firmId);

        IEnumerable<AdditionalServicesDto> GetFirmAdditionalServices(long firmId);
        void SetFirmAdditionalServices(long firmId, IEnumerable<AdditionalServicesDto> additionalServices);

        IEnumerable<AdditionalServicesDto> GetFirmAddressAdditionalServices(long firmAddressId);
        void SetFirmAddressAdditionalServices(long firmAddressId, IEnumerable<AdditionalServicesDto> additionalServices);

        IEnumerable<FirmContact> GetContacts(long firmAddressId);
        IDictionary<long, IEnumerable<FirmContact>> GetFirmContacts(long firmId);
        
        bool IsTerritoryReplaceable(long oldTerritoryId, long newTerritoryId);

        void ImportFirmContacts(long firmAddressId, IEnumerable<ImportFirmContactDto> firmContacts, bool isDepCard);
        void ImportCategoryFirmAddresses(long firmAddressId, IEnumerable<ImportCategoryFirmAddressDto> categoryFirmAddresses);
        void ImportFirmAddresses(IEnumerable<DTO.CardForErm.ImportFirmAddressDto> firmAddresses, string regionalTerritoryName);

        // TODO {d.ivanov, 19.12.2013}: IReadModel
        IEnumerable<string> GetAddressesNames(IEnumerable<long> firmAddressIds); 

        // todo {d.ivanov, 2013-11-21}: IReadModel
        long GetFirmAddressOrganizationUnitId(long cardCode);

        void ImportDepCards(IEnumerable<ImportDepCardDto> importDepCardDtos);
    }
}
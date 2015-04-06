using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.DTO;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Firms
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
        int Qualify(Firm firm, long currentUserCode, long reserveCode, long ownerCode, DateTime qualifyDate);
        Client PerformDisqualificationChecks(long firmId, long currentUserCode);
        int Disqualify(Firm firm, long currentUserCode, long reserveCode, DateTime disqualifyDate);
        Client GetFirmClient(long firmId);
        int SetFirmClient(Firm firm, long clientId);
        int ChangeTerritory(Firm firm, long territoryId);
        int ChangeTerritory(IEnumerable<Firm> firm, long territoryId);
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
        [Obsolete("usecase оставлен просто для подстраховки - пока все города не откажутся от ДГПП, на практике он уже не используется")]
        IEnumerable<FirmAddress> ImportFirmAddresses(Firm firm, ImportFirmDto dto, FirmImportContext context);
        void ImportAddressContacts(FirmAddress firmAddress, ImportFirmAddressDto dto);
        void ImportAddressCategories(FirmAddress firmAddress, ImportFirmAddressDto dto, FirmImportContext context);
        [Obsolete("usecase оставлен просто для подстраховки - пока все города не откажутся от ДГПП, на практике он уже не используется")]
        void DeleteFirmRelatedObjects(Firm firm);

        void ImportFirmPromisingValues(long userId);
        void UpdateFirmAddresses(IEnumerable<FirmAddress> syncFirmAddressesDtos);

        long[] GetAdvertisementIds(long firmId);

        bool IsTerritoryReplaceable(long oldTerritoryId, long newTerritoryId);
    }
}
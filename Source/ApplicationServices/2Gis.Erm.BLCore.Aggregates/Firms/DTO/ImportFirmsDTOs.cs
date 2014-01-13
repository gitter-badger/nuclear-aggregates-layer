using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using DoubleGis.Erm.Platform.API.Security.UserContext.Identity;

namespace DoubleGis.Erm.BLCore.Aggregates.Firms.DTO
{
    public sealed class FirmImportContext
    {
        public IDictionary<int, long> OrganizationUnits { get; set; }
        public IEnumerable<long> Territories { get; set; }
        public IEnumerable<long> Categories { get; set; }
        public IUserInfo ReserveUserIdentity { get; set; }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public sealed class ImportFirmDto
    {
        public long DgppId { get; set; }
        public string Name { get; set; }
        public long TerritoryDgppId { get; set; }
        public int OrganizationUnitDgppId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsClosedForAscertainment { get; set; }

        public IEnumerable<ImportFirmAddressDto> Addresses { get; set; }
    }

    public sealed class ImportFirmAddressDto
    {
        public long DgppId { get; set; }
        public int SortingPosition { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string PaymentMethods { get; set; }
        public string WorkingTime { get; set; }
        public string WorkingTimeComment { get; set; }
        public bool IsActive { get; set; }
        public bool IsClosedForAscertainment { get; set; }

        public IEnumerable<ImportFirmAddressContactDto> Contacts { get; set; }
        public IEnumerable<ImportFirmAddressCategoryDto> Categories { get; set; }
    }

    public sealed class ImportFirmAddressContactDto
    {
        public long DgppId { get; set; }
        public int SortingPosition { get; set; }
        public int ContactType { get; set; }
        public string Contact { get; set; }
        public bool IsActive { get; set; }
        public bool IsClosedForAscertainment { get; set; }
    }

    public sealed class ImportFirmAddressCategoryDto
    {
        public long DgppId { get; set; }
        public int SortingPosition { get; set; }
        public bool IsPrimary { get; set; }
    }

    public sealed class ImportFirmsHeaderDto : ImportHeaderDto
    {
        public DateTime ExportStartDate { get; set; }
        public DateTime ExportEndDate { get; set; }
    }

    public sealed class ImportFirmsResultDto
    {
        public IEnumerable<long> FirmIdsOfImportedCards { get; set; }
    }

    public sealed class ImportFirmServiceBusDto
    {
        public string FirmXml { get; set; }
        public string CardsXml { get; set; }
        public IList<CardRelationServiceBusDto> CardRelationDtos { get; set; }
        public IList<ReferenceItemServiceBusDto> ReferenceItemDtos { get; set; }
        public IList<ReferenceServiceBusDto> ReferenceDtos { get; set; }
    }

    public sealed class CardRelationServiceBusDto
    {
        public long Code { get; set; }
        public long PointOfServiceCardCode { get; set; }
        public long DepartmentCardCode { get; set; }
        public int DepartmentCardSortingPosition { get; set; }
        public bool IsDeleted { get; set; }
    }

    public sealed class ReferenceItemServiceBusDto
    {
        public int Code { get; set; }
        public string ReferenceCode { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
    }

    public sealed class ReferenceServiceBusDto
    {
        public string Code { get; set; }
    }

    public sealed class CompactFirmDto
    {
        public long FirmId { get; set; }
        public string FirmName { get; set; }
        public long OrganizationUnitId { get; set; }
        public string OrganizationUnitName { get; set; }
        public long OwnerCode { get; set; }
    }

    public sealed class BuildingServiceBusDto
    {
        public long Code { get; set; }
        public long? SaleTerritoryCode { get; set; }
        public bool IsDeleted { get; set; }
    }
}

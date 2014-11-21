using System;

using DoubleGis.Erm.Platform.API.Core.Operations;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListOrganizationUnitDto : IOperationSpecificEntityDto
    {
        public long Id { get; set; }
        public int? DgppId { get; set; }
        public Guid ReplicationCode { get; set; }
        public string Name { get; set; }
        public DateTime FirstEmitDate { get; set; }
        public DateTime? ErmLaunchDate { get; set; }
        public DateTime? InfoRussiaLaunchDate { get; set; }
        public bool ErmLaunched { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }

        public long CurrencyId { get; set; }
        public long CountryId { get; set; }
        public string CountryName { get; set; }
    }
}
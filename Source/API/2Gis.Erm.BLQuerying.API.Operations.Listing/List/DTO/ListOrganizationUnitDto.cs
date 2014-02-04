using System;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListOrganizationUnitDto : IListItemEntityDto<OrganizationUnit>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int? DgppId { get; set; }
        public long CountryId { get; set; }
        public string CountryName { get; set; }
        public DateTime FirstEmitDate { get; set; }
        public Guid ReplicationCode { get; set; }
        public long OwnerCode { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public bool ErmLaunched { get; set; }
    }
}
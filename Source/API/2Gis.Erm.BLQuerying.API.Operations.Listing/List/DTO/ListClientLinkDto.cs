using System;

using DoubleGis.Erm.Platform.API.Core.Operations;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListClientLinkDto : IOperationSpecificEntityDto
    {
        public long Id { get; set; }

        public long MasterClientId { get; set; }
        public string MasterClientName { get; set; }

        public long ChildClientId { get; set; }
        public string ChildClientName { get; set; }

        public long CreatedBy { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
    }
}
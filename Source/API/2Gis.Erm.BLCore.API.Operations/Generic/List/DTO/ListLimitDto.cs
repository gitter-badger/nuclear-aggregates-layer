using System;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.List.DTO
{
    public sealed class ListLimitDto : IListItemEntityDto<Limit>
    {
        public long Id { get; set; }
        public string BranchOfficeName { get; set; }
        public string LegalPersonName { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? CloseDate { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public string ClientName { get; set; }
        public string OwnerName { get; set; }
        public string InspectorName { get; set; }
    }
}
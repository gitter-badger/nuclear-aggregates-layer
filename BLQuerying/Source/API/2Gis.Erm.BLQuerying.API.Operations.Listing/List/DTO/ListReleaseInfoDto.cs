using System;

using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListReleaseInfoDto : IOperationSpecificEntityDto
    {
        public long Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public DateTime PeriodStartDate { get; set; }
        public DateTime PeriodEndDate { get; set; }
        public long OrganizationUnitId { get; set; }
        public string OrganizationUnitName { get; set; }
        public bool IsBeta { get; set; }
        public string OperationType { get; set; }
        public ReleaseStatus StatusEnum { get; set; }
        public string Status { get; set; }
        public long OwnerCode { get; set; }
        public string Owner { get; set; }
        public string Comment { get; set; }
    }
}
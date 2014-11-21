using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Operations;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListRegionalAdvertisingSharingDto : IOperationSpecificEntityDto
    {
        public long Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime BeginDistributionDate { get; set; }
        public long SourceOrganizationUnitId { get; set; }
        public string SourceOrganizationUnitName { get; set; }
        public long DestOrganizationUnitId { get; set; }
        public string DestOrganizationUnitName { get; set; }
        public long SourceBranchOfficeOrgUnitId { get; set; }
        public string SourceBranchOfficeOrgUnitName { get; set; }
        public long DestBranchOfficeOrgUnitId { get; set; }
        public string DestBranchOfficeOrgUnitName { get; set; }
        public IEnumerable<string> OrderNumbersCollection { get; set; }
        public string OrderNumbers { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
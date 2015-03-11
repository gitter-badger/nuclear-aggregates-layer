using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Accounts.DTO
{
    public sealed class AccountDetailForExportDto
    {
        public long SourceOrganizationUnitId { get; set; }
        public long OrderId { get; set; }
        public string OrganizationUnitSyncCode1C { get; set; }
        public string BranchOfficeOrganizationUnitSyncCode1C { get; set; }
        public OrderType OrderType { get; set; }
        public string OrderNumber { get; set; }
        public DateTime OrderSignupDateUtc { get; set; }
        public string ElectronicMedia { get; set; }
        public decimal DebitAccountDetailAmount { get; set; }
        public Dictionary<PlatformEnum, decimal> PlatformDistributions { get; set; }

        public long AccountCode { get; set; }
        public long ProfileCode { get; set; }
    }
}
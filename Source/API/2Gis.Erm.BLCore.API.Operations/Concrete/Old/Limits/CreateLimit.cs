using System;

using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Limits
// ReSharper restore CheckNamespace
{
    public sealed class CreateLimitRequest : Request
    {
        public long AccountId { get; set; }
    }

    public sealed class CreateLimitResponse : Response
    {
        public long LegalPersonId { get; set; }
        public string LegalPersonName { get; set; }
        public long BranchOfficeId { get; set; }
        public string BranchOfficeName  { get; set; }
        public long LegalPersonOwnerId { get; set; }
        public decimal Amount { get; set; }
        public LimitStatus Status { get; set; }
        public DateTime StartPeriodDate { get; set; }
        public DateTime EndPeriodDate { get; set; }
    }
}
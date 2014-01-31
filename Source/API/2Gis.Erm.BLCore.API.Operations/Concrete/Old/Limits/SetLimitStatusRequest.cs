using System;

using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Limits
{
    public sealed class SetLimitStatusRequest : Request
	{
		public long LimitId { get; set; }
		public Guid[] LimitReplicationCodes { get; set; }
		public LimitStatus Status { get; set; }
	}
}
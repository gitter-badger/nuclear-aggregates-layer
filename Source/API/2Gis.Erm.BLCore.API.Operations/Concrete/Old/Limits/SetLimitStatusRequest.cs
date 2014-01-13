using System;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Limits
// ReSharper restore CheckNamespace
{
	public sealed class SetLimitStatusRequest : EditRequest<Limit>
	{
		public long LimitId { get; set; }
		public Guid[] LimitReplicationCodes { get; set; }

		public LimitStatus Status { get; set; }
	}
}
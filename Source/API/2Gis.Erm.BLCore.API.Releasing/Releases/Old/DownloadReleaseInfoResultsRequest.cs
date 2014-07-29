using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Releasing.Releases.Old
{
	public sealed class DownloadReleaseInfoResultsRequest : Request
	{
		public long ReleaseInfoId { get; set; }
	}
}
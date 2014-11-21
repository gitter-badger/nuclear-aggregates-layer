using System.IO;

using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.OneC
{
    public class ValidateAccountDetailsFrom1CRequest : Request
    {
        public string FileName { get; set; }
        public Stream InputStream { get; set; }
        public long BranchOfficeOrganizationUnitId { get; set; }
    }
}

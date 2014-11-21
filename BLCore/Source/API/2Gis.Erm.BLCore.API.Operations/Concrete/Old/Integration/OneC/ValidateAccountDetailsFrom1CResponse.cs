using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.OneC
{
    public class ValidateAccountDetailsFrom1CResponse : Response
    {
        public ValidateAccountDetailsFrom1CResponse()
        {
            Errors = new List<string>();
        }

        public List<string> Errors { get; private set; }
    }
}

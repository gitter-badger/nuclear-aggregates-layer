using DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Emirates.Crosscutting
{
    public sealed class EmiratesInnService : ICheckInnService, IEmiratesAdapted
    {
        private const int CommercialLicenseMaxLength = 10;

        public bool TryGetErrorMessage(string commercialLicense, out string message)
        {
            if (string.IsNullOrWhiteSpace(commercialLicense) || commercialLicense.Length > CommercialLicenseMaxLength)
            {
                message = Resources.Server.Properties.BLResources.EmiratesEnteredCommercialLicenseIsInvalid;
                return true;
            }

            message = null;
            return false;
        }
    }
}
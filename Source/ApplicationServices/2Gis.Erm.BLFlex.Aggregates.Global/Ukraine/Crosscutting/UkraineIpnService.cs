using DoubleGis.Erm.BLCore.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Ukraine.Crosscutting
{
    public sealed class UkraineIpnService : ICheckInnService, IUkraineAdapted
    {
        private const int LegalPersonIpnLength = 12;
        private const int BusinessManIpnLength = 10;

        public bool TryGetErrorMessage(string inn, out string message)
        {
            if (!string.IsNullOrWhiteSpace(inn) && inn.Length != LegalPersonIpnLength && inn.Length != BusinessManIpnLength)
            {
                message = Resources.Server.Properties.BLResources.UkraineEnteredIpnIsNotCorrect;
                return true;
            }

            message = null;
            return false;
        }
    }
}
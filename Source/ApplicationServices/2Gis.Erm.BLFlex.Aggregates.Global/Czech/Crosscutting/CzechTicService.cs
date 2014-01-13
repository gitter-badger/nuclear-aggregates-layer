using DoubleGis.Erm.BLCore.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.Platform.API.Core.Globalization;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Czech.Crosscutting
{
    public sealed class CzechTicService : ICheckInnService, ICzechAdapted
    {
        public bool TryGetErrorMessage(string inn, out string message)
        {
            message = null;
            return false;
        }
    }
}
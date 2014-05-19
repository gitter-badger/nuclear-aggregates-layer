using DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

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
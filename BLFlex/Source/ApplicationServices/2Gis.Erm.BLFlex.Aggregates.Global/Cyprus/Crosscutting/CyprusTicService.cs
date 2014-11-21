using DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Cyprus.Crosscutting
{
    public sealed class CyprusTicService : ICheckInnService, ICyprusAdapted
    {
        public bool TryGetErrorMessage(string inn, out string message)
        {
            message = null;
            return false;
        }
    }
}

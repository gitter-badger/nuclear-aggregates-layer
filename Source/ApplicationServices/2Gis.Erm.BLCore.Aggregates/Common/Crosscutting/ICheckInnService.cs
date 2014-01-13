
namespace DoubleGis.Erm.BLCore.Aggregates.Common.Crosscutting
{
    public interface ICheckInnService
    {
        bool TryGetErrorMessage(string inn, out string message);
    }
}
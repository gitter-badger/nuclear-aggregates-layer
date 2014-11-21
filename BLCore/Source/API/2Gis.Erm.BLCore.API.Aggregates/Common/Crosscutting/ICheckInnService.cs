namespace DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting
{
    public interface ICheckInnService
    {
        bool TryGetErrorMessage(string inn, out string message);
    }
}
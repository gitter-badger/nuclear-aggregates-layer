namespace DoubleGis.Erm.Platform.Common.Crosscutting
{
    /// <summary>
    /// Маркерный интерфейс для сквозной функциональности, 
    /// которая при выполнении не нарушает инвариантов, хотя и может работать на чтение/запись.
    /// </summary>
    public interface IInvariantSafeCrosscuttingService : ICrosscuttingService
    {
    }
}

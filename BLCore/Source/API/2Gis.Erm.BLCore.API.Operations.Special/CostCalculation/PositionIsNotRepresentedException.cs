using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.CostCalculation
{
    /// <summary>
    /// Исключение, возникающее при попытке использовать элемент номенклатуры, отсутсвуюий в прайс-листе
    /// </summary>
    public class PositionIsNotRepresentedException : BusinessLogicException
    {
        public PositionIsNotRepresentedException(string message) : base(message)
        {
            
        }

    }
}

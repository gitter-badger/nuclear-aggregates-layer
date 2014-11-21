using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders
{
    /// <summary>
    /// Формирует номер для заказа.
    /// Важно:
    /// 1. Сформированный номер возвращается с Response'ом. Переданный заказ остаётся без изменений.
    /// 2. Если у заказа уже есть номер, то новый номер будет сгенерирован на основе имеющегося. 
    ///    Если номера нет, то необходимо параметром ReservedNumber передать число, на основе которого будет сформирован номер.
    /// </summary>
    public sealed class GenerateOrderNumberRequest : Request
    {
        public Order Order { get; set; }
        public long? ReservedNumber { get; set; }
        public bool IsRegionalNumber { get; set; }
    }
}

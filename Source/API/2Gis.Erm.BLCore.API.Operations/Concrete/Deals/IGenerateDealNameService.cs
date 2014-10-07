using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Deal;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Deals
{
    // TODO {all, 03.09.2014}: Не смог понять откуда приехала эта штука, но это явно никакая не операция. Может быть это часть read-модели клиента?
    // COMMENT {d.ivanov, 04.09.2014}: не похоже на ответственность read-model. Возможно, необходимо ввести понятие операции, не изменяющей состояние системы.
    public interface IGenerateDealNameService : IOperation<GenerateDealNameIdentity>
    {
        string GenerateDealName(string clientName, string mainFirmName);
        string GenerateDealName(long clientId);
    }
}
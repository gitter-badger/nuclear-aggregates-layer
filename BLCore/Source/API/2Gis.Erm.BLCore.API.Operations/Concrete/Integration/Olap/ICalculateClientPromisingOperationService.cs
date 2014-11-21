using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Client;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Olap
{
    public interface ICalculateClientPromisingOperationService : IOperation<CalculateClientPromisingIdentity>
    {
        void CalculateClientPromising();
    }
}
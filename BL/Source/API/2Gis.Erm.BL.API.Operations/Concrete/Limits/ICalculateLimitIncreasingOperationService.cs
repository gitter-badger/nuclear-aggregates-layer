using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Limit;

namespace DoubleGis.Erm.BL.API.Operations.Concrete.Limits
{
    public interface ICalculateLimitIncreasingOperationService : IOperation<CalculateLimitIncreasingIdentity>
    {
        bool IsIncreasingRequired(long limitId, out decimal amountToIncrease);
    }
}
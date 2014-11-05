using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Deal;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Deals
{
    public interface ISetMainLegalPersonForDealOperationService : IOperation<SetMainLegalPersonForDealIdentity>
    {
        void SetMainLegalPerson(long dealId, long legalPersonId);
    }
}
using System;

using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Contact;

namespace DoubleGis.Erm.BL.API.Operations.Concrete.Simplified
{
    public interface IRequestBirthdayCongratulationOperationService : IOperation<RequestBirthdayCongratulationsIdentity>
    {
        void AddRequest(DateTime congratulationDate);
    }
}
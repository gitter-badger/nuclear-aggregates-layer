using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.OneC;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.LegalPerson;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.LegalPersons
{
    public interface IValidateLegalPersonsForExportOperationService : IOperation<ValidateLegalPersonsForExportIdentity>
    {
        IEnumerable<ErrorDto> Validate(IEnumerable<ValidateLegalPersonRequestItem> legalPersonsToValidate);
    }
}

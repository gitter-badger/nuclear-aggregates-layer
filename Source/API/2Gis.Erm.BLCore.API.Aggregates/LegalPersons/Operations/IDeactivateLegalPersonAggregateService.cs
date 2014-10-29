using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.Operations
{
    public interface IDeactivateLegalPersonAggregateService : IAggregateSpecificOperation<LegalPerson, DeactivateIdentity>
    {
        void Deactivate(LegalPerson legalPerson, IEnumerable<LegalPersonProfile> profiles);
    }
}

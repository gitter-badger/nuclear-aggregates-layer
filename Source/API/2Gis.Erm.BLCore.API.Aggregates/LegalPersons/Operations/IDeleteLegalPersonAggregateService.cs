using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.Operations
{
    public interface IDeleteLegalPersonAggregateService : IAggregateSpecificOperation<LegalPerson, DeleteIdentity>
    {
        void Delete(LegalPerson legalPerson, IEnumerable<LegalPersonProfile> profiles);
    }
}
using System.Collections.Generic;

using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.Operations
{
    public interface IDeleteLegalPersonAggregateService : IAggregateSpecificService<LegalPerson, DeleteIdentity>
    {
        void Delete(LegalPerson legalPerson, IEnumerable<LegalPersonProfile> profiles);
    }
}
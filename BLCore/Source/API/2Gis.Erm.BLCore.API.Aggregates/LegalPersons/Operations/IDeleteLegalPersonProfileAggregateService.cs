using System.Collections.Generic;

using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.Operations
{
    public interface IDeleteLegalPersonProfileAggregateService : IAggregateSpecificOperation<LegalPerson, DeleteIdentity>
    {
        void Delete(LegalPersonProfile legalPersonProfile, IEnumerable<Order> referringOrders);
    }
}
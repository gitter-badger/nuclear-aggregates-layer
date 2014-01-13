using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Common.Generics
{
    public interface IChangeAggregateClientRepository<TEntity> : IUnknownAggregateSpecificOperation<ChangeClientIdentity>
        where TEntity : class, IEntity, IEntityKey
    {
        ChangeAggregateClientValidationResult Validate(long entityId, long currentUserCode, long reserveCode);
        int ChangeClient(long entityId, long clientId, long currentUserCode, bool bypassValidation);
    }
    
    public sealed class ChangeAggregateClientValidationResult
    {
        public ChangeAggregateClientValidationResult(
            IEnumerable<string> warnings, 
            IEnumerable<string> securityErrors, 
            IEnumerable<string> domainErrors)
        {
            Warnings = warnings ?? Enumerable.Empty<string>();
            SecurityErrors = securityErrors ?? Enumerable.Empty<string>();
            DomainErrors = domainErrors ?? Enumerable.Empty<string>();
        }
        
        public IEnumerable<string> Warnings { get; private set; }
        public IEnumerable<string> SecurityErrors { get; private set; }
        public IEnumerable<string> DomainErrors { get; private set; }
    }
}
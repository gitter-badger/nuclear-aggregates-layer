using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Aggregates;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics
{
    [Obsolete("Use non-generic interface marked with IAggregateSpecificOperation")]
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
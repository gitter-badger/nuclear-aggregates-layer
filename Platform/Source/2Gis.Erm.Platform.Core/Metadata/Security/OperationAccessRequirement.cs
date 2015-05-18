using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Metadata.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;

using NuClear.Model.Common;
using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Core.Metadata.Security
{
    /// <summary>
    /// Требование применимо к конкретному типу сущности
    /// </summary>
    public class OperationAccessRequirement<TConcreteIdentity> : IOperationAccessRequirement
        where TConcreteIdentity : IdentityBase<TConcreteIdentity>, new()
    {
        private readonly StrictOperationIdentity _strictOperationIdentity;
        private readonly IList<StrictOperationIdentity> _usedOperations;
        private readonly HashSet<IAccessRequirement> _requirements;

        public OperationAccessRequirement(StrictOperationIdentity strictOperationIdentity)
        {
            _strictOperationIdentity = strictOperationIdentity;
            _usedOperations = new List<StrictOperationIdentity>();
            _requirements = new HashSet<IAccessRequirement>();
        }

        StrictOperationIdentity IOperationAccessRequirement.StrictOperationIdentity
        {
            get { return _strictOperationIdentity; }
        }

        IEnumerable<StrictOperationIdentity> IOperationAccessRequirement.UsedOperations
        {
            get { return _usedOperations; }
        }

        IEnumerable<IAccessRequirement> IOperationAccessRequirement.Requirements
        {
            get { return _requirements; }
        }

        public OperationAccessRequirement<TConcreteIdentity> Require(EntityAccessTypes privelege, params IEntityType[] names)
        {
            var requirements = names.Select(name => new EntityAccessRequirement(privelege, name)).Where(requirement => !_requirements.Contains(requirement));
            foreach (var requirement in requirements)
            {
                _requirements.Add(requirement);
            }

            return this;
        }

        public OperationAccessRequirement<TConcreteIdentity> Require(FunctionalPrivilegeName privelege)
        {
            var requirement = new FunctionalAccessRequirement(privelege);
            if (!_requirements.Contains(requirement))
            {
                _requirements.Add(requirement);
            }

            return this;
        }

        public OperationAccessRequirement<TConcreteIdentity> UsesOperation<TIdentity>()
            where TIdentity : OperationIdentityBase<TIdentity>, INonCoupledOperationIdentity, new()
        {
            _usedOperations.Add(new StrictOperationIdentity(OperationIdentityBase<TIdentity>.Instance, EntitySet.Create.NonCoupled));
            return this;
        }

        public OperationAccessRequirement<TConcreteIdentity> UsesOperation<TIdentity, TEntity>()
            where TIdentity : OperationIdentityBase<TIdentity>, IEntitySpecificOperationIdentity, new()
            where TEntity : IEntity, IEntityKey
        {
            _usedOperations.Add(new StrictOperationIdentity(OperationIdentityBase<TIdentity>.Instance, new[] { typeof(TEntity) }.AsEntitySet()));
            return this;
        }

        public OperationAccessRequirement<TConcreteIdentity> UsesOperation<TIdentity, TEntity1, TEntity2>()
            where TIdentity : OperationIdentityBase<TIdentity>, IEntitySpecificOperationIdentity, new()
            where TEntity1 : IEntity, IEntityKey
            where TEntity2 : IEntity, IEntityKey
        {
            _usedOperations.Add(new StrictOperationIdentity(OperationIdentityBase<TIdentity>.Instance, new[] { typeof(TEntity1), typeof(TEntity2) }.AsEntitySet()));
            return this;
        }
    }
}

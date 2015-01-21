using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Identities;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Core.Identities
{
    public abstract class IdentityProviderBase : IIdentityProvider
    {
        private readonly IIdentityRequestChecker _checker;

        protected IdentityProviderBase(IIdentityRequestChecker checker)
        {
            _checker = checker;
        }

        public void SetFor<TEntity>(params TEntity[] entities) where TEntity : class, IEntityKey
        {
            SetFor((IReadOnlyCollection<TEntity>)entities);
        }

        public void SetFor<TEntity>(IReadOnlyCollection<TEntity> entities) where TEntity : class, IEntityKey
        {
            _checker.CheckIdentityRequest(typeof(TEntity));

            var i = 0;
            var ids = New(entities.Count);
            foreach (var entity in entities)
            {
                entity.Id = ids[i++];
            }
        }

        protected abstract long[] New(int count);
    }
}
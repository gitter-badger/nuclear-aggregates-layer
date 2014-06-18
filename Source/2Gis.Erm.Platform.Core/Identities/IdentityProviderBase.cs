using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

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
            _checker.CheckIdentityRequest(typeof(TEntity));

            var i = 0;
            var ids = New(entities.Length);
            foreach (var entity in entities)
            {
                entity.Id = ids[i++];
            }
        }

        protected abstract long[] New(int count);
    }
}
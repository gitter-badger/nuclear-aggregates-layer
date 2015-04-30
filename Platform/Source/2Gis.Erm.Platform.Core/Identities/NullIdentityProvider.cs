using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Core.Identities
{
    public class NullIdentityProvider : IIdentityProvider
    {
        public void SetFor<TEntity>(params TEntity[] entities) where TEntity : class, IEntityKey
        {
            throw new NotSupportedException("NullIdentityProvider cannot provide identities");
        }

        public void SetFor<TEntity>(IReadOnlyCollection<TEntity> entities) where TEntity : class, IEntityKey
        {
            throw new NotSupportedException("NullIdentityProvider cannot provide identities");
        }
    }
}
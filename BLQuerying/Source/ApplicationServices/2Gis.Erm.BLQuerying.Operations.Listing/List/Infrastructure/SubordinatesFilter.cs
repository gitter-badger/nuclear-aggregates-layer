using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Security.API.UserContext;
using NuClear.Storage;
using NuClear.Storage.Futures.Queryable;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure
{
    public sealed class SubordinatesFilter : DescendantEntityFilter
    {
        private readonly IFinder _finder;
        private readonly IUserContext _userContext;

        public SubordinatesFilter(IFinder finder, IUserContext userContext)
        {
            _finder = finder;
            _userContext = userContext;
        }

        public IQueryable<TEntity> Apply<TEntity>(IQueryable<TEntity> queryable)
        {
            var descendantIds =
                    _finder.Find(new FindSpecification<UsersDescendant>(x => x.AncestorId == _userContext.Identity.Code && x.DescendantId.HasValue))
                    .Map(q => q.Select(x => x.DescendantId.Value))
                    .Many();

            return Apply<TEntity>(queryable, descendantIds, "OwnerCode");
        }
    }
}
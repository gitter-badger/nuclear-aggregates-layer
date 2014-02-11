using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure
{
    public abstract class ListEntityDtoServiceBase<TEntity, TEntityListDto> : IListGenericEntityDtoService<TEntity, TEntityListDto>
        where TEntity : class,  IEntity, IEntityKey
        where TEntityListDto : IListItemEntityDto<TEntity>
    {
        private static readonly MethodInfo WhereMethodInfo = typeof(Queryable).GetMethods().First(x => x.Name == "Where");
        private static readonly MethodInfo ContainsInt64MethodInfo = typeof(Enumerable).GetMethods().First(x => x.Name == "Contains").MakeGenericMethod(typeof(long));

        private readonly IQuerySettingsProvider _querySettingsProvider;
        private readonly EntityName _entityName = typeof(TEntity).AsEntityName();
        private readonly IFinderBaseProvider _finderBaseProvider;
        private readonly IFinder _finder;
        private readonly IUserContext _userContext;

        protected ListEntityDtoServiceBase(IQuerySettingsProvider querySettingsProvider,
                                           IFinderBaseProvider finderBaseProvider,
                                           IFinder finder,
                                           IUserContext userContext)
        {
            _querySettingsProvider = querySettingsProvider;
            _finderBaseProvider = finderBaseProvider;
            _finder = finder;
            _userContext = userContext;
        }

        protected IFinderBaseProvider FinderBaseProvider
        {
            get { return _finderBaseProvider; }
        }

        protected IUserContext UserContext
        {
            get { return _userContext; }
        }

        public ListResult List(SearchListModel searchListModel)
        {
            int count;

            var query = FinderBaseProvider.GetFinderBase(_entityName).FindAll<TEntity>();
            var querySettings = _querySettingsProvider.GetQuerySettings(_entityName, searchListModel);

            bool forSubordinates;
            if (querySettings.TryGetExtendedProperty("ForSubordinates", out forSubordinates))
            {
                query = GetDescendantQuery(query);
            }

            var data = GetListData(query, querySettings, out count).ToArray();

            return new EntityDtoListResult<TEntity, TEntityListDto>
                {
                    Data = data,
                    RowCount = count,
                    MainAttribute = querySettings.MainAttribute
                };
        }

        protected abstract IEnumerable<TEntityListDto> GetListData(IQueryable<TEntity> query,
                                                                   QuerySettings querySettings,
                                                                   out int count);

        private IQueryable<TEntity> GetDescendantQuery(IQueryable query)
        {
            var entityType = typeof(TEntity);

            var descendantIds =
                _finder.Find<UsersDescendant>(x => x.AncestorId == _userContext.Identity.Code && x.DescendantId.HasValue)
                       .Select(x => x.DescendantId.Value)
                       .ToArray();

            var parameter = Expression.Parameter(entityType, "x");

            var ownerCodePropertyInfo = entityType.GetProperty("OwnerCode");
            if (ownerCodePropertyInfo == null)
            {
                throw new ArgumentException("В типе не определёно свойство OwnerCode");
            }

            // ownerCodes.Contains(x.OwnerCode)
            var ownerCodeProperty = Expression.Property(parameter, ownerCodePropertyInfo);
            var ownerCodesConstant = Expression.Constant(descendantIds);
            var ownerCodesContainsX = Expression.Call(ContainsInt64MethodInfo, ownerCodesConstant, ownerCodeProperty);

            var lambdaExpression = Expression.Lambda(ownerCodesContainsX, parameter);
            var whereMethod = WhereMethodInfo.MakeGenericMethod(entityType);
            var whereExpression = Expression.Call(whereMethod, query.Expression, lambdaExpression);

            return query.Provider.CreateQuery<TEntity>(whereExpression);
        }
    }
}
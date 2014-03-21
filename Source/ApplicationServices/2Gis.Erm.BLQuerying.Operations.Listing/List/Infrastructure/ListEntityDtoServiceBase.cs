using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure
{
    public abstract class ListEntityDtoServiceBase<TEntity, TEntityListDto> : IListGenericEntityDtoService<TEntity, TEntityListDto>
        where TEntity : class,  IEntity, IEntityKey
        where TEntityListDto : IOperationSpecificEntityDto
    {
        public ListResult List(SearchListModel searchListModel)
        {
            var querySettings = GetQuerySettings(searchListModel);

            int count;
            var data = List(querySettings, out count).ToArray();

            return new EntityDtoListResult<TEntity, TEntityListDto>
            {
                Data = data,
                RowCount = count,
            };
        }

        private static QuerySettings GetQuerySettings(SearchListModel searchListModel)
        {
            var querySettings = new QuerySettings
            {
                SkipCount = searchListModel.Start,
                TakeCount = searchListModel.Limit,
                SortDirection = searchListModel.Dir,
                SortOrder = searchListModel.Sort,
                ParentEntityName = searchListModel.ParentEntityName,
                ParentEntityId = searchListModel.ParentEntityId,
                UserInputFilter = searchListModel.FilterInput,
                FilterName = searchListModel.NameLocaleResourceId,
            };

            var extendedInfo = searchListModel.ExtendedInfo;

            if (!string.IsNullOrEmpty(querySettings.FilterName))
            {
                string serverExtendedInfo;
                if (ExtendedInfoMetadata.TryGetExtendedInfo(querySettings.FilterName, out serverExtendedInfo))
                {
                    extendedInfo = extendedInfo + "&" + serverExtendedInfo;
                }
            }

            if (!string.IsNullOrEmpty(extendedInfo))
            {
                querySettings.ExtendedInfoMap = extendedInfo
                    .Split(new[] { '&', ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries))
                    .Where(x => x.Length == 2 && !string.Equals(x[1], "null", StringComparison.OrdinalIgnoreCase))
                    .ToDictionary(x => x[0].ToLowerInvariant(), x => x[1]);
            }
            else
            {
                querySettings.ExtendedInfoMap = new Dictionary<string, string>();
            }

            return querySettings;
        }

        protected abstract IEnumerable<TEntityListDto> List(QuerySettings querySettings, out int count);
    }
}
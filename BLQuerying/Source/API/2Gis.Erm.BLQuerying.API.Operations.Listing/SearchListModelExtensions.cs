using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing
{
    public static class SearchListModelExtensions
    {
        public static QuerySettings ToQuerySettings(this SearchListModel searchListModel)
        {
            var querySettings = new QuerySettings
            {
                SkipCount = searchListModel.Start,
                TakeCount = searchListModel.Limit,
                ParentEntityName = searchListModel.ParentEntityName,
                ParentEntityId = searchListModel.ParentEntityId,
                UserInputFilter = searchListModel.FilterInput,
                FilterName = searchListModel.NameLocaleResourceId,
                SearchListModel = searchListModel,
            };

            querySettings.Sort = (searchListModel.Sort ?? string.Empty).Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(x => new QuerySettingsSort
            {
                PropertyName = x,
                Direction = GetSortDirection(searchListModel.Dir),
            }).ToArray();

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

        private static SortDirection GetSortDirection(string sortDirection)
        {
            if (string.IsNullOrEmpty(sortDirection) || string.Equals(sortDirection, "ASC", StringComparison.OrdinalIgnoreCase))
            {
                return SortDirection.Ascending;
            }

            if (string.Equals(sortDirection, "DESC", StringComparison.OrdinalIgnoreCase))
            {
                return SortDirection.Descending;
            }

            throw new ArgumentException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing
{
    public static class QuerySettingsSortExtension
    {
        public static IReadOnlyCollection<QuerySettingsSort> InsertAndGetQuerySettingsSort(this IReadOnlyCollection<QuerySettingsSort> querySettingsSort, String sortField, SortDirection direction)
        {
            var addingQuerySettingsSort = new QuerySettingsSort
            {
                PropertyName = sortField,
                Direction = direction
            };
            return new[] { addingQuerySettingsSort }
                .Concat(querySettingsSort)
                .ToArray();
        }
    }
}

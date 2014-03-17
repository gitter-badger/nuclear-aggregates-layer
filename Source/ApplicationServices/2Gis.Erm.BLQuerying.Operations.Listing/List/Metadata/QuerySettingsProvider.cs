using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List.Metadata
{
    public sealed class QuerySettingsProvider : IQuerySettingsProvider
    {
        public QuerySettings GetQuerySettings(EntityName entityName, Type documentType, SearchListModel searchListModel)
        {
            var querySettings = new QuerySettings
            {
                SkipCount = searchListModel.Start,
                TakeCount = searchListModel.Limit,
                SortDirection = searchListModel.Dir,
                SortOrder = searchListModel.Sort,
                EntityName = entityName,
                ParentEntityName = searchListModel.ParentEntityName,
                ParentEntityId = searchListModel.ParentEntityId,
            };

            // TODO вычистить dataListStructure, заменить на нашу metadata (Filtered Fields)
            if (!string.IsNullOrEmpty(searchListModel.FilterInput))
            {
                var filteredFields = FilteredFieldMetadata.GetFilteredFields(documentType);
                querySettings.UserInputFilter = GetUserInputFilterPredicate(filteredFields, searchListModel.FilterInput);                
            }

            DataListInfo dataListInfo;
            if (!DataListMetadata.TryGetDataListInfo(entityName, searchListModel.NameLocaleResourceId, out dataListInfo))
            {
                throw new ArgumentException(searchListModel.NameLocaleResourceId);
            }
            querySettings.DefaultFilter = dataListInfo.DefaultFilter;

            // fill extended info map
            querySettings.ExtendedInfoMap = ParseExtendedInfo(dataListInfo.ExtendedInfo, searchListModel.ExtendedInfo);

            return querySettings;
        }

        private static IReadOnlyDictionary<string, string> ParseExtendedInfo(string extendedInfo1, string extendedInfo2)
        {
            if (string.IsNullOrEmpty(extendedInfo1) && string.IsNullOrEmpty(extendedInfo2))
            {
                return new Dictionary<string, string>();
            }

            var extendedInfo = extendedInfo1 + '&' + extendedInfo2;

            var extendedInfoMap = extendedInfo
                .Split(new[] { '&', ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries))
                .Where(x => x.Length == 2 && !string.Equals(x[1], "null", StringComparison.OrdinalIgnoreCase))
                .ToDictionary(x => x[0].ToLowerInvariant(), x => x[1]);

            return extendedInfoMap;
        }

        public static string GetUserInputFilterPredicate(FilteredField[] filteredFields, string filterInput)
        {
            const string Or = " || ";

            var stringBuilder = new StringBuilder();

            foreach (var filteredField in filteredFields)
        {
                if (filteredField.Type == typeof(string))
        {
                    if (stringBuilder.Length != 0)
            {
                        stringBuilder.Append(Or);
                    }

                    // escape double quotes
                    var filterInputEscaped = filterInput.Replace("\"", "\"\"");

                    // asterisks
                    if (filterInputEscaped.IndexOf('*') == 0)
            {
                        var filterInputNoAsterisk = filterInputEscaped.Trim('*');
                        stringBuilder.Append(filteredField.Name).AppendFormat(".Contains(\"{0}\")", filterInputNoAsterisk);

                }
                    else
                {
                        stringBuilder
                            .Append(filteredField.Name)
                            .AppendFormat(".StartsWith(\"{0}\")", filterInputEscaped);
                }
            }
                else if (filteredField.Type == typeof(short) ||
                         filteredField.Type == typeof(int) ||
                         filteredField.Type == typeof(long))
                {
                    long filterInputParsed;
                    if (long.TryParse(filterInput, out filterInputParsed))
            {
                        if (stringBuilder.Length != 0)
                {
                            stringBuilder.Append(Or);
                }

                        stringBuilder
                            .Append(filteredField.Name)
                            .AppendFormat("={0}", filterInputParsed);
                    }
            }
                else
                {
                    throw new ArgumentException("Unsupported field type");
                }
                }

            return stringBuilder.ToString();
        }
    }
}
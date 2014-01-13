using System;
using System.Linq;
using System.Text;

namespace DoubleGis.Erm.BLCore.API.Common.Metadata.Old.Dto
{
    public static class DtoUtils
    {
        public static string GetUserInputFilterPredicate(this DataListStructure dataListStructure, string filterInput)
        {
            if (string.IsNullOrEmpty(filterInput))
            {
                return null;
            }

            filterInput = filterInput.Replace(@"""", @"""""");
            var filteredFields = dataListStructure.Fields.Where(f => f.Filtered).ToArray();
            if (filteredFields.Length == 0)
            {
                return null;
            }

            var sb = new StringBuilder();
            foreach (var field in filteredFields)
            {
                if (!field.ExpressionPath.Contains("{0}"))
                {
                    switch (field.FieldType)
                    {
                        case "String":
                            sb.AppendFormat("{0}.StartsWith(\"{1}\"){2}", field.ExpressionPath, filterInput.ToLower(), DataListStructure.Delimiter);
                            break;
                        case "Date":
                            sb.AppendFormat("{0}=\"{1}\"{2}", field.ExpressionPath, filterInput, DataListStructure.Delimiter);
                            break;
                        case "Int":
                            int intVal;
                            if (int.TryParse(filterInput, out intVal))
                            {
                                sb.AppendFormat("{0}={1}{2}", field.ExpressionPath, intVal, DataListStructure.Delimiter);
                            }

                            break;
                        case "Float":
                            decimal decimalVal;
                            if (decimal.TryParse(filterInput, out decimalVal))
                            {
                                sb.AppendFormat("{0}={1}{2}", field.ExpressionPath, decimalVal, DataListStructure.Delimiter);
                            }

                            break;
                        case "Boolean":
                            bool boolValue;
                            if (bool.TryParse(filterInput, out boolValue))
                            {
                                sb.AppendFormat("{0}=\"{1}\"{2}", field.ExpressionPath, boolValue, DataListStructure.Delimiter);
                            }

                            break;
                        case "Custom":
                            sb.AppendFormat(field.ExpressionPath, filterInput);
                            break;
                        case "Auto":
                            Guid guidVal;
                            if (field.DotNetType == "System.GuId" && Guid.TryParse(filterInput, out guidVal))
                            {
                                sb.AppendFormat("{0}=\"{1}\"{2}", field.ExpressionPath, guidVal, DataListStructure.Delimiter);
                            }

                            break;
                    }
                }
                else
                {
                    sb.AppendFormat(field.ExpressionPath, filterInput.ToLower());
                    sb.Append(DataListStructure.Delimiter);
                }
            }

            var filterStr = sb.ToString();
            return filterStr.EndsWith(DataListStructure.Delimiter, StringComparison.OrdinalIgnoreCase) ? filterStr.Substring(0, filterStr.Length - DataListStructure.Delimiter.Length) : filterStr;
        }
    }
}

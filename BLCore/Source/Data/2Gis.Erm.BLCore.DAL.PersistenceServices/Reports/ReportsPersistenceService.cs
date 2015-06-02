using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml;

using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Reports.DTO;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.DAL.AdoNet;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices.Reports
{
    public class ReportPersistenceService : IReportPersistenceService
    {
        private static readonly XmlReaderSettings XmlSettings = new XmlReaderSettings
        {
            IgnoreComments = true,
            IgnoreWhitespace = true,
        }; 

        private readonly IDatabaseCaller _databaseCaller;
        private readonly IBusinessModelSettings _businessModelSettings;

        public ReportPersistenceService(IDatabaseCaller databaseCaller, IBusinessModelSettings businessModelSettings)
        {
            _databaseCaller = databaseCaller;
            _businessModelSettings = businessModelSettings;
        }

        public IEnumerable<ReportDto> GetReportNames(long userId)
        {
            var resultTable = _databaseCaller.ExecuteProcedureWithResultTable("ReportName",
                                                        null,
                                                         new Tuple<string, object>("User", userId),
                                                         new Tuple<string, object>("BusinessModel", (int)_businessModelSettings.BusinessModel));

            return resultTable.Rows
                        .Cast<DataRow>().Select(row => new ReportDto
            {
                Id = Convert.ToInt32(row[0]),
                DisplayName = Convert.ToString(row[1]),
                ReportName = Convert.ToString(row[2]),
                Timestamp = ToLong((byte[])row[3]),
                IsHidden = Convert.ToBoolean(row[4]),
                FormatParameter = Convert.ToString(row[5]),
            })
                       .ToList();
        }

        public IEnumerable<ReportFieldDto> GetReportFields(long reportId, long userId)
        {
            var result = new List<ReportFieldDto>();
            var resultTable = _databaseCaller.ExecuteProcedureWithResultTable("ReportField",
                                                         null,
                                                         new Tuple<string, object>("ReportName", reportId),
                                                         new Tuple<string, object>("User", userId));

            foreach (DataRow row in resultTable.Rows)
            {
                var fieldType = ParseFieldTypeXml(Convert.ToString(row[3]));
                var attributes = ParseAttrubutes(Convert.ToString(row[7]));
                var dto = new ReportFieldDto
                {
                    DisplayName = Convert.ToString(row[1]),
                    Name = Convert.ToString(row[2]),
                    Type = fieldType.Type,
                    LookupExtendedInfo = fieldType.ExtendedInfo,
                    LookupEntityName = fieldType.LookupEntityName,
                    ListValues = fieldType.ListValues,
                    IsRequired = !Convert.ToBoolean(row[4]),
                    DisplayOrder = Convert.ToInt32(row[5]),
                    Default = (ReportFieldDefault)Convert.ToInt32(row.IsNull(6) ? 0 : row[6]),
                    Timestamp = ToLong((byte[])row[8]),
                    IsHidden = Convert.ToBoolean(row[9]),
                    Dependencies = attributes.Dependencies,
                    GreaterOrEqualThan = attributes.GreaterThan,
                };
                result.Add(dto);
            }

            return result;
        }

        private static ulong ToLong(byte[] bytes)
        {
            if (bytes.Length > 8)
            {
                throw new ArgumentException(BLResources.TooLongTimestamp, "bytes");
            }

            return BitConverter.ToUInt64(bytes.Reverse().ToArray(), 0);
        }

        private static FieldTypeXml ParseFieldTypeXml(string xml)
        {
            var result = new FieldTypeXml();

            using (var stream = new StringReader(xml))
            using (var reader = XmlReader.Create(stream, XmlSettings))
            {
                reader.Read();
                while (reader.MoveToNextAttribute())
                {
                    switch (reader.Name.ToLower())
                    {
                        case "type":
                            // Не всегда устанавливаем значение, для полей Lookup требуется прочитать аттрибут "entity"
                            var fieldType = ParseFieldType(reader.Value);
                            if (fieldType != ReportFieldType.NotSpecified)
                            {
                                result.Type = fieldType;
                            }

                            break;
                        case "values":
                            result.ListValues = ParseEnumValues(reader.Value);
                            break;
                        case "entity":
                            // Когда указан тип сущности обязательно оспользуется "Lookup"
                            result.LookupEntityName = ParseEntityName(reader.Value);
                            break;
                        case "extendedinfo":
                            result.ExtendedInfo = reader.Value;
                            break;
                        default:
                            throw new ArgumentException(BLResources.WrongMessageXmlFormat);
                    }
                }
            }

            return result;
        }

        private static IDictionary<long, string> ParseEnumValues(string value)
        {
            return value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => { var items = s.Split(';'); return new { Key = long.Parse(items[0]), Name = items[1] }; })
                .ToDictionary(arg => arg.Key, arg => arg.Name);
        }

        private static IEntityType ParseEntityName(string value)
        {
            IEntityType result;
            return EntityType.Instance.TryParse(value, out result) ? result : EntityType.Instance.None();
        }

        private static ReportFieldType ParseFieldType(string value)
        {
            switch (value.ToLower())
            {
                case "dropdownlist":
                    return ReportFieldType.DropDownList;
                case "boolean":
                    return ReportFieldType.Boolean;
                case "dateday":
                    return ReportFieldType.DateDay;
                case "datemonth":
                    return ReportFieldType.DateMonth;
                case "lookup":
                    return ReportFieldType.Lookup;
                case "plaintext":
                    return ReportFieldType.PlainText;
                default:
                    return ReportFieldType.NotSpecified;
            }
        }

        private static AttriburesXml ParseAttrubutes(string xml)
        {
            var result = new AttriburesXml
            {
                Dependencies = new List<DependencyDefinitionDto>(),
                GreaterThan = new List<GreaterOrEqualThanDefinitionDto>(),
            };

            if (string.IsNullOrEmpty(xml))
            {
                return result;
            }

            using (var stream = new StringReader(xml))
            using (var reader = XmlReader.Create(stream, XmlSettings))
            {
                reader.Read();
                while (reader.Read() && reader.NodeType != XmlNodeType.EndElement)
                {
                    switch (reader.Name.ToLower())
                    {
                        case "dependency":
                            result.Dependencies.Add(ParseDependencyAttribute(reader));
                            break;
                        case "greater":
                            result.GreaterThan.Add(ParseGreaterAttribute(reader));
                            break;
                        default:
                            throw new ArgumentException(BLResources.WrongMessageXmlFormat);
                    }
                }
            }

            return result;
        }

        private static DependencyDefinitionDto ParseDependencyAttribute(XmlReader reader)
        {
            var result = new DependencyDefinitionDto();

            while (reader.MoveToNextAttribute())
            {
                switch (reader.Name.ToLower())
                {
                    case "type":
                        result.DependencyType = (DependencyType)int.Parse(reader.Value);
                        break;
                    case "field":
                        result.FieldName = reader.Value;
                        break;
                    case "expression":
                        result.DependencyScript = reader.Value;
                        break;
                }
            }

            reader.MoveToElement();
            return result;
        }

        private static GreaterOrEqualThanDefinitionDto ParseGreaterAttribute(XmlReader reader)
        {
            var result = new GreaterOrEqualThanDefinitionDto();
            while (reader.MoveToNextAttribute())
            {
                switch (reader.Name.ToLower())
                {
                    case "than":
                        result.FieldName = reader.Value;
                        break;
                    case "message":
                        result.ErrorMessage = reader.Value;
                        break;
                }
            }

            if (result.ErrorMessage == null || result.FieldName == null)
            {
                throw new ArgumentException(BLResources.InvalidGreaterOrEqualThanXml);
            }

            reader.MoveToElement();
            return result;
        }

        private sealed class AttriburesXml
        {
            public IList<DependencyDefinitionDto> Dependencies { get; set; }
            public IList<GreaterOrEqualThanDefinitionDto> GreaterThan { get; set; }
        }

        private sealed class FieldTypeXml
        {
            public ReportFieldType Type { get; set; }
            public string ExtendedInfo { get; set; }
            public IDictionary<long, string> ListValues { get; set; }
            public IEntityType LookupEntityName { get; set; }
        }
    }
}
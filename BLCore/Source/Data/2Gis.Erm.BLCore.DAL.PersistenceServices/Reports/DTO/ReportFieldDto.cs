using System.Collections.Generic;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices.Reports.DTO
{
    public enum ReportFieldType
    {
        NotSpecified = 0,

        Boolean = 1,
        DateDay = 2,
        DateMonth = 3,
        PlainText = 4,

        Lookup = 14,

        DropDownList = 100,
    }

    public enum ReportFieldDefault
    {
        DeafaultValue = 0,
        DateTimeMonthStart = 1,
        DateTimeNextMonthStart = 3,
        DateTimePrevMonthStart = 4, 
        BooleanTrue = 5,
        BooleanFalse = 6,
        DateTimeMonthEnd = 7,
        DateTimeNextMonthEnd = 8,
        DateTimePrevMonthEnd = 9, 

        CurrentUser = 10,
        CurrentOrganizationUnit = 11,
    }

    public sealed class ReportFieldDto
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public bool IsHidden { get; set; }
        public ReportFieldType Type { get; set; }
        public ReportFieldDefault Default { get; set; }
        public ulong Timestamp { get; set; }
        public int DisplayOrder { get; set; }

        // генерация атрибутов
        public bool IsRequired { get; set; }
        public IEnumerable<DependencyDefinitionDto> Dependencies { get; set; }
        public IEnumerable<GreaterOrEqualThanDefinitionDto> GreaterOrEqualThan { get; set; }

        // специфично для списковых типов
        public IDictionary<long, string> ListValues { get; set; }

        // специфично для Lookup
        public string LookupExtendedInfo { get; set; }
        public IEntityType LookupEntityName { get; set; }
    }
}

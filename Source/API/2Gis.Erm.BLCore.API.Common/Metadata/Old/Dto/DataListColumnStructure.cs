namespace DoubleGis.Erm.BLCore.API.Common.Metadata.Old.Dto
{
    public sealed class DataListColumnStructure
    {
        public bool Filtered { get; set; }
        public string ReferenceEntityName
        {
            set { ReferenceTo = value ?? string.Empty; }
        }
        public string ReferenceTo { get; set; }
        //public int? CrmCode { get; set; }
        //public bool ShowReadOnlyCard { get; set; }
        public string ReferenceFieldName
        {
            set { ReferenceKeyField = value ?? string.Empty; }
        }
        public string ReferenceKeyField { get; set; }
        
        public string LocalizedName { get; set; }
        public bool Hidden { get; set; }
        public string Name { get; set; }
        public short Width { get; set; }
        public string FieldType { get; set; }
        public string Type { get; set; }
        public string DotNetType { get; set; }
        public bool Sortable { get; set; }

        // [JsonIgnore] fields
        public string NameLocaleResourceId { get; set; }
        public string ExpressionPath { get; set; }
    }
}
namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings.ConfigurationDto
{
    public sealed class DataViewFieldJson
    {
        public string ReferenceEntityName
        {
            set { ReferenceTo = value ?? string.Empty; }
        }
        public string ReferenceTo { get; set; }
        public string ReferenceFieldName
        {
            set { ReferenceKeyField = value ?? string.Empty; }
        }
        public string ReferenceKeyField { get; set; }
        public string LocalizedName { get; set; }
        public bool Hidden { get; set; }
        public string Name { get; set; }
        public string RenderFn { get; set; }
        public string Style { get; set; }
        public short Width { get; set; }
        public string FieldType { get; set; }
        public string Type { get; set; }
        public bool Sortable { get; set; }
    }
}
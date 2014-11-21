using System.Collections.Generic;

namespace DoubleGis.Erm.BLCore.API.Common.Metadata.Old.Dto
{
    public sealed class DataListStructure
    {
        public string NameLocaleResourceId { get; set; }
        public string Title { get; set; }
        public string TitleLocaleResourceId { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string MainAttribute { get; set; }
        public short RowsPerPage { get; set; }
        public bool AllowMultiple { get; set; }
        public string ControllerAction { get; set; }
        public bool ReadOnly { get; set; }
        public string LocalizedName { get; set; }
        public string DefaultSortField { get; set; }
        public byte DefaultSortDirection { get; set; }
        public bool DisableEdit { get; set; }
        public string HideInCardRelatedGrid { get; set; }
        public bool IsHidden { get; set; }

        public IEnumerable<DataListColumnStructure> Fields { get; set; }
        public IEnumerable<ToolbarElementStructure> ToolbarItems { get; set; }

        public IEnumerable<DataListScriptReference> Scripts { get; set; }
    }
}
using System;

namespace DoubleGis.Erm.BLCore.API.Common.Metadata.Old.Dto
{
    public sealed class ToolbarElementStructure
    {
        public string Name { get; set; }
        public string ParentName { get; set; }
        public string HideInCardRelatedGrid { get; set; }

        public string ControlType { get; set; }
        public string Action { get; set; }
        public string Icon { get; set; }
        public bool Disabled { get; set; }
        public string LocalizedName { get; set; }
        public bool DisableOnEmpty { get; set; }

        // [JsonIgnore] fields
        [Obsolete]
        public int? SecurityPrivelege { get; set; }
        [Obsolete]
        public string NameLocaleResourceId { get; set; }
        [Obsolete]
        public bool LockOnInactive { get; set; }
        [Obsolete]
        public bool LockOnNew { get; set; }
    }
}
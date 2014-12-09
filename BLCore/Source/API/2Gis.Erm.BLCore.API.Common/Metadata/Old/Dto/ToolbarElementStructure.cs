using System;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.BLCore.API.Common.Metadata.Old.Dto
// ReSharper restore CheckNamespace
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
        [Obsolete("Убрать после удаления EntitySettings.xml")]
        public int? SecurityPrivelege { get; set; }
        [Obsolete("Убрать после удаления EntitySettings.xml")]
        public string NameLocaleResourceId { get; set; }
        [Obsolete("Убрать после удаления EntitySettings.xml")]
        public bool LockOnInactive { get; set; }
        [Obsolete("Убрать после удаления EntitySettings.xml")]
        public bool LockOnNew { get; set; }
    }
}
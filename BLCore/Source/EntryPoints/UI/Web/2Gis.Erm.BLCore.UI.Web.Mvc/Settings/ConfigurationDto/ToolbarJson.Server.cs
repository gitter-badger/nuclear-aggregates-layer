using Newtonsoft.Json;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings.ConfigurationDto
{
    public sealed partial class ToolbarJson
    {
        [JsonIgnore]
        public int? SecurityPrivelege { get; set; }
        [JsonIgnore]
        public string NameLocaleResourceId { get; set; }
        [JsonIgnore]
        public bool LockOnInactive { get; set; }
        [JsonIgnore]
        public bool LockOnNew { get; set; }
    }
}
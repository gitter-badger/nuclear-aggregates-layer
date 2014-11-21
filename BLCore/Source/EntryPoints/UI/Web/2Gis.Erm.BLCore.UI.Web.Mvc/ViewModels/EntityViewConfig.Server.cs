using DoubleGis.Erm.Platform.Model.Entities;

using Newtonsoft.Json;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels
// ReSharper restore CheckNamespace
{
    public sealed partial class EntityViewConfig
    {
        [JsonIgnore]
        public string DependencyList { get; set; }
        public EntityName EntityName { get; set; }
        public EntityName PType { get; set; }
    }
}
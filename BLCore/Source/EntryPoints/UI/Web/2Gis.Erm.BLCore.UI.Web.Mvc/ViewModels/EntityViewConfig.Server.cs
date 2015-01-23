using Newtonsoft.Json;

using NuClear.Model.Common.Entities;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels
// ReSharper restore CheckNamespace
{
    public sealed partial class EntityViewConfig
    {
        [JsonIgnore]
        public string DependencyList { get; set; }
        public IEntityType EntityName { get; set; }
        public IEntityType PType { get; set; }
    }
}
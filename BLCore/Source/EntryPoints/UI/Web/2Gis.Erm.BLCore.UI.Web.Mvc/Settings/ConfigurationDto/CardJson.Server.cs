using Newtonsoft.Json;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings.ConfigurationDto
// ReSharper restore CheckNamespace
{
    public sealed partial class CardJson
    {
        [JsonIgnore]
        public string TitleResourceId { get; set; }
        [JsonIgnore]
        public string EntityNameLocaleResourceId { get; set; }
    }
}
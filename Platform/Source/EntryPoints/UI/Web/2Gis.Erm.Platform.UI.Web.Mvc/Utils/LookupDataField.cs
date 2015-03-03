using Newtonsoft.Json;

namespace DoubleGis.Erm.Platform.UI.Web.Mvc.Utils
{
    public class LookupDataField    
    {
        [JsonProperty(PropertyName = "mapping")]
        public string Mapping { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        
    }
}
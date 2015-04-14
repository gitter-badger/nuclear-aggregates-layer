using Newtonsoft.Json;

namespace DoubleGis.Erm.Qds.Common
{
    internal sealed class ErmBulkOperationBody
    {
        [JsonProperty(PropertyName = "doc")]
        public object Doc { get; set; }

        [JsonIgnore]
        public UpdateType UpdateType { get; set; }
    }
}
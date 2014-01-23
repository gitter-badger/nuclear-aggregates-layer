using Nest;

using Newtonsoft.Json;

namespace DoubleGis.Erm.Qds.Migrations.Extensions
{
    internal sealed class IcuTokenFilter : TokenFilterBase
    {
        public IcuTokenFilter()
            : base("icu_collation")
        {
        }

        [JsonProperty("language")]
        public string Language { get; set; }
    }
}
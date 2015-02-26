using System;
using System.Text;

using Elasticsearch.Net.Serialization;

using Nest;
using Nest.Resolvers;

using Newtonsoft.Json;

namespace DoubleGis.Erm.Qds.Common
{
    internal sealed class InternalSerializer : NestSerializer
    {
        private readonly IConnectionSettingsValues _settings;

        public InternalSerializer(IConnectionSettingsValues settings)
            : base(settings)
        {
            _settings = settings;
        }

        public override byte[] Serialize(object data, SerializationFormatting formatting = SerializationFormatting.Indented)
        {
            var ermBulkOperationBody = data as ErmBulkOperationBody;
            if (ermBulkOperationBody == null)
            {
                return base.Serialize(data, formatting);
            }

            var formatting2 = (formatting == SerializationFormatting.None) ? Formatting.None : Formatting.Indented;
            var settings = new JsonSerializerSettings { ContractResolver = new ElasticContractResolver(_settings) };

            switch (ermBulkOperationBody.UpdateType)
            {
                case UpdateType.UpdateOverride:
                    settings.DefaultValueHandling = DefaultValueHandling.Include;
                    settings.NullValueHandling = NullValueHandling.Include;
                    break;
                case UpdateType.UpdateMerge:
                    settings.DefaultValueHandling = DefaultValueHandling.Ignore;
                    settings.NullValueHandling = NullValueHandling.Ignore;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data, formatting2, settings));
        }
    }
}
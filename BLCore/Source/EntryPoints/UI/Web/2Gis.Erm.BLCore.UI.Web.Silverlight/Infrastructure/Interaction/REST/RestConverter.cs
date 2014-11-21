using System.IO;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DoubleGis.Erm.BLCore.UI.Web.Silverlight.Infrastructure.Interaction.REST
{
    public class RestConverter
    {
        private static readonly DotNetDateTimeConverter CustomDateTimeConverter = new DotNetDateTimeConverter();
        private static readonly JsonSerializerSettings SerializerSettings;

        static RestConverter()
        {
            SerializerSettings = new JsonSerializerSettings();
            SerializerSettings.Converters.Add(CustomDateTimeConverter);
            SerializerSettings.Binder = new DefaultSerializationBinder();
        }

        public T ConstructObject<T>(Stream stream) where T : class, new()
        {
            
            var jsonSerializer = JsonSerializer.Create(SerializerSettings);
            using (var reader = new JsonTextReader(new StreamReader(stream)))
            {
                return jsonSerializer.Deserialize<T>(reader);
            }
        }
    }
}

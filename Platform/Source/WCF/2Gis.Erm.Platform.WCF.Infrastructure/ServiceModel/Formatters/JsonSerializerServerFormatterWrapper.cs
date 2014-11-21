using System.IO;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using System.Text;

using DoubleGis.Erm.Platform.Common.Serialization;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DoubleGis.Erm.Platform.WCF.Infrastructure.ServiceModel.Formatters
{
    // Предназначен только для BasicOperations, не рекомендуется к использованию в других сервисах.
    public class JsonSerializerServerFormatterWrapper : IDispatchMessageFormatter
    {
        private readonly IDispatchMessageFormatter _originalMessageFormatter;
        private readonly JsonSerializerSettings _settings;

        public JsonSerializerServerFormatterWrapper(IDispatchMessageFormatter originalMessageFormatter)
        {
            _originalMessageFormatter = originalMessageFormatter;
            _settings = new JsonSerializerSettings();
            _settings.Converters.Add(new DotNet2JavaScriptDateTimeConverter());
            _settings.Converters.Add(new StringEnumConverter());
            _settings.Converters.Add(new Int64ToStringConverter());
        }

        public void DeserializeRequest(Message message, object[] parameters)
        {
            _originalMessageFormatter.DeserializeRequest(message, parameters);
        }

        public Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result)
        {
            var stream = new MemoryStream();

            // WCF для null возвращает "", сделаем также.
            if (result == null)
            {
                result = string.Empty;
            }

            // Такой конструктор нужен, чтобы StreamWriter при закрытии не закрыл MemoryStream.
            using (var writer = new JsonTextWriter(new StreamWriter(stream, Encoding.UTF8, 0x400, true)))
            {
                var serializer = JsonSerializer.Create(_settings);
                serializer.Serialize(writer, result);
                writer.Flush();
            }

            stream.Seek(0, SeekOrigin.Begin);
            return WebOperationContext.Current.CreateStreamResponse(stream, "application/json");
        }
    }
}

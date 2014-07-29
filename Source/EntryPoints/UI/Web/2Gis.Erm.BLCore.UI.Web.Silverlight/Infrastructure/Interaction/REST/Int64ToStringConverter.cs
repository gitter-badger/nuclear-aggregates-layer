using System;

using Newtonsoft.Json;

namespace DoubleGis.Erm.BLCore.UI.Web.Silverlight.Infrastructure.Interaction.REST
{
    public class Int64ToStringConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value != null ? value.ToString() : "null");
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(long) || objectType == typeof(long?);
        }
    }
}
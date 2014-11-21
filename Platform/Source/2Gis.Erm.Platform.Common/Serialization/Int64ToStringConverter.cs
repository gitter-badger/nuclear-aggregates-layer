using System;

using Newtonsoft.Json;

namespace DoubleGis.Erm.Platform.Common.Serialization
{
    public class Int64ToStringConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value != null ? value.ToString() : "null");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value is long || reader.Value is int)
            {
                return Convert.ToInt64(reader.Value);
            }

            if (reader.Value is string)
            {
                if (string.IsNullOrWhiteSpace((string)reader.Value) && objectType == typeof(long?))
                {
                    return null;
                }

                long val;
                if (long.TryParse((string)reader.Value, out val))
                {
                    return val;
                }
            }

            if (reader.Value == null)
            {
                return null;
            }

            throw new JsonReaderException("Cannot convert value " + reader.Value + "to Int64");
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(long) || objectType == typeof(long?);
        }
    }
}
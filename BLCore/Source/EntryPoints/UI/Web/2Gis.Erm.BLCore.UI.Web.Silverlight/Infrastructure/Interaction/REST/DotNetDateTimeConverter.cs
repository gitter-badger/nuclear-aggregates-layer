using System;
using System.Globalization;

using Newtonsoft.Json;

namespace DoubleGis.Erm.BLCore.UI.Web.Silverlight.Infrastructure.Interaction.REST
{
    public sealed class DotNetDateTimeConverter : Newtonsoft.Json.Converters.JavaScriptDateTimeConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            string val;
            if (value is DateTime)
            {
                val = ((DateTime)value).ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                if (!(value is DateTimeOffset))
                {
                    throw new Exception("Expected date object value.");
                }

                var dateTimeOffset = (DateTimeOffset)value;
                val = dateTimeOffset.DateTime.ToString(CultureInfo.InvariantCulture);
            }

            writer.WriteStartConstructor("Date");
            writer.WriteValue(val);
            writer.WriteEndConstructor();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Type t = objectType.IsNullable()
                ? Nullable.GetUnderlyingType(objectType) 
                : objectType;
            
            if (reader.TokenType == JsonToken.Null)
            {
                if (!objectType.IsNullable())
                {
                    throw new Exception(string.Format("Cannot convert null value to {0}.", objectType));
                }

                return null;
            }

            if (reader.TokenType != JsonToken.StartConstructor || !string.Equals(reader.Value.ToString(), "Date", StringComparison.Ordinal))
            {
                throw new Exception(string.Format("Unexpected token or value when parsing date. Token: {0}, Value: {1}", reader.TokenType, reader.Value));
            }

            reader.Read();

            if (reader.TokenType != JsonToken.String)
            {
                throw new Exception(string.Format("Unexpected token parsing date. Expected {0}, got {1}.", JsonToken.String, reader.TokenType));
            }

            var parsedDateTime = DateTime.Parse((string)reader.Value, CultureInfo.InvariantCulture);
            reader.Read();

            if (reader.TokenType != JsonToken.EndConstructor)
            {
                throw new Exception(string.Format("Unexpected token parsing date. Expected EndConstructor, got {0}.", reader.TokenType));
            }

            if (t == typeof(DateTimeOffset))
            {
                return new DateTimeOffset(parsedDateTime);
            }

            return parsedDateTime;
        }   
    }
}

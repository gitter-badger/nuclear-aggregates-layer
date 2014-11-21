using System;
using System.Globalization;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DoubleGis.Erm.Platform.Common.Serialization
{
    // entity framework return datetime as DateTimeKind.Unspecified, assume this is DateTimeKind.Utc
    public sealed class DotNet2JavaScriptDateTimeConverter : JavaScriptDateTimeConverter
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
    }
}

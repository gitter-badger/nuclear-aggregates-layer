using System;

using Newtonsoft.Json;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Common.Serialization
{
    public class EntityTypeToStringConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var entityType = value as IEntityType;
            if (entityType != null)
            {
                writer.WriteValue(entityType.Description);
            }
            else
            {
                writer.WriteValue(value != null ? value.ToString() : "null");
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            IEntityType value;
            if (reader.Value is int && EntityType.Instance.TryParse((int)reader.Value, out value))
            {
                return value;
            }

            var stringValue = reader.Value as string;
            if (string.IsNullOrEmpty(stringValue))
            {
                return null;
            }
            
            if (EntityType.Instance.TryParse(stringValue, out value))
            {
                return value;
            }

            throw new JsonReaderException("Cannot convert value " + reader.Value + "to IEntityType");
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(IEntityType).IsAssignableFrom(objectType) || objectType == typeof(int) || objectType == typeof(string);
        }
    }
}
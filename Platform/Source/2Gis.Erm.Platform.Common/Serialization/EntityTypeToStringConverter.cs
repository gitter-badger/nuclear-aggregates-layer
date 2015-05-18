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
                throw new JsonWriterException("A value of IEntityType was expected");
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(IEntityType).IsAssignableFrom(objectType);
        }
    }
}
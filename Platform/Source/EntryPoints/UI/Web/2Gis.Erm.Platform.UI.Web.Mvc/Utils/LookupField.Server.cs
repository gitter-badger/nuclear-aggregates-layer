using System;

using DoubleGis.Erm.Platform.Model.Entities;

using Newtonsoft.Json;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.UI.Web.Mvc.Utils
{
    public sealed partial class LookupField
    {
        public static LookupField ParseFromJson(string value)
        {
            try
            {
                var lookupField = JsonConvert.DeserializeObject<LookupField>(value);
                if (lookupField == null || lookupField.Key == null)
                {
                    return null;
                }

                return lookupField;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static LookupField FromReference(EntityReference reference)
        {
            return reference != null
                       ? new LookupField
                           {
                               Key = reference.Id, 
                               Value = reference.Name
                           }
                       : new LookupField();
        }

        public override string ToString()
        {
            var javascriptAdaptedObject = new
            {
                Key = Key.HasValue ? Key.Value.ToString() : string.Empty, 
                Value = Value
            };

            return JsonConvert.SerializeObject(javascriptAdaptedObject);
        }
    }

    public static class LookupFieldExtensions
    {
        public static EntityReference ToReference(this LookupField lookupField, IEntityType entityName = null)
        {
            return lookupField != null
                       ? entityName != null
                             ? new EntityReference(lookupField.Key, lookupField.Value) { EntityTypeId = entityName.Id }
                             : new EntityReference(lookupField.Key, lookupField.Value)
                       : new EntityReference();
        }

        public static LookupField ToLookupField(this EntityReference entityReference)
        {
            return entityReference != null ? new LookupField { Key = entityReference.Id, Value = entityReference.Name } : new LookupField();
        }
    }
}
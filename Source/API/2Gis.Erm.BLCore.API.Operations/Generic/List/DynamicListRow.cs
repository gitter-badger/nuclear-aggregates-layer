using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.Serialization;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.List
{
    /// <summary>
    /// Необходима кастомная сериализация для единообразия json-представления DynamicListResult и остальных наследников ListResult.
    /// </summary>
    [Serializable]
    public class DynamicListRow : DynamicObject, ISerializable
    {
        public DynamicListRow()
        {
        }
        
        protected DynamicListRow(SerializationInfo info, StreamingContext context)
        {
            var fieldsList = new List<DynamicPropertyValue>(info.MemberCount);
            foreach (var field in info)
            {
                fieldsList.Add(new DynamicPropertyValue { Name = field.Name, Value = field.Value });
            }

            Values = fieldsList;
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (Values != null)
            {
                foreach (var dynamicProperty in Values)
                {
                    info.AddValue(dynamicProperty.Name, dynamicProperty.Value);
                }
            }
        }
        

        public IEnumerable<DynamicPropertyValue> Values { get; set; }
         
        /// <summary>
        /// Для поддержки биндинга.
        /// </summary>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (Values != null)
            {
                var foundPropertyValue = GetPropertyValue(binder.Name);
                if (foundPropertyValue != null)
                {
                    result = foundPropertyValue;
                    return true;
                }
            }

            return base.TryGetMember(binder, out result);
        }

        public object GetPropertyValue(string propertyName)
        {
            return Values.Where(x => x.Name == propertyName).Select(x => x.Value).FirstOrDefault();
        }
    }
}

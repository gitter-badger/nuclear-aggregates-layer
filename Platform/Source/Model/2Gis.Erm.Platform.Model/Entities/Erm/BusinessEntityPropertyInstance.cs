using System;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class BusinessEntityPropertyInstance : IEntity,
                                                         INonActivityDynamicEntityPropertyInstance
    {
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (GetType() != obj.GetType())
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            var propertyInstance = (BusinessEntityPropertyInstance)obj;

            return EntityInstanceId == propertyInstance.EntityInstanceId && PropertyId == propertyInstance.PropertyId;
        }

        public long EntityInstanceId { get; set; }
        public int PropertyId { get; set; }
        public string TextValue { get; set; }
        public decimal? NumericValue { get; set; }
        public DateTime? DateTimeValue { get; set; }

        public BusinessEntityInstance BusinessEntityInstance { get; set; }

        public override int GetHashCode()
        {
            unchecked
            {
                return (EntityInstanceId.GetHashCode() * 397) ^ PropertyId;
            }
        }
    }
}
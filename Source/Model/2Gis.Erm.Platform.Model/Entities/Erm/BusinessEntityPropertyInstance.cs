using System;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed partial class BusinessEntityPropertyInstance :
        IEntity
    {
        public long EntityInstanceId { get; set; }
        public int PropertyId { get; set; }
        public string TextValue { get; set; }
        public decimal? NumericValue { get; set; }
        public DateTime? DateTimeValue { get; set; }

        public BusinessEntityInstance BusinessEntityInstance { get; set; }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
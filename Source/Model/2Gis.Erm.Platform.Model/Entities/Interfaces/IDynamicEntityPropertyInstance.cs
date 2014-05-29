using System;

namespace DoubleGis.Erm.Platform.Model.Entities.Interfaces
{
    public interface IDynamicEntityPropertyInstance : IEntity
    {
        long EntityInstanceId { get; set; }
        int PropertyId { get; set; }
        string TextValue { get; set; }
        decimal? NumericValue { get; set; }
        DateTime? DateTimeValue { get; set; } 
    }
}
using System;

namespace DoubleGis.Erm.Platform.Model.Entities.Interfaces
{
    public interface IAuditableEntity
    {
        long CreatedBy { get; set; }
        DateTime CreatedOn { get; set; }
        long? ModifiedBy { get; set; }
        DateTime? ModifiedOn { get; set; }
    }
}

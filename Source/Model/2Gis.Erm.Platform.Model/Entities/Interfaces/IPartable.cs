using System.Collections.Generic;

namespace DoubleGis.Erm.Platform.Model.Entities.Interfaces
{
    public interface IPartable
    {
        IEnumerable<IEntityPart> Parts { get; set; }
    }
}
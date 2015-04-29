using NuClear.Model.Common.Entities;
using NuClear.Storage;

namespace DoubleGis.Erm.Platform.DAL
{
    public interface IQueryProvider
    {
        IQuery Get(IEntityType entityName);
    }
}
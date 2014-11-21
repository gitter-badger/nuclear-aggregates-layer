using System.Data.Entity;

namespace DoubleGis.Erm.Platform.DAL.EntityFramework
{
    public interface IDbEntityEntry
    {
        object Entity { get; }
        EntityState State { get; }
        void SetCurrentValues(object entity);
        void SetAsModified();
    }
}
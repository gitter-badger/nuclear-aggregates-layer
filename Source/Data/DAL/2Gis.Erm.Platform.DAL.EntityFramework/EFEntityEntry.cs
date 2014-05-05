using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace DoubleGis.Erm.Platform.DAL.EntityFramework
{
    public class EFEntityEntry : IDbEntityEntry
    {
        private readonly DbEntityEntry _entry;

        public EFEntityEntry(DbEntityEntry entry)
        {
            _entry = entry;
        }

        public object Entity
        {
            get { return _entry.Entity; }
        }

        public EntityState State
        {
            get { return _entry.State; }
        }

        public void SetCurrentValues(object entity)
        {
            _entry.CurrentValues.SetValues(entity);
            SetAsModified();
        }

        public void SetAsModified()
        {
            _entry.State = EntityState.Modified;
        }
    }
}
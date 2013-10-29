using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Linq;

namespace DoubleGis.Erm.Platform.DAL.EntityFramework
{
    public interface IObjectContext : IDisposable
    {
        int? CommandTimeout { get; set; }

        IQueryable CreateQuery(Type entityType);
        IObjectSet<TEntity> CreateObjectSet<TEntity>() where TEntity : class;

        object GetObjectByKey(EntityKey key);
        void TryGetObjectStateEntry(EntityKey getEntityKey, out EFEntityStateEntry stateEntry);
        IEnumerable<EFEntityStateEntry> GetObjectStateEntries(EntityState state);

        void ChangeObjectState(object entity, EntityState state);

        void AcceptAllChanges();
        void DetectChanges();
        int SaveChanges(System.Data.Objects.SaveOptions options);
        
        int ExecuteFunction(string functionName, params ObjectParameter[] parameters);
        ObjectResult<TElement> ExecuteFunction<TElement>(string functionName, params ObjectParameter[] parameters);
    }
}

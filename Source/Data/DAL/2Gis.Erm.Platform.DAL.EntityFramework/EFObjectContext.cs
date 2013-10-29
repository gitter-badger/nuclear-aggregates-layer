using System;
using System.Collections.Generic;
using System.Data;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DoubleGis.Erm.Platform.DAL.EntityFramework
{
    public class EFObjectContext : IObjectContext
    {
        private static readonly MethodInfo CreateObjectSetGenericMethodDefinition = typeof(ObjectContext).GetMethod("CreateObjectSet", new Type[] { });

        private readonly ObjectContext _objectContext;

        public EFObjectContext(EntityConnection connection)
        {
            _objectContext = new ObjectContext(connection);
        }

        public void AcceptAllChanges()
        {
            _objectContext.AcceptAllChanges();
        }

        public IQueryable CreateQuery(Type entityType)
        {
            var createObjectSetGenericMethod = CreateObjectSetGenericMethodDefinition.MakeGenericMethod(entityType);
            var callExpression = Expression.Call(Expression.Constant(_objectContext), createObjectSetGenericMethod);
            var lambdaExpression = Expression.Lambda<Func<IQueryable>>(callExpression);

            var lambdaCompiled = lambdaExpression.Compile();
            var lambdaCalled = lambdaCompiled();
            return lambdaCalled;
        }

        public IObjectSet<TEntity> CreateObjectSet<TEntity>() where TEntity : class
        {
            var objectSet = _objectContext.CreateObjectSet<TEntity>();
            return new EFObjectSet<TEntity>(objectSet);
        }

        public void Dispose()
        {
            _objectContext.Dispose();
        }

        public object GetObjectByKey(EntityKey key)
        {
            return _objectContext.GetObjectByKey(key);
        }

        public void DetectChanges()
        {
            _objectContext.DetectChanges();
        }

        public int SaveChanges(System.Data.Objects.SaveOptions options)
        {
            return _objectContext.SaveChanges(options);
        }

        public int ExecuteFunction(string functionName, params ObjectParameter[] parameters)
        {
            return _objectContext.ExecuteFunction(functionName, parameters);
        }

        public ObjectResult<TElement> ExecuteFunction<TElement>(string functionName, params ObjectParameter[] parameters)
        {
            return _objectContext.ExecuteFunction<TElement>(functionName, parameters);
        }

        public int? CommandTimeout
        {
            get { return _objectContext.CommandTimeout; }
            set { _objectContext.CommandTimeout = value; }
        }

        public void TryGetObjectStateEntry(EntityKey getEntityKey, out EFEntityStateEntry stateEntry)
        {
            ObjectStateEntry entry;
            _objectContext.ObjectStateManager.TryGetObjectStateEntry(getEntityKey, out entry);
            stateEntry = entry != null ? ConvertStateEntry(entry) : null;
        }

        public IEnumerable<EFEntityStateEntry> GetObjectStateEntries(EntityState state)
        {
            return _objectContext.ObjectStateManager.GetObjectStateEntries(state).Select(ConvertStateEntry);
        }

        public void ChangeObjectState(object entity, EntityState state)
        {
            _objectContext.ObjectStateManager.ChangeObjectState(entity, state);
        }

        private static EFEntityStateEntry ConvertStateEntry(ObjectStateEntry entry)
        {
            return new EFEntityStateEntry(entry.Entity, entry.State, entry.IsRelationship);
        }
    }
}

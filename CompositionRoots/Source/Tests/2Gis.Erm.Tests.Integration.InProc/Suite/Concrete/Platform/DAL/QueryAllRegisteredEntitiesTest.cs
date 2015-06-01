using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Transactions;

using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage.EntityFramework;
using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;
using NuClear.Tracing.API;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Platform.DAL
{
    public class QueryAllRegisteredEntitiesTest : IIntegrationTest
    {
        private readonly IEFDbModelConfigurationsProvider _dbModelConfigurationsProvider;
        private readonly IQuery _query;
        private readonly IFinder _finder;
        private readonly ITracer _tracer;

        public QueryAllRegisteredEntitiesTest(IEFDbModelConfigurationsProvider dbModelConfigurationsProvider, IQuery query, IFinder finder, ITracer tracer)
        {
            _dbModelConfigurationsProvider = dbModelConfigurationsProvider;
            _query = query;
            _finder = finder;
            _tracer = tracer;
        }

        public ITestResult Execute()
        {
            var ermTypes = _dbModelConfigurationsProvider.GetConfigurations(ErmContainer.Instance.Name).Select(x => x.EntityType);
            var securityTypes = _dbModelConfigurationsProvider.GetConfigurations(ErmSecurityContainer.Instance.Name).Select(x => x.EntityType);

            return CheckTypes(ermTypes) && CheckTypes(securityTypes) ? OrdinaryTestResult.As.Succeeded : OrdinaryTestResult.As.Failed;
        }

        private bool CheckTypes(IEnumerable<Type> entityTypes)
        {
            var succeeded = true;
            foreach (var entityType in entityTypes)
            {
                var method = typeof(QueryAllRegisteredEntitiesTest).GetMethod(ResolveMethodName(entityType), BindingFlags.NonPublic | BindingFlags.Instance)
                                                                   .MakeGenericMethod(entityType);

                try
                {
                    using (var transaction = new TransactionScope(TransactionScopeOption.RequiresNew, DefaultTransactionOptions.Default))
                    {
                        var entity = method.Invoke(this, null);
                        _tracer.InfoFormat("Query for entity type {0, -60} succeeded with {1} value", entityType.Name, entity == null ? "null" : "not null");

                        transaction.Complete();
                    }
                }
                catch (Exception e)
                {
                    succeeded = false;
                    _tracer.ErrorFormat(e, "Query for entity type {0, -60} failed", entityType);
                }
            }

            return succeeded;
        }

        private static string ResolveMethodName(Type entityType)
        {
            return typeof(IPartable).IsAssignableFrom(entityType) ? "CallFindOne" : "CallFind";
        }

        // ReSharper disable once UnusedMember.Local
        private TEntity CallFor<TEntity>() where TEntity : class, IEntity
        {
            return _query.For<TEntity>().FirstOrDefault();
        }

        // ReSharper disable once UnusedMember.Local
        private TEntity CallFindOne<TEntity>() where TEntity : class, IEntity, IEntityKey
        {
            var id = _query.For<TEntity>().Select(x => x.Id).FirstOrDefault();
            return _finder.Find(new FindSpecification<TEntity>(x => x.Id == id)).One();
        }
    }
}
using System;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Base
{
    public abstract class UseModelEntityTestBase<TEntity> : IIntegrationTest
        where TEntity : class, IEntity
    {
        private readonly IAppropriateEntityProvider<TEntity> _appropriateEntityProvider;
        private readonly Lazy<FindSpecification<TEntity>> _spec;

        protected UseModelEntityTestBase(
            IAppropriateEntityProvider<TEntity> appropriateEntityProvider)
        {
            _appropriateEntityProvider = appropriateEntityProvider;
            _spec = new Lazy<FindSpecification<TEntity>>(CreateSpec);
        }

        public ITestResult Execute()
        {
            var modelEntity = _appropriateEntityProvider.Get(ModelEntitySpec);
            if (modelEntity == null)
            {
                return OrdinaryTestResult.As.Ignored.WithReport("Can't find appropriate model entity " + typeof(TEntity).Name);
            }

            return ExecuteWithModel(modelEntity);
        }

        protected virtual FindSpecification<TEntity> ModelEntitySpec
        {
            get { return _spec.Value; }
        }

        protected abstract OrdinaryTestResult ExecuteWithModel(TEntity modelEntity);

        private static FindSpecification<TEntity> CreateSpec()
        {
            var specification = new FindSpecification<TEntity>(x => true);
            if (typeof(IDeactivatableEntity).IsAssignableFrom(typeof(TEntity)))
            {
                specification &= CreateGenericMethodCall(typeof(Specs.Find), "Active")();
            }

            if (typeof(IDeletableEntity).IsAssignableFrom(typeof(TEntity)))
            {
                specification &= CreateGenericMethodCall(typeof(Specs.Find), "NotDeleted")();
            }

            return specification;
        }

        private static Func<FindSpecification<TEntity>> CreateGenericMethodCall(Type type, string methodName)
        {
            var method = type.GetMethods().Single(x => x.Name == methodName && x.IsGenericMethod && x.IsStatic);

            var targetMethodInfo = method.GetGenericMethodDefinition().MakeGenericMethod(typeof(TEntity));

            return (Func<FindSpecification<TEntity>>)Expression.Lambda(Expression.Call(targetMethodInfo)).Compile();
        }
    }
}
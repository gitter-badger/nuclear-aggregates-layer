using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.MSpec
{
    public class UnitySutFactory<TSubject> : ISutFactory<TSubject> where TSubject : class
    {
        private IUnityContainer _ioc;

        public UnitySutFactory()
        {
            _ioc = new UnityContainer();
        }

        public TSubject Create()
        {
            return _ioc.Resolve<TSubject>();
        }

        public T AddDependency<T>(T dependency)
        {
            _ioc.RegisterInstance(dependency, new ContainerControlledLifetimeManager());

            return dependency;
        }

        public void RegisterDependency<TDependency, TImpl>() where TImpl : TDependency
        {
            _ioc.RegisterType<TDependency, TImpl>();
        }

        public T Resolve<T>()
        {
            return _ioc.Resolve<T>();
        }
    }
}
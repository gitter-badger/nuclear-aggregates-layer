using NuClear.Security.API.UserContext;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.Platform.DI.Factories
{
    public sealed class UnityUserContextProvider : IUserContextProvider
    {
        private readonly IUnityContainer _container;

        public UnityUserContextProvider(IUnityContainer container)
        {
            _container = container;
        }

        #region Implementation of IUserContextProvider

        public IUserContext Current
        {
            get
            {
                return _container.Resolve<IUserContext>();
            }
        }

        #endregion
    }
}

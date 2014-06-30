using System;
using System.Collections.Generic;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.Qds.Etl.Transform.Docs
{
    public class UnityQdsComponentsFactory : IQdsComponentsFactory
    {
        private readonly IUnityContainer _container;

        public UnityQdsComponentsFactory(IUnityContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            _container = container;
        }

        public IEnumerable<IQdsComponent> CreateQdsComponents()
        {
            /* Cut for ERM-4267
            yield return _container.Resolve<UserDocQdsComponent>();
            yield return _container.Resolve<TerritoryDocQdsComponent>();
            yield return _container.Resolve<ClientGridDocQdsComponent>();
             */
            yield return _container.Resolve<OrderGridDocQdsComponent>();
        }
    }
}
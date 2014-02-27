using System;
using System.Collections.Generic;

using DoubleGis.Erm.Qds.Etl.Transform.Docs;
using DoubleGis.Erm.Qds.Etl.Transform.EF;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.Qds.IndexService.DI
{
    class UnityQdsComponentsFactory : IQdsComponentsFactory
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
            yield return _container.Resolve<UserDocQdsComponent>();
            yield return _container.Resolve<TerritoryDocQdsComponent>();
            yield return _container.Resolve<ClientGridDocQdsComponent>();
        }
    }
}
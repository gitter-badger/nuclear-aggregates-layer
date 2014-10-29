using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import;
using DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Import.Infrastructure;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLCore.DI.Factories.Integration
{
    public class UnityDeserializeServiceBusDtoServiceFactory : IDeserializeServiceBusDtoServiceFactory
    {
        private readonly IUnityContainer _container;
        private readonly IImportMetadataProvider _importMetadataProvider;

        public UnityDeserializeServiceBusDtoServiceFactory(IImportMetadataProvider importMetadataProvider, IUnityContainer container)
        {
            _importMetadataProvider = importMetadataProvider;
            _container = container;
        }

        public bool TryGetDeserializeServiceBusObjectServices(string flowName,
                                                              string busObjectTypeName,
                                                              out IReadOnlyCollection<IDeserializeServiceBusObjectService> services)
        {
            services = null;
            if (!_importMetadataProvider.IsSupported(flowName, busObjectTypeName))
            {
                return false;
            }

            var dtoType = _importMetadataProvider.GetObjectType(flowName, busObjectTypeName);
            var serviceType = typeof(IDeserializeServiceBusObjectService<>).MakeGenericType(dtoType);
            services = _container.ResolveAll(serviceType).Cast<IDeserializeServiceBusObjectService>().ToArray();

            return true;
        }
    }
}
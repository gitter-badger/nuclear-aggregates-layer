using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import;
using DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Import.Infrastructure;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLCore.DI.Infrastructure.Integration
{
    public class UnityImportServiceBusDtoServiceFactory : IImportServiceBusDtoServiceFactory
    {
        private readonly IUnityContainer _container;
        private readonly IReadOnlyDictionary<Type, Type> _dtoTypeToImportOperationServiceMap;
        private readonly IImportMetadataProvider _importMetadataProvider;

        public UnityImportServiceBusDtoServiceFactory(IUnityContainer container,
                                                      IImportMetadataProvider importMetadataProvider,
                                                      IReadOnlyDictionary<Type, Type> dtoTypeToImportOperationServiceMap)
        {
            _importMetadataProvider = importMetadataProvider;
            _container = container;
            _dtoTypeToImportOperationServiceMap = dtoTypeToImportOperationServiceMap;
        }

        public bool TryGetServiceBusObjectImportService(string flowName, string busObjectType, out IImportServiceBusDtoService service)
        {
            service = null;
            if (!_importMetadataProvider.IsSupported(flowName, busObjectType))
            {
                return false;
            }

            var dtoType = _importMetadataProvider.GetObjectType(flowName, busObjectType);

            var serviceType = _dtoTypeToImportOperationServiceMap[dtoType];
            service = (IImportServiceBusDtoService)_container.Resolve(serviceType);

            return true;
        }
    }
}
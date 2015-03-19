using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Settings;
using DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Import.Infrastructure;
using DoubleGis.Erm.Platform.API.Core.Exceptions.ServiceBus.Import;
using DoubleGis.Erm.Platform.API.ServiceBusBroker;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Import
{
    public class ImportFromServiceBusService : IImportFromServiceBusService
    {
        private const string BrokerApiReceiverConfigurationEndpointName = "NetTcpBinding_IBrokerApiReceiver";
        private readonly IClientProxyFactory _clientProxyFactory;
        private readonly IDeserializeServiceBusDtoServiceFactory _deserializeServiceBusDtoServiceFactory;
        private readonly IImportMetadataProvider _importMetadataProvider;
        private readonly IImportServiceBusDtoServiceFactory _importServiceBusDtoServiceFactory;
        private readonly IIntegrationSettings _integrationSettings;
        private readonly ITracer _tracer;

        public ImportFromServiceBusService(IClientProxyFactory clientProxyFactory,
                                           IImportServiceBusDtoServiceFactory importServiceBusDtoServiceFactory,
                                           IImportMetadataProvider importMetadataProvider,
                                           IIntegrationSettings integrationSettings,
                                           ITracer tracer,
                                           IDeserializeServiceBusDtoServiceFactory deserializeServiceBusDtoServiceFactory)
        {
            _clientProxyFactory = clientProxyFactory;
            _importServiceBusDtoServiceFactory = importServiceBusDtoServiceFactory;
            _importMetadataProvider = importMetadataProvider;
            _integrationSettings = integrationSettings;
            _tracer = tracer;
            _deserializeServiceBusDtoServiceFactory = deserializeServiceBusDtoServiceFactory;
        }

        public void Import(string flowName)
        {
            if (!_importMetadataProvider.IsSupported(flowName))
            {
                _tracer.InfoFormat("Импорт объектов из потока {0} - поток не поддерживается", flowName);
                return;
            }

            var clientProxy = _clientProxyFactory.GetClientProxy<IBrokerApiReceiver>(BrokerApiReceiverConfigurationEndpointName);

            clientProxy.Execute(brokerApiReceiver =>
                {
                    brokerApiReceiver.BeginReceiving(_integrationSettings.IntegrationApplicationName, flowName);

                    try
                    {
                        while (true)
                        {
                            var package = brokerApiReceiver.ReceivePackage();
                            if (package == null)
                            {
                                _tracer.InfoFormat("Импорт объектов из потока {0} - шина пустая", flowName);
                                break;
                            }

                            _tracer.InfoFormat("Импорт объектов из потока {0} - загружено {1} объектов из шины", flowName, package.Length);

                            if (package.Length == 0)
                            {
                                brokerApiReceiver.Acknowledge();
                                continue;
                            }

                            var groupedObjects = ParseAndGroupByObjectType(package);
                            var deserializedObjects = DeserializeObjects(groupedObjects, flowName);
                            try
                            {
                                ProcessObjects(deserializedObjects, flowName);
                            }
                            catch (NonBlockingImportErrorException e)
                            {
                                _tracer.ErrorFormat(e, "Неблокирующая ошибка при импорте объектов из потока {0} - {1}", flowName, e.Message);
                            }

                            brokerApiReceiver.Acknowledge();
                        }
                    }
                    catch (Exception e)
                    {
                        _tracer.ErrorFormat(e, "Ошибка при импорте объектов из потока {0} - {1}", flowName, e.Message);
                        throw;
                    }
                    finally
                    {
                        brokerApiReceiver.EndReceiving();
                    }
                });
        }

        private static IEnumerable<KeyValuePair<string, IEnumerable<XElement>>> ParseAndGroupByObjectType(IEnumerable<string> package)
        {
            return package.Select(XElement.Parse).GroupBy(x => x.Name.LocalName).ToDictionary(x => x.Key, x => x.AsEnumerable());
        }

        private IEnumerable<KeyValuePair<string, IEnumerable<IServiceBusDto>>> DeserializeObjects(IEnumerable<KeyValuePair<string, IEnumerable<XElement>>> messageGroups, string flowName)
        {
            var dtoGroups = new Dictionary<string, IEnumerable<IServiceBusDto>>();
            foreach (var objectGroup in messageGroups)
            {
                var objectTypeName = objectGroup.Key;

                IReadOnlyCollection<IDeserializeServiceBusObjectService> deserializers;
                if (!_deserializeServiceBusDtoServiceFactory.TryGetDeserializeServiceBusObjectServices(flowName, objectGroup.Key, out deserializers))
                {
                    continue;
                }

                var deserializedDtos = new List<IServiceBusDto>();
                foreach (var obj in objectGroup.Value)
                {
                    var appropriateDesealizers = deserializers.Where(d => d.CanDeserialize(obj)).ToArray();
                    if (appropriateDesealizers.Length != 1)
                    {
                        throw new ServiceBusObjectDeserializerNotFoundException(
                            string.Format("Can't find appropriate deserializer for flow '{0}' and message '{1}'",
                                          flowName,
                                          objectTypeName));
                    }

                    var deserializer = appropriateDesealizers[0];

                    string report;
                    if (!deserializer.Validate(obj, out report))
                    {
                        throw new ApplicationException(report);
                    }

                    deserializedDtos.Add(deserializer.Deserialize(obj));
                }

                dtoGroups.Add(objectTypeName, deserializedDtos);
            }

            return dtoGroups;
        }

        private void ProcessObjects(IEnumerable<KeyValuePair<string, IEnumerable<IServiceBusDto>>> deserializedDtoGroups, string flowName)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                foreach (var dtoGroup in deserializedDtoGroups.OrderBy(x => _importMetadataProvider.GetProcessingOrder(flowName, x.Key)))
                {
                    var messageName = dtoGroup.Key;
                    IImportServiceBusDtoService importer;
                    if (!_importServiceBusDtoServiceFactory.TryGetServiceBusObjectImportService(flowName, messageName, out importer))
                    {
                        throw new ServiceBusObjectImportServiceNotFoundException(string.Format("Can't find import service for flow '{0}' and message '{1}'",
                                                                                               flowName,
                                                                                               messageName));
                    }

                    importer.Import(dtoGroup.Value);
                }

                transaction.Complete();
            }
        }
    }
}
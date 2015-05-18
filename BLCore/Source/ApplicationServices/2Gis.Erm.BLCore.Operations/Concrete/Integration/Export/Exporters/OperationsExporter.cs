﻿using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.ServiceBus;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.ServiceBusBroker;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Model.Common.Entities.Aspects.Integration;
using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Export.Exporters
{
    public sealed class OperationsExporter<TEntity, TProcessedOperationEntity> : IOperationsExporter<TEntity, TProcessedOperationEntity> 
        where TEntity : class, IEntity, IEntityKey
        where TProcessedOperationEntity : class, IIntegrationProcessorState
    {
        private readonly ITracer _tracer;
        private readonly IPublicService _publicService;
        private readonly IClientProxyFactory _clientProxyFactory;
        private readonly IIntegrationSettings _integrationSettings;

        public OperationsExporter(ITracer tracer,
                                  IClientProxyFactory clientProxyFactory,
                                  IIntegrationSettings integrationSettings,
                                  IPublicService publicService)
        {
            _tracer = tracer;
            _clientProxyFactory = clientProxyFactory;
            _integrationSettings = integrationSettings;
            _publicService = publicService;
        }

        public void ExportOperations(FlowDescription flowDescription,
                                     IEnumerable<PerformedBusinessOperation> operations,
                                     int packageSize,
                                     out IEnumerable<IExportableEntityDto> failedEntites)
        {
            var request = SerializeObjectsRequest<TEntity, TProcessedOperationEntity>.Create(flowDescription.SchemaResourceName, operations);
            var serializeObjectsResponse = ExportOperationsImpl(request, packageSize, flowDescription.FlowName);

            failedEntites = serializeObjectsResponse.FailedObjects;
        }

        public void ExportFailedEntities(FlowDescription flowDescription,
                                         IEnumerable<ExportFailedEntity> failedEntities,
                                         int packageSize,
                                         out IEnumerable<IExportableEntityDto> exportedEntites)
        {
            var request = SerializeObjectsRequest<TEntity, TProcessedOperationEntity>.Create(flowDescription.SchemaResourceName, failedEntities);
            var response = ExportOperationsImpl(request, packageSize, flowDescription.FlowName);

            exportedEntites = response.SuccessObjects;
        }

        private SerializeObjectsResponse ExportOperationsImpl(SerializeObjectsRequest<TEntity, TProcessedOperationEntity> request, int batchSize, string flowName)
        {
            try
            {
                var response = (SerializeObjectsResponse)_publicService.Handle(request);
                var objectCount = response.SerializedObjects.Count();

                for (var i = 0; i < objectCount; i += batchSize)
                {
                    var slice = response.SerializedObjects.Skip(i).Take(batchSize).ToArray();
                    WriteObjectsToServiceBus(slice, flowName);
                }

                return response;
            }
            catch (Exception e)
            {
                _tracer.FatalFormat(e, "Ошибка при экспорте сущности {0}", typeof(TEntity).Name);
                throw;
            }
        }

        private void WriteObjectsToServiceBus(IEnumerable<string> exportObjects, string flowName)
        {
            if (!_integrationSettings.EnableIntegration)
            {
                return;
            }

            // FIXME {all, 15.08.2013}: Вынести "NetTcpBinding_IBrokerApiSender" в настройки
            var clientProxy = _clientProxyFactory.GetClientProxy<IBrokerApiSender>("NetTcpBinding_IBrokerApiSender");

            clientProxy.Execute(brokerApiSender =>
                {
                    brokerApiSender.BeginSending(_integrationSettings.IntegrationApplicationName, flowName);

                    foreach (var serializedObject in exportObjects)
                    {
                        brokerApiSender.SendDataObject(serializedObject);
                    }

                    brokerApiSender.Commit();
                    brokerApiSender.EndSending();
                });
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Assign;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Assign;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Operations;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Settings;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.APIInteraction.Operations.Assign
{
    public sealed class SoapApiAssignGenericEntityService<TEntity> : SoapApiOperationEntitySpecificServiceBase<TEntity>, 
                                                                     IAssignGenericEntityService<TEntity>,
                                                                     IGroupAssignGenericEntityService<TEntity>
        where TEntity : class, IEntityKey, ICuratedEntity, new()
    {
        private readonly IOperationProgressCallback _operationProgressCallback;

        public SoapApiAssignGenericEntityService(
            IOperationProgressCallback operationProgressCallback,
            IDesktopClientProxyFactory clientProxyFactory,
            IStandartConfigurationSettings configuration,
            IApiSettings apiSettings,
            ICommonLog logger)
            : base(clientProxyFactory, configuration, apiSettings, logger)
        {
            _operationProgressCallback = operationProgressCallback;
        }

        public AssignResult Assign(long entityId, long ownerCode, bool bypassValidation, bool isPartialAssign)
        {
            var serviceProxy = ClientProxyFactory.GetClientProxy<IAssignApplicationService, WSHttpBinding>();

            try
            {
                return serviceProxy.Execute(x => x.Execute(EntityName, entityId, ownerCode, bypassValidation, isPartialAssign));
            }
            catch (FaultException<AssignOperationErrorDescription> ex)
            {
                Logger.Error(ex, "Can't assign entity " + EntityType.Name + ". Entity id: " + entityId);
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Can't assign entity " + EntityType.Name + ". Entity id: " + entityId);
                throw;
            }
        }

        public IEnumerable<AssignResult> Assign(AssignCommonParameter operationParameter, IEnumerable<AssignEntityParameter> operationItemParameters)
        {
            var serviceProxy = ClientProxyFactory.GetDuplexClientProxy<IGroupAssignApplicationService>(_operationProgressCallback);

            try
            {
                return serviceProxy.Execute(x => x.Assign(operationParameter, operationItemParameters.ToArray()));
            }
            catch (FaultException<AssignOperationErrorDescription> ex)
            {
                Logger.Error(ex, "Can't execute group assign entity " + EntityType.Name);
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Can't execute group assign entity " + EntityType.Name);
                throw;
            }
        }
    }
}
﻿using System;
using System.ServiceModel;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Remote.CreateOrUpdate;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Operations;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Settings;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.APIInteraction.Operations.Modify
{
    public sealed class SoapApiModifySimplifiedModelEntityService<TEntity> : SoapApiOperationEntitySpecificServiceBase<TEntity>, IModifySimplifiedModelEntityService<TEntity>
        where TEntity : class, IEntityKey, IEntity, new()
    {
        public SoapApiModifySimplifiedModelEntityService(
            IDesktopClientProxyFactory clientProxyFactory,
            IStandartConfigurationSettings configuration,
            IApiSettings apiSettings,
            ICommonLog logger)
            : base(clientProxyFactory, configuration, apiSettings, logger)
        {
        }

        public long Modify(IDomainEntityDto domainEntityDto)
        {
            var serviceProxy = ClientProxyFactory.GetClientProxy<ICreateOrUpdateApplicationService, WSHttpBinding>();

            try
            {
                return serviceProxy.Execute(x => x.Execute(EntityName, domainEntityDto));
            }
            catch (FaultException<CreateOrUpdateOperationErrorDescription> ex)
            {
                Logger.ErrorEx(ex, "Can't modify entity " + EntityType.Name + ". Entity id: " + domainEntityDto.Id);
                throw;
            }
            catch (Exception ex)
            {
                Logger.ErrorEx(ex, "Can't modify entity " + EntityType.Name + ". Entity id: " + domainEntityDto.Id);
                throw;
            }
        }
    }
}
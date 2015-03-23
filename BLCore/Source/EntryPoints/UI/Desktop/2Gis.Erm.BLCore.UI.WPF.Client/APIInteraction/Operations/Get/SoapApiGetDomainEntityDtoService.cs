using System;
using System.ServiceModel;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Get;
using DoubleGis.Erm.BLCore.API.Operations.Remote.GetDomainEntityDto;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Operations;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Settings;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.APIInteraction.Operations.Get
{
    public sealed class SoapApiGetDomainEntityDtoService<TEntity> : SoapApiOperationEntitySpecificServiceBase<TEntity>, IGetDomainEntityDtoService<TEntity>
        where TEntity : class, IEntityKey, new()
    {
        public SoapApiGetDomainEntityDtoService(
            IDesktopClientProxyFactory clientProxyFactory, 
            IStandartConfigurationSettings configuration, 
            IApiSettings apiSettings,
            ITracer tracer)
            : base(clientProxyFactory, configuration, apiSettings, tracer)
        {
        }

        public IDomainEntityDto GetDomainEntityDto(long entityId, bool readOnly, long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {
            var getDomainEntityDtoServiceProxy = ClientProxyFactory.GetClientProxy<IGetDomainEntityDtoApplicationService, WSHttpBinding>();

            try
            {
                return getDomainEntityDtoServiceProxy.Execute(x => x.GetDomainEntityDto(EntityName, entityId));
            }
            catch (FaultException<GetDomainEntityDtoOperationErrorDescription> ex)
            {
                Tracer.Error(ex, "Can't get dto for entity " + EntityType.Name + ". Entity id: " + entityId);
                throw;
            }
            catch (Exception ex)
            {
                Tracer.Error(ex, "Can't get dto for entity " + EntityType.Name + ". Entity id: " + entityId);
                throw;
            }
        }
    }
}
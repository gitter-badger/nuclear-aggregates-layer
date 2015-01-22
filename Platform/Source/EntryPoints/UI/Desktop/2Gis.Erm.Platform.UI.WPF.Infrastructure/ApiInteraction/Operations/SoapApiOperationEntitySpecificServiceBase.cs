using System;

using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Settings;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Operations
{
    public abstract class SoapApiOperationEntitySpecificServiceBase<TEntity> : SoapApiOperationServiceBase
        where TEntity : class, IEntityKey
    {
        private readonly Type _entityType;
        private readonly IEntityType _entityName;

        protected SoapApiOperationEntitySpecificServiceBase(
            IDesktopClientProxyFactory clientProxyFactory,
            IStandartConfigurationSettings configuration,
            IApiSettings apiSettings,
            ICommonLog logger)
            : base(clientProxyFactory, configuration, apiSettings, logger)
        {
            _entityType = typeof(TEntity);
            _entityName = EntityType.AsEntityName();
        }

        protected Type EntityType
        {
            get
            {
                return _entityType;
            }
        }

        protected IEntityType EntityName
        {
            get
            {
                return _entityName;
            }
        }
    }
}
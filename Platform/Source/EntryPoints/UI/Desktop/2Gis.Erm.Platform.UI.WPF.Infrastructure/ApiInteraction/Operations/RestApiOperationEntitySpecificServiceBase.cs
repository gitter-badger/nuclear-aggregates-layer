using System;

using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Infrastructure;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Tracing.API;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Operations
{
    public abstract class RestApiOperationEntitySpecificServiceBase<TEntity> : RestApiOperationServiceBase
        where TEntity : class, IEntityKey
    {
        private readonly Type _entityType;
        private readonly IEntityType _entityName;

        protected RestApiOperationEntitySpecificServiceBase(IApiClient apiClient, ITracer tracer, string operationApiTargetResource)
            : base(apiClient, tracer, operationApiTargetResource)
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
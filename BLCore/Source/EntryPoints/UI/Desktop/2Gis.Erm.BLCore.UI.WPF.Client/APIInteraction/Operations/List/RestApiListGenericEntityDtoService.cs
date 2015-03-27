using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Infrastructure;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.APIInteraction.Operations.List
{
    public sealed class RestApiListGenericEntityDtoService<TEntity, TEntityDto> : RestApiListEntityServiceBase<TEntity>, IListGenericEntityDtoService<TEntity, TEntityDto> 
        where TEntity : class, IEntityKey 
        where TEntityDto : IOperationSpecificEntityDto
    {
        public RestApiListGenericEntityDtoService(IApiClient apiClient, ITracer tracer)
            : base(apiClient, tracer)
        {
        }

        #region Implementation of IListEntityService

        public override IRemoteCollection List(SearchListModel searchListModel)
        {
            var list = (EntityDtoListResult)GetList(searchListModel);
            return new RemoteCollection<TEntityDto>(list.Data.Cast<TEntityDto>().ToList(), list.RowCount);
        }

        #endregion
    }
}

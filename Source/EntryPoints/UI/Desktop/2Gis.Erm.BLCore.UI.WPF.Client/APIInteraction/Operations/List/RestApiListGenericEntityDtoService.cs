using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Infrastructure;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.APIInteraction.Operations.List
{
    public sealed class RestApiListGenericEntityDtoService<TEntity, TEntityDto> : RestApiListEntityServiceBase<TEntity>, IListGenericEntityDtoService<TEntity, TEntityDto> 
        where TEntity : class, IEntityKey 
        where TEntityDto : IOperationSpecificEntityDto
    {
        public RestApiListGenericEntityDtoService(IApiClient apiClient, ICommonLog logger)
            : base(apiClient, logger)
        {
        }

        #region Implementation of IListEntityService

        public override ListResult List(SearchListModel searchListModel)
        {
            return GetList(searchListModel);
        }

        #endregion
    }
}

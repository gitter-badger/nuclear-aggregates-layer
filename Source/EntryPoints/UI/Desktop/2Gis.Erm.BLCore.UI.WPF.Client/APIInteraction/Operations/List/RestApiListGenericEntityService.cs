using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Infrastructure;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.APIInteraction.Operations.List
{
    public sealed class RestApiListGenericEntityService<TEntity> : RestApiListEntityServiceBase<TEntity>, IListGenericEntityService<TEntity>
        where TEntity : class, IEntityKey
    {
        public RestApiListGenericEntityService(IApiClient apiClient, ICommonLog logger)
            : base(apiClient, logger)
        {
        }

        public override ListResult List(SearchListModel searchListModel)
        {
            return GetList(searchListModel);
        }
    }
}
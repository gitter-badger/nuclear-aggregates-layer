using System.ServiceModel;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.Remote.List;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.APIInteraction.Operations.List
{
    public class SoapApiListGenericEntityService<TEntity> : IListGenericEntityService<TEntity> where TEntity : class, IEntityKey
    {
        private readonly IDesktopClientProxyFactory _clientProxyFactory;

        public SoapApiListGenericEntityService(IDesktopClientProxyFactory clientProxyFactory)
        {
            _clientProxyFactory = clientProxyFactory;
        }

        public ListResult List(SearchListModel searchListModel)
        {
            var listAppServiceProxy = _clientProxyFactory.GetClientProxy<IListApplicationService, WSHttpBinding>();
            return listAppServiceProxy.Execute(x => x.Execute(typeof(TEntity).AsEntityName(),
                                                               searchListModel.WhereExp,
                                                               searchListModel.Start,
                                                               searchListModel.FilterInput,
                                                               searchListModel.ExtendedInfo,
                                                               searchListModel.NameLocaleResourceId,
                                                               searchListModel.Limit,
                                                               searchListModel.Dir,
                                                               searchListModel.Sort,
                                                               searchListModel.PId.ToString(),
                                                               searchListModel.PType));
        }
    }
}
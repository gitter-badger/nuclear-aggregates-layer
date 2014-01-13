﻿using System.ServiceModel;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLCore.API.Operations.Remote.List;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.APIInteraction.Operations.List
{
    public class SoapApiListNonGenericEntityService : IListNonGenericEntityService
    {
        private readonly IDesktopClientProxyFactory _clientProxyFactory;

        public SoapApiListNonGenericEntityService(IDesktopClientProxyFactory clientProxyFactory)
        {
            _clientProxyFactory = clientProxyFactory;
        }

        public ListResult List(EntityName entityName, SearchListModel searchListModel)
        {
            var listAppServiceProxy = _clientProxyFactory.GetClientProxy<IListApplicationService, WSHttpBinding>();
            return listAppServiceProxy.Execute(x => x.Execute(entityName,
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
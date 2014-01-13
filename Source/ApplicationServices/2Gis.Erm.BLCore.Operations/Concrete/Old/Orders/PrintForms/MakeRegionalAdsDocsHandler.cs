using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.ServiceModel;

using DoubleGis.Erm.BLCore.Aggregates.Releases.ReadModel;
using DoubleGis.Erm.BLCore.API.MoDi;
using DoubleGis.Erm.BLCore.API.MoDi.PrintRegional;
using DoubleGis.Erm.BLCore.API.MoDi.Remote.PrintRegional;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Operations.Concrete.Old.Journal.Concrete;
using DoubleGis.Erm.BLCore.Operations.Concrete.Old.Journal.Infrastructure;
using DoubleGis.Erm.BLCore.Operations.Concrete.Old.Journal.Infrastructure.PolicyInjection;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Compression;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Orders.PrintForms
{
    // FIXME {all, 23.10.2013}: требуется избавиться от аспектного логирования, т.к. в данном случае логирование требование business case, а не инвраструктуры, то и вызывать логирование нужно явно, а не через мутную инфраструктуру Interception
    public class MakeRegionalAdsDocsHandler : RequestHandler<MakeRegionalAdsDocsRequest, StreamResponse>, IPropertyBagContainer 
    {
        private readonly ISecurityServiceFunctionalAccess _securityServiceFunctionalAccess;
        private readonly IPropertyBag _propertyBag;
        private readonly IUserContext _userContext;

        private readonly IReleaseReadModel _releaseRepository;
        private readonly IPrintFormService _printFormService;
        private readonly ISubRequestProcessor _requestProcessor;
        private readonly IClientProxyFactory _clientProxyFactory;

        public MakeRegionalAdsDocsHandler(ISecurityServiceFunctionalAccess securityServiceFunctionalAccess,
                                            IPropertyBag propertyBag,
                                            IUserContext userContext,
                                            IReleaseReadModel releaseRepository,
                                            IClientProxyFactory clientProxyFactory,
                                            IPrintFormService printFormService,
                                            ISubRequestProcessor requestProcessor)
        {
            _securityServiceFunctionalAccess = securityServiceFunctionalAccess;
            _propertyBag = propertyBag;
            _userContext = userContext;
            _releaseRepository = releaseRepository;
            _clientProxyFactory = clientProxyFactory;
            _printFormService = printFormService;
            _requestProcessor = requestProcessor;
        }

        public IPropertyBag PropertyBag
        {
            get { return _propertyBag; }
        }

        [JournalBusinessOperation(BusinessOperation.MakeRegionalAdsDocs)]
        protected override StreamResponse Handle(MakeRegionalAdsDocsRequest request)
        {
            Validate(request);

            var clientProxy = _clientProxyFactory.GetClientProxy<IPrintRegionalApplicationService, WSHttpBinding>();

            var response = clientProxy.Execute(service => service.PrintRegionalOrders(
                                request.SourceOrganizationUnitId,
                                request.StartPeriodDate,
                                request.StartPeriodDate.GetEndPeriodOfThisMonth()));
            if (response.Items.Length == 0)
            {
                throw new NotificationException("По указанным параметрам заказов не найдено");
            }

            SetJournalProperties(request, response);

            var streamDictionary = response.Items.Select(x => x.File).ToDictionary<FileDescription, string, Stream>(x => "Заказы\\" + x.FileName, x => new MemoryStream(x.Stream));

            foreach (var responseItem in response.Items)
            {
                var orderStreams = new List<Stream>();
                foreach (var firmWithOrders in responseItem.FirmWithOrders)
                {
                    var streamResponse = (StreamResponse)_requestProcessor.HandleSubRequest(new PrintReferenceInformationRequest
                    {
                        OrderId = firmWithOrders.OrderIds.First(),
                        LegalPersonProfileId = 0, // потом подумать что тут должно быть
                    }, Context);

                    orderStreams.Add(streamResponse.Stream);
                }

                var orderStream = _printFormService.MergeDocuments(orderStreams);

                streamDictionary.Add("Справочная информация\\" + responseItem.DestOrganizationUnitSyncCode1C + ".docx", orderStream);
            }

            return new StreamResponse
            {
                FileName = "RegionalOrders.zip",
                ContentType = MediaTypeNames.Application.Zip,
                Stream = streamDictionary.ZipStreamDictionary(),
            };
        }

        private void Validate(MakeRegionalAdsDocsRequest request)
        {
            var hasPriviledge = _securityServiceFunctionalAccess.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.MakeRegionalAdsDocs, _userContext.Identity.Code);
            if (!hasPriviledge)
            {
                throw new NotificationException(BLResources.AccessDenied);
            }

            var sourceOrgUnitHasReleases = _releaseRepository.HasSuccededFinalReleaseFromDate(request.SourceOrganizationUnitId, request.StartPeriodDate);
            if (!sourceOrgUnitHasReleases)
            {
                throw new NotificationException(BLResources.NoFinalReleaseForSourceOrgUnit);
            }
        }

        private void SetJournalProperties(MakeRegionalAdsDocsRequest request, PrintRegionalOrdersResponse response)
        {
            _propertyBag.SetProperty(JournalMakeRegionalAdsDocsProperties.MakeRegionalAdsDocsRequest, request);
            _propertyBag.SetProperty(JournalMakeRegionalAdsDocsProperties.MoneyDistributionResponse, response);
        }
    }
}

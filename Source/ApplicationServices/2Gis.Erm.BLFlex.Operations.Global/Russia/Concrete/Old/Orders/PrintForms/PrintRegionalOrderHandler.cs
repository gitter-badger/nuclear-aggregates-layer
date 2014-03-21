using System.IO;
using System.Linq;
using System.Net.Mime;
using System.ServiceModel;

using DoubleGis.Erm.BLCore.API.MoDi;
using DoubleGis.Erm.BLCore.API.MoDi.Remote.PrintRegional;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Common.Compression;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;

using BLFlexResources = DoubleGis.Erm.BLFlex.Resources.Server.Properties.BLResources;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Concrete.Old.Orders.PrintForms
{
    public sealed class PrintRegionalOrderHandler : RequestHandler<PrintRegionalOrderRequest, StreamResponse>, IRussiaAdapted
    {
        private readonly IFinder _finder;
        private readonly IClientProxyFactory _clientProxyFactory;

        public PrintRegionalOrderHandler(IFinder finder, IClientProxyFactory clientProxyFactory)
        {
            _finder = finder;
            _clientProxyFactory = clientProxyFactory;
        }

        protected override StreamResponse Handle(PrintRegionalOrderRequest request)
        {
            var orderInfo = _finder.Find(Specs.Find.ById<Order>(request.OrderId))
                .Select(order => new
                                 {
                                     order.BranchOfficeOrganizationUnitId,
                                     order.RegionalNumber,
                                 })
                .Single();

            if (orderInfo.BranchOfficeOrganizationUnitId == null)
            {
                throw new NotificationException(BLFlexResources.OrderHasNoBranchOfficeOrganizationUnit);
            }

            return PrintRegionalOrder(request.OrderId, orderInfo.RegionalNumber);
        }

        private StreamResponse PrintRegionalOrder(long orderId, string orderRegionalNumber)
        {
            var clientProxy = _clientProxyFactory.GetClientProxy<IPrintRegionalApplicationService, WSHttpBinding>();

            var response = clientProxy.Execute(service => service.PrintRegionalOrder(orderId));
            if (response.Items.Length == 0)
            {
                throw new NotificationException(BLFlexResources.OrderTotalAmountIsZero);
            }

            var streamResponse = new StreamResponse();
            if (response.Items.Length == 1)
            {
                var file = response.Items.First().File;

                streamResponse.FileName = file.FileName;
                streamResponse.ContentType = file.ContentType;
                streamResponse.Stream = new MemoryStream(file.Stream);
                return streamResponse;
            }

            var streamDictionary = response.Items.Select(x => x.File).ToDictionary<FileDescription, string, Stream>(x => x.FileName, x => new MemoryStream(x.Stream));
            streamResponse.FileName = orderRegionalNumber + ".zip";
            streamResponse.ContentType = MediaTypeNames.Application.Zip;
            streamResponse.Stream = streamDictionary.ZipStreamDictionary();
            return streamResponse;
        }
    }
}
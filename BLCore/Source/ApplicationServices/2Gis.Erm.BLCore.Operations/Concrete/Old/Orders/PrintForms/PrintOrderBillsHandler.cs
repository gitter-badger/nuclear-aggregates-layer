using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bills;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Common.Compression;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Orders.PrintForms
{
    public sealed class PrintOrderBillsHandler : RequestHandler<PrintOrderBillsRequest, StreamResponse>
    {
        private readonly IFinder _finder;
        private readonly ISubRequestProcessor _subRequestProcessor;

        public PrintOrderBillsHandler(IFinder finder, ISubRequestProcessor subRequestProcessor)
        {
            _finder = finder;
            _subRequestProcessor = subRequestProcessor;
        }

        protected override StreamResponse Handle(PrintOrderBillsRequest request)
        {
            var orderInfo = _finder.Find(Specs.Find.ById<Order>(request.OrderId))
                .Select(order => new
                                 {
                                     order.Number,
                                     BillIds = order.Bills.Where(bill => bill.IsActive && !bill.IsDeleted).Select(bill => bill.Id)
                                 })
                .SingleOrDefault();

            if (orderInfo == null || !orderInfo.BillIds.Any())
            {
                throw new NotificationException(BLResources.BillsMissing);
            }

            var billPrintForms = new Dictionary<string, Stream>();

            foreach (var billId in orderInfo.BillIds)
            {
                var streamResponse = (StreamResponse)_subRequestProcessor.HandleSubRequest(new PrintBillRequest { BillId = billId }, Context);
                billPrintForms[streamResponse.FileName] = streamResponse.Stream;
            }

            var zipStream = billPrintForms.ZipStreamDictionary();

            foreach (var stream in billPrintForms.Values)
                stream.Dispose();
            billPrintForms.Clear();

            return new StreamResponse
            {
                Stream = zipStream,
                ContentType = MediaTypeNames.Application.Zip,
                FileName = string.Format(BLResources.BillFileNameTemplate, orderInfo.Number)
            };
        }
    }
}
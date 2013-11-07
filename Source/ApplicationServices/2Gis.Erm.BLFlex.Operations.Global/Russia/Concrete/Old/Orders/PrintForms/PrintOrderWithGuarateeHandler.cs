using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;

using DoubleGis.Erm.BL.Aggregates.Orders;
using DoubleGis.Erm.BL.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BL.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BL.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Common.Compression;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Concrete.Old.Orders.PrintForms
{
    public sealed class PrintOrderWithGuarateeHandler : RequestHandler<PrintOrderWithGuarateeRequest, Response>, IRussiaAdapted
    {
        private readonly ISubRequestProcessor _requestProcessor;
        private readonly IOrderRepository _orderRepository;

        public PrintOrderWithGuarateeHandler(ISubRequestProcessor requestProcessor, IOrderRepository orderRepository)
        {
            _requestProcessor = requestProcessor;
            _orderRepository = orderRepository;
        }

        protected override Response Handle(PrintOrderWithGuarateeRequest request)
        {
            var orderRequest = new PrintOrderRequest
                {
                    OrderId = request.OrderId,
                    LegalPersonProfileId = request.LegalPersonProfileId,
                    PrintRegionalVersion = request.PrintRegionalVersion
                };

            var letterRequest = new PrintLetterOfGuaranteeRequest
                {
                    OrderId = request.OrderId,
                    LegalPersonProfileId = request.LegalPersonProfileId
                };

            var order = _orderRepository.GetOrder(request.OrderId);
            return new StreamResponse
            {
                Stream = ProcessRequests(orderRequest, letterRequest).ZipStreamDictionary(),
                ContentType = MediaTypeNames.Application.Zip,
                FileName = string.Format("{0}.zip", order.Number),
            };
        }

        private Dictionary<string, Stream> ProcessRequests(params Request[] requests)
        {
            return requests
                .Select(request => (StreamResponse)_requestProcessor.HandleSubRequest(request, Context))
                .ToDictionary(response => response.FileName, response => response.Stream);
        }
    }
}
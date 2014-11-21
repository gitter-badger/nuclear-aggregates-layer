using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Common.Compression;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Concrete.Old.Orders.PrintForms
{
    public sealed class PrintOrderWithGuarateeHandler : RequestHandler<PrintOrderWithGuarateeRequest, Response>, IRussiaAdapted, IKazakhstanAdapted
    {
        private readonly ISubRequestProcessor _requestProcessor;
        private readonly IOrderReadModel _orderReadModel;

        public PrintOrderWithGuarateeHandler(ISubRequestProcessor requestProcessor, IOrderReadModel orderReadModel)
        {
            _requestProcessor = requestProcessor;
            _orderReadModel = orderReadModel;
        }

        protected override Response Handle(PrintOrderWithGuarateeRequest request)
        {
            var orderRequest = new PrintOrderRequest
            {
                OrderId = request.OrderId,
            };

            var letterRequest = new PrintLetterOfGuaranteeRequest
            {
                OrderId = request.OrderId,
            };

            var order = _orderReadModel.GetOrderSecure(request.OrderId);
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

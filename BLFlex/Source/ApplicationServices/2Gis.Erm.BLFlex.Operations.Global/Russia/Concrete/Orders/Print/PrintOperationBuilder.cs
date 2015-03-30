using System;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders.Print;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Concrete.Orders.Print
{
    public class PrintOperationBuilder
    {
        private Func<IOperationScope> _scopeFactory;
        private Func<PrintDocumentRequest> _requestFactory;
        private Func<PrintDocumentRequest, Response> _func;
        private TemplateCode _templateCode;

        public PrintOperationBuilder UseScope(Func<IOperationScope> scopeFactory)
        {
            _scopeFactory = scopeFactory;
            return this;
        }

        public PrintOperationBuilder UseTemplate(TemplateCode templateCode)
        {
            _templateCode = templateCode;
            return this;
        }

        public PrintOperationBuilder UseData(Func<PrintDocumentRequest> requestFactory)
        {
            _requestFactory = requestFactory;
            return this;
        }

        public PrintOperationBuilder UsePublicService(Func<PrintDocumentRequest, Response> func)
        {
            _func = func;
            return this;
        }

        public PrintFormDocument Execute()
        {
            using (var scope = _scopeFactory())
            {
                var printDocumentRequest = _requestFactory();
                printDocumentRequest.TemplateCode = _templateCode;
                var response = (StreamResponse)_func(printDocumentRequest);
                scope.Complete();

                return new PrintFormDocument
                           {
                               Stream = response.Stream,
                               ContentType = response.ContentType,
                               FileName = response.FileName,
                           };
            }
        }
    }
}
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.OneC;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.Common.Compression;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Integration.OneC
{
    [UseCase(Duration = UseCaseDuration.VeryLong)]
    public sealed class ExportAccountDetailsTo1CForFranchiseesHandler : RequestHandler<ExportAccountDetailsTo1CForFranchiseesRequest, IntegrationResponse>
    {
        private readonly IClientProxyFactory _clientProxyFactory;

        public ExportAccountDetailsTo1CForFranchiseesHandler(
            IClientProxyFactory clientProxyFactory)
        {
            _clientProxyFactory = clientProxyFactory;
        }

        protected override IntegrationResponse Handle(ExportAccountDetailsTo1CForFranchiseesRequest request)
        {
            var streamDictionary = new Dictionary<string, Stream>();

            var modiResponse = AccountDetailsFrom1CHelper.ExportRegionalAccountDetails(
                                _clientProxyFactory,
                                request.OrganizationUnitId,
                                request.StartPeriodDate,
                                request.EndPeriodDate);

            if (modiResponse.File != null)
            {
                streamDictionary.Add(modiResponse.File.FileName, new MemoryStream(modiResponse.File.Stream));
            }

            if (modiResponse.ErrorFile != null)
            {
                streamDictionary.Add(modiResponse.ErrorFile.FileName, new MemoryStream(modiResponse.ErrorFile.Stream));
            }

                return new IntegrationResponse
                {
                FileName = "Acts.zip",
                ContentType = MediaTypeNames.Application.Zip,
                Stream = streamDictionary.ZipStreamDictionary(),

                ProcessedWithoutErrors = modiResponse.ProcessedWithoutErrors,
                BlockingErrorsAmount = modiResponse.BlockingErrorsAmount,
                NonBlockingErrorsAmount = modiResponse.NonBlockingErrorsAmount,
                           };
            }
    }
}

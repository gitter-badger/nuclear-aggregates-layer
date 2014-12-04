using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Text;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.OrganizationUnits.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.AccountDetails.Dto;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.OneC;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.Common.Compression;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Integration.OneC
{
    [UseCase(Duration = UseCaseDuration.VeryLong)]
    public sealed class ExportAccountDetailsToServiceBusForFranchiseesHandler : RequestHandler<ExportAccountDetailsToServiceBusForFranchiseesRequest, IntegrationResponse>
    {
        private readonly IOrganizationUnitReadModel _organizationUnitReadModel;

        public ExportAccountDetailsToServiceBusForFranchiseesHandler(IOrganizationUnitReadModel organizationUnitReadModel)
        {
            _organizationUnitReadModel = organizationUnitReadModel;
        }

        protected override IntegrationResponse Handle(ExportAccountDetailsToServiceBusForFranchiseesRequest request)
        {
            var streamDictionary = new Dictionary<string, Stream>();

            // Региональные списания мы уже не выгружаем
            // Клиентские списания мы еще не выгружаем
            // Договорились с Олегом, что в переходный период будем выгружать пустой объект
            var emptyData = new DebitsInfoForErpDto
                {
                    OrganizationUnitCode = _organizationUnitReadModel.GetSyncCode(request.OrganizationUnitId),
                    EndDate = request.EndPeriodDate,
                    StartDate = request.StartPeriodDate,
                    Debits = new DebitDto[0]
                };

            streamDictionary.Add("DebitsInfoForERP_" + DateTime.Today.ToShortDateString() + ".xml",
                                 new MemoryStream(Encoding.UTF8.GetBytes(emptyData.ToXElement().ToString(SaveOptions.None))));

            return new IntegrationResponse
            {
                FileName = "Acts.zip",
                ContentType = MediaTypeNames.Application.Zip,
                Stream = streamDictionary.ZipStreamDictionary(),

                ProcessedWithoutErrors = 0,
                BlockingErrorsAmount = 0,
                NonBlockingErrorsAmount = 0,
            };
        }
    }
}
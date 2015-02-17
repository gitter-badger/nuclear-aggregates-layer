using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.AccountDetails.Dto;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Common.Compression;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.AccountDetail;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Export
{
    public sealed class GetAccountDetailsForExportContentOperationService : IGetAccountDetailsForExportContentOperationService
    {
        private readonly IGetDebitsInfoInitialForExportOperationService _getDebitsInfoInitialForExportOperationService;
       
        private readonly IOperationScopeFactory _operationScopeFactory;

        public GetAccountDetailsForExportContentOperationService(
            IGetDebitsInfoInitialForExportOperationService getDebitsInfoInitialForExportOperationService,
            IOperationScopeFactory operationScopeFactory)
        {
            _getDebitsInfoInitialForExportOperationService = getDebitsInfoInitialForExportOperationService;
            _operationScopeFactory = operationScopeFactory;
        }

        public IEnumerable<IntegrationResponse> Get(DateTime startPeriodDate, DateTime endPeriodDate, IEnumerable<long> organizationUnitIds)
        {
            using (var scope = _operationScopeFactory.CreateNonCoupled<GetAccountDetailsForExportContentIdentity>())
            {
                var debitsInfoInitialsByOrganizationUnit = _getDebitsInfoInitialForExportOperationService.Get(startPeriodDate,
                                                                                                              endPeriodDate,
                                                                                                              organizationUnitIds);

                scope.Complete();

                return organizationUnitIds.Select(organizationUnitId => ConstructResponse(debitsInfoInitialsByOrganizationUnit[organizationUnitId]));
            }
        }

        private IntegrationResponse ConstructResponse(DebitsInfoInitialDto debitsInfoInitial)
        {
            var streamDictionary = new Dictionary<string, Stream>();
            var debitsStream = new MemoryStream(Encoding.UTF8.GetBytes(debitsInfoInitial.ToXElement().ToString(SaveOptions.None)));
            streamDictionary.Add("DebitsInfoInitial_" + DateTime.Today.ToShortDateString() + ".xml", debitsStream);

            var response = new IntegrationResponse();
            response.FileName = "Acts.zip";
            response.ContentType = MediaTypeNames.Application.Zip;
            response.Stream = streamDictionary.ZipStreamDictionary();
            
            response.ProcessedWithoutErrors = debitsInfoInitial.Debits.Count();

            return response;
        }
    }
}
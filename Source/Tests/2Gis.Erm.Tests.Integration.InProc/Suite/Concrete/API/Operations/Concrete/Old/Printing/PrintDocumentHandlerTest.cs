using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Old.Printing
{
    public class PrintDocumentHandlerTest : UseModelEntityHandlerTestBase<BranchOfficeOrganizationUnit, PrintDocumentRequest, StreamResponse>
    {
        public PrintDocumentHandlerTest(IPublicService publicService, IAppropriateEntityProvider<BranchOfficeOrganizationUnit> appropriateEntityProvider)
            : base(publicService, appropriateEntityProvider)
        {
        }

        protected override bool TryCreateRequest(BranchOfficeOrganizationUnit modelEntity, out PrintDocumentRequest request)
        {
            request = new PrintDocumentRequest
                {
                    BranchOfficeOrganizationUnitId = modelEntity.Id,
                    CurrencyIsoCode = 643,
                    FileName = "Test",
                    PrintData = new object(),
                    TemplateCode = TemplateCode.ReferenceInformation
                };

            return true;
        }

        protected override IResponseAsserter<StreamResponse> ResponseAsserter
        {
            get { return new StreamResponseAsserter(); }
        }
    }
}
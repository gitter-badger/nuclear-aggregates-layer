using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Limits;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Old.Limits
{
    public class PrintLimitsHandlerTest : UseModelEntityHandlerTestBase<Limit, PrintLimitsRequest, StreamResponse>
    {
        public PrintLimitsHandlerTest(IPublicService publicService, IAppropriateEntityProvider<Limit> appropriateEntityProvider)
            : base(publicService, appropriateEntityProvider)
        {
        }

        protected override FindSpecification<Limit> ModelEntitySpec
        {
            get
            {
                return base.ModelEntitySpec &&
                       new FindSpecification<Limit>(
                           l => l.Account.BranchOfficeOrganizationUnit.PrintFormTemplates.Any(
                               pft => pft.IsActive && !pft.IsDeleted && pft.TemplateCode == TemplateCode.LimitRequest));
            }
        }

        protected override bool TryCreateRequest(Limit modelEntity, out PrintLimitsRequest request)
        {
            request = new PrintLimitsRequest
                {
                    LimitIds = new[] { modelEntity.Id }
                };

            return true;
        }

        protected override IResponseAsserter<StreamResponse> ResponseAsserter
        {
            get { return new StreamResponseAsserter(); }
        }
    }
}
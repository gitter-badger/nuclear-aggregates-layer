using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.OneC;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Old.LegalPersons
{
    public class ExportLegalPersonsHandlerTest : UseModelEntityHandlerTestBase<OrganizationUnit, ExportLegalPersonsRequest, IntegrationResponse>
    {
        private readonly DateTime _startDate = DateTime.Now.AddMonths(-1).GetFirstDateOfMonth();

        public ExportLegalPersonsHandlerTest(IPublicService publicService, IAppropriateEntityProvider<OrganizationUnit> appropriateEntityProvider)
            : base(publicService, appropriateEntityProvider)
        {
        }

        protected override FindSpecification<OrganizationUnit> ModelEntitySpec
        {
            get
            {
                return Specs.Find.ActiveAndNotDeleted<OrganizationUnit>() &&
                       new FindSpecification<OrganizationUnit>(
                           ou =>
                           ou.BranchOfficeOrganizationUnits.Any(
                               boou =>
                               boou.IsActive && !boou.IsDeleted &&
                               boou.Accounts.Any(
                                   a =>
                                   a.IsActive && !a.IsDeleted && a.LegalPerson.IsActive && !a.LegalPerson.IsDeleted &&
                                   (a.LegalPerson.LegalPersonProfiles.Any(
                                       lpp => lpp.IsMainProfile && lpp.IsActive && !lpp.IsDeleted && lpp.ModifiedOn >= _startDate)) ||
                                   a.LegalPerson.ModifiedOn >= _startDate)));
            }
        }

        protected override bool TryCreateRequest(OrganizationUnit modelEntity, out ExportLegalPersonsRequest request)
        {
            request = new ExportLegalPersonsRequest
                {
                    OrganizationUnitId = modelEntity.Id,
                    PeriodStart = _startDate
                };

            return true;
        }

        protected override IResponseAsserter<IntegrationResponse> ResponseAsserter
        {
            get { return new StreamResponseAsserter(); }
        }
    }
}
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Orders
{
    public sealed class GetOrderDestinationOrganizationUnitHandler : RequestHandler<GetOrderDestinationOrganizationUnitRequest, GetOrderDestinationOrganizationUnitResponse>
    {
        private readonly IFinder _finder;

        public GetOrderDestinationOrganizationUnitHandler(IFinder finder)
        {
            _finder = finder;
        }

        protected override GetOrderDestinationOrganizationUnitResponse Handle(GetOrderDestinationOrganizationUnitRequest request)
        {
            var response = new GetOrderDestinationOrganizationUnitResponse();
            if (request.FirmId == 0)
            {
                return response;
            }

            var orgUnitInfo = _finder.Find(Specs.Find.ById<Firm>(request.FirmId)).Select(f => new { Id = f.OrganizationUnitId, f.OrganizationUnit.Name }).FirstOrDefault();

            if (orgUnitInfo == null)
            {
                return response;
            }

            response.OrganizationUnitId   = orgUnitInfo.Id;
            response.OrganizationUnitName = orgUnitInfo.Name;

            return response;
        }
    }
}
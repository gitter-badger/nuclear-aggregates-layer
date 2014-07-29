using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Old
{
    public sealed class EditContributionTypeHandler : RequestHandler<EditRequest<ContributionType>, EmptyResponse>
    {
        private readonly IContributionTypeService _contributionTypeService;

        public EditContributionTypeHandler(IContributionTypeService contributionTypeService)
        {
            _contributionTypeService = contributionTypeService;
        }

        protected override EmptyResponse Handle(EditRequest<ContributionType> request)
        {
            _contributionTypeService.CreateOrUpdate(request.Entity);
            return Response.Empty;
        }
    }
}
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Territories;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;

using NuClear.Security.API.UserContext;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Old.Territories
{
    public class ValidateTerritoryAvailabilityHandlerTest : UseModelEntityHandlerTestBase<Territory, ValidateTerritoryAvailabilityRequest, EmptyResponse>
    {
        private readonly long _currentUserId;


        public ValidateTerritoryAvailabilityHandlerTest(IPublicService publicService,
                                                        IAppropriateEntityProvider<Territory> appropriateEntityProvider,
                                                        IUserContext userContext) : base(publicService, appropriateEntityProvider)
        {

            _currentUserId = userContext.Identity.Code;
        }

        protected override FindSpecification<Territory> ModelEntitySpec
        {
            get
            {
                return base.ModelEntitySpec &&
                       new FindSpecification<Territory>(t => t.UserTerritoriesOrganizationUnits.Any(ut => ut.Territory.IsActive && ut.UserId == _currentUserId));
            }
        }

        protected override bool TryCreateRequest(Territory modelEntity, out ValidateTerritoryAvailabilityRequest request)
        {
            request = new ValidateTerritoryAvailabilityRequest
                {
                    TerritoryId = modelEntity.Id
                };

            return true;
        }
    }
}
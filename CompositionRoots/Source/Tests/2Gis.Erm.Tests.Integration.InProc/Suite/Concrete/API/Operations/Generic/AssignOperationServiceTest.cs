using DoubleGis.Erm.BLCore.API.Operations.Generic.Assign;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using FluentAssertions;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Generic
{
    public sealed class AssignOperationServiceTest : UseModelEntityTestBase<LegalPerson>
    {
        private const long TargetUserId = 1;
        private readonly IAssignGenericEntityService<LegalPerson> _assignGenericEntityService;
        private readonly ISecurityServiceUserIdentifier _securityServiceUserIdentifier;

        public AssignOperationServiceTest(
            IAssignGenericEntityService<LegalPerson> assignGenericEntityService,
            ISecurityServiceUserIdentifier securityServiceUserIdentifier,
            IAppropriateEntityProvider<LegalPerson> appropriateEntityProvider)
            : base(appropriateEntityProvider)
        {
            _assignGenericEntityService = assignGenericEntityService;
            _securityServiceUserIdentifier = securityServiceUserIdentifier;
        }

        protected override FindSpecification<LegalPerson> ModelEntitySpec
        {
            get
            {
                var reserveUserCode = _securityServiceUserIdentifier.GetReserveUserIdentity().Code;

                return Specs.Find.ActiveAndNotDeleted<LegalPerson>() 
                       && Specs.Find.NotOwned<LegalPerson>(new[]{TargetUserId, reserveUserCode}); 
            }
        }

        protected override OrdinaryTestResult ExecuteWithModel(LegalPerson modelEntity)
        {
            return Result
                .When(_assignGenericEntityService.Assign(modelEntity.Id, TargetUserId, true, false))
                .Then(result => result.Should().BeNull());
        }
    }
}
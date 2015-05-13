using DoubleGis.Erm.BLCore.API.Operations.Generic.Qualify;
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
    public sealed class QualifyOperationServiceTest : UseModelEntityTestBase<Firm>
    {
        private readonly IQualifyGenericEntityService<Firm> _qualifyGenericEntityService;
        private readonly ISecurityServiceUserIdentifier _securityServiceUserIdentifier;

        public QualifyOperationServiceTest(
            IQualifyGenericEntityService<Firm> qualifyGenericEntityService,
            ISecurityServiceUserIdentifier securityServiceUserIdentifier,
            IAppropriateEntityProvider<Firm> appropriateEntityProvider)
            : base(appropriateEntityProvider)
        {
            _qualifyGenericEntityService = qualifyGenericEntityService;
            _securityServiceUserIdentifier = securityServiceUserIdentifier;
        }

        protected override FindSpecification<Firm> ModelEntitySpec
        {
            get
            {
                var reserveUserCode = _securityServiceUserIdentifier.GetReserveUserIdentity().Code;
                return Specs.Find.ActiveAndNotDeleted<Firm>() 
                    && Specs.Find.Owned<Firm>(reserveUserCode) 
                    && new FindSpecification<Firm>(x => x.ClientId == null);
            }
        }

        protected override OrdinaryTestResult ExecuteWithModel(Firm modelEntity)
        {
            const long TargetUserCode = 1;
            return Result
                .When(_qualifyGenericEntityService.Qualify(modelEntity.Id, TargetUserCode, null))
                .Then(qualified => qualified.Should().NotBeNull());
        }
    }
}
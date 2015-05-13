using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Disqualify;
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
    public sealed class DisqualifyOperationServiceTest : UseModelEntityTestBase<Firm>
    {
        private readonly IDisqualifyGenericEntityService<Firm> _disqualifyGenericEntityService;
        private readonly ISecurityServiceUserIdentifier _securityServiceUserIdentifier;

        public DisqualifyOperationServiceTest(
            IDisqualifyGenericEntityService<Firm> disqualifyGenericEntityService,
            ISecurityServiceUserIdentifier securityServiceUserIdentifier,
            IAppropriateEntityProvider<Firm> appropriateEntityProvider)
            : base(appropriateEntityProvider)
        {
            _disqualifyGenericEntityService = disqualifyGenericEntityService;
            _securityServiceUserIdentifier = securityServiceUserIdentifier;
        }

        protected override FindSpecification<Firm> ModelEntitySpec
        {
            get
            {
                var reserveUserCode = _securityServiceUserIdentifier.GetReserveUserIdentity().Code;
                return Specs.Find.ActiveAndNotDeleted<Firm>()
                        && Specs.Find.NotOwned<Firm>(reserveUserCode)
                        && FirmSpecs.Firms.Find.WithoutActiveOrders();
            }
        }

        protected override OrdinaryTestResult ExecuteWithModel(Firm modelEntity)
        {
            return Result
                .When(_disqualifyGenericEntityService.Disqualify(modelEntity.Id, true))
                .Then(disqualified => disqualified.Should().BeNull());
        }
    }
}
using DoubleGis.Erm.BLCore.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Get;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using FluentAssertions;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Generic
{
    public sealed class ModifyOperationServiceTest : UseModelEntityTestBase<LegalPerson>
    {
        private readonly IGetDomainEntityDtoService<LegalPerson> _getDomainEntityDtoService;
        private readonly IModifyBusinessModelEntityService<LegalPerson> _modifyEntityService;
        private readonly ISecurityServiceUserIdentifier _securityServiceUserIdentifier;

        public ModifyOperationServiceTest(
            IGetDomainEntityDtoService<LegalPerson> getDomainEntityDtoService,
            IModifyBusinessModelEntityService<LegalPerson> modifyEntityService,
            ISecurityServiceUserIdentifier securityServiceUserIdentifier,
            IAppropriateEntityProvider<LegalPerson> appropriateEntityProvider) 
            : base(appropriateEntityProvider)
        {
            _getDomainEntityDtoService = getDomainEntityDtoService;
            _modifyEntityService = modifyEntityService;
            _securityServiceUserIdentifier = securityServiceUserIdentifier;
        }

        protected override FindSpecification<LegalPerson> ModelEntitySpec
        {
            get
            {
                var reserveUserCode = _securityServiceUserIdentifier.GetReserveUserIdentity().Code;
                return Specs.Find.ActiveAndNotDeleted<LegalPerson>()
                       && Specs.Find.NotOwned<LegalPerson>(reserveUserCode)
                       && LegalPersonSpecs.LegalPersons.Find.OfType(LegalPersonType.LegalPerson)
                       && LegalPersonSpecs.LegalPersons.Find.InSyncWith1C();
            }
        }

        protected override OrdinaryTestResult ExecuteWithModel(LegalPerson modelEntity)
        {
            var domainEntityDto = (LegalPersonDomainEntityDto)_getDomainEntityDtoService.GetDomainEntityDto(modelEntity.Id, false, null, EntityType.Instance.None(), null);

            domainEntityDto.ShortName = "test";

            return Result
                .When(_modifyEntityService.Modify(domainEntityDto))
                .Then(result => result.Should().BeGreaterThan(0));
        }
    }
}
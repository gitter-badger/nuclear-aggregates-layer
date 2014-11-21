using DoubleGis.Erm.BLCore.API.Operations.Generic.Get;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using FluentAssertions;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Generic
{
    public sealed class GetDomainEntityDtoOperationServiceTest : UseModelEntityTestBase<Client>
    {
        private readonly IGetDomainEntityDtoService<Client> _getDomainEntityDtoService;

        public GetDomainEntityDtoOperationServiceTest(
            IGetDomainEntityDtoService<Client> getDomainEntityDtoService,
            IAppropriateEntityProvider<Client> appropriateEntityProvider) : 
            base(appropriateEntityProvider)
        {
            _getDomainEntityDtoService = getDomainEntityDtoService;
        }

        protected override OrdinaryTestResult ExecuteWithModel(Client modelEntity)
        {
            return Result
                .When(_getDomainEntityDtoService.GetDomainEntityDto(modelEntity.Id, false, null, EntityName.None, null))
                .Then(dto => dto.Id.Should().Be(modelEntity.Id));
        }
    }
}
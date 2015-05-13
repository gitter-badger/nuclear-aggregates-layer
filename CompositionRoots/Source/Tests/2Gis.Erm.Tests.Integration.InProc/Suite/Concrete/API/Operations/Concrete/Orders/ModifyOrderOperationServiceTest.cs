using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Get;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using FluentAssertions;

using NuClear.Model.Common.Entities;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Orders
{
    public class ModifyOrderOperationServiceTest : UseModelEntityTestBase<Order>
    {
        private readonly IGetDomainEntityDtoService<Order> _getDomainEntityDtoService;
        private readonly IModifyBusinessModelEntityService<Order> _modifyEntityService;

        public ModifyOrderOperationServiceTest(IAppropriateEntityProvider<Order> appropriateEntityProvider,
                                               IGetDomainEntityDtoService<Order> getDomainEntityDtoService,
                                               IModifyBusinessModelEntityService<Order> modifyEntityService)
            : base(appropriateEntityProvider)
        {
            _getDomainEntityDtoService = getDomainEntityDtoService;
            _modifyEntityService = modifyEntityService;
        }

        protected override FindSpecification<Order> ModelEntitySpec
        {
            get
            {
                return Specs.Find.ActiveAndNotDeleted<Order>() &&
                       OrderSpecs.Orders.Find.NotInArchive() &&
                       OrderSpecs.Orders.Find.NotRejected();
            }
        }

        protected override OrdinaryTestResult ExecuteWithModel(Order modelEntity)
        {
            var domainEntityDto = (OrderDomainEntityDto)_getDomainEntityDtoService.GetDomainEntityDto(modelEntity.Id, false, null, EntityType.Instance.None(), null);

            domainEntityDto.EndDistributionDatePlan = domainEntityDto.EndDistributionDatePlan.AddMonths(1);

            return Result
                .When(_modifyEntityService.Modify(domainEntityDto))
                .Then(result => result.Should().BeGreaterThan(0));
        }
    }
}
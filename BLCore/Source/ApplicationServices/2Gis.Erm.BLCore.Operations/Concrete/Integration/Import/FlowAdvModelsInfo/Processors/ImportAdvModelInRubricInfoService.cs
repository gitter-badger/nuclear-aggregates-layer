using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.Categories;
using DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.Categories.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.AdvModelsInfo;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Shared;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Charge;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Import.FlowAdvModelsInfo.Processors
{
    public class ImportAdvModelInRubricInfoService : IImportAdvModelInRubricInfoService
    {
        private readonly ISalesModelCategoryRestrictionReadModel _restrictionReadModel;
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IBulkCreateSalesModelCategoryRestrictionsService _bulkCreateSalesModelCategoryRestrictionsService;
        private readonly IBulkDeleteSalesModelCategoryRestrictionsService _bulkDeleteSalesModelCategoryRestrictionsService;
        private readonly IRegisterOrderStateChangesOperationService _registerOrderStateChangesOperationService;

        public ImportAdvModelInRubricInfoService(ISalesModelCategoryRestrictionReadModel restrictionReadModel,
                                                 IOperationScopeFactory operationScopeFactory,
                                                 IBulkCreateSalesModelCategoryRestrictionsService bulkCreateSalesModelCategoryRestrictionsService,
                                                 IBulkDeleteSalesModelCategoryRestrictionsService bulkDeleteSalesModelCategoryRestrictionsService,
                                                 IRegisterOrderStateChangesOperationService registerOrderStateChangesOperationService)
        {
            _restrictionReadModel = restrictionReadModel;
            _operationScopeFactory = operationScopeFactory;
            _bulkCreateSalesModelCategoryRestrictionsService = bulkCreateSalesModelCategoryRestrictionsService;
            _bulkDeleteSalesModelCategoryRestrictionsService = bulkDeleteSalesModelCategoryRestrictionsService;
            _registerOrderStateChangesOperationService = registerOrderStateChangesOperationService;
        }

        public void Import(IEnumerable<IServiceBusDto> dtos)
        {
            var serviceBusDtos = dtos.Cast<AdvModelInRubricInfoServiceBusDto>().ToArray();
            using (var scope = _operationScopeFactory.CreateNonCoupled<ImportAdvModelInRubricInfoIdentity>())
            {
                foreach (var serviceBusDto in serviceBusDtos)
                {
                    var oldRestrictions = _restrictionReadModel.GetRestrictionsByProject(serviceBusDto.BranchCode);
                    _bulkDeleteSalesModelCategoryRestrictionsService.Delete(oldRestrictions);
                }

                foreach (var serviceBusDto in serviceBusDtos)
                {
                    var newRestrictions = serviceBusDto.AdvModelInRubrics
                                                       .Select(advModelInRubricDto => new SalesModelCategoryRestriction
                                                                                          {
                                                                                              CategoryId = advModelInRubricDto.RubricCode,
                                                                                              ProjectId = serviceBusDto.BranchCode,
                                                                                              SalesModel = advModelInRubricDto.AdvModel.ConvertToSalesModel()
                                                                                          })
                                                       .ToArray();

                    _bulkCreateSalesModelCategoryRestrictionsService.Create(newRestrictions);
                    scope.Added(newRestrictions.AsEnumerable());
                }

                foreach (var serviceBusDto in serviceBusDtos)
                {
                    var orderIds = _restrictionReadModel.GetDependedByRestrictionsInProjectOrderIds(serviceBusDto.BranchCode);
                    _registerOrderStateChangesOperationService.Changed(orderIds.Select(x => new OrderChangesDescriptor
                                                                                                {
                                                                                                    OrderId = x,
                                                                                                    ChangedAspects =
                                                                                                        new[]
                                                                                                            {
                                                                                                                OrderValidationRuleGroup.SalesModelValidation
                                                                                                            }
                                                                                                }));
                }

                scope.Complete();
            }
        }
    }
}
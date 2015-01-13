﻿using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.Categories;
using DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.Categories.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Billing;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Charge;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Import.FlowBilling.Processors
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
            using (var scope = _operationScopeFactory.CreateNonCoupled<ImportAdvModelInRubricInfoIdentity>())
            {
                foreach (var dto in dtos)
                {
                    var serviceBusDto = (AdvModelInRubricInfoServiceBusDto)dto;
                    var oldRestrictions = _restrictionReadModel.GetRestrictionsByProject(serviceBusDto.BranchCode);
                    _bulkDeleteSalesModelCategoryRestrictionsService.Delete(oldRestrictions.ToArray());
                    scope.Deleted(oldRestrictions);

                    var newRestrictions = serviceBusDto.AdvModelInRubrics
                                                       .Select(advModelInRubricDto => new SalesModelCategoryRestriction
                                                                                          {
                                                                                              CategoryId = advModelInRubricDto.RubricCode,
                                                                                              ProjectId = serviceBusDto.BranchCode,
                                                                                              SalesModel = ConvertAdvModelToSalesModel(advModelInRubricDto.AdvModel)
                                                                                          }).ToList();

                    _bulkCreateSalesModelCategoryRestrictionsService.Create(newRestrictions);
                    scope.Added(newRestrictions.AsEnumerable());

                    var orderIds = _restrictionReadModel.GetDependedByRestrictionsInProjectOrderIds(serviceBusDto.BranchCode);

                    _registerOrderStateChangesOperationService.Changed(orderIds.Select(x => new OrderChangesDescriptor
                                                                                                {
                                                                                                    OrderId = x,
                                                                                                    ChangedAspects =
                                                                                                        new[]
                                                                                                            {
                                                                                                                OrderValidationRuleGroup
                                                                                                                    .SalesModelValidation
                                                                                                            }
                                                                                                }));
                }

                scope.Complete();
            }
        }

        private SalesModel ConvertAdvModelToSalesModel(AdvModel advModel)
        {
            switch (advModel)
            {
                case AdvModel.Cps:
                    return SalesModel.GuaranteedProvision;                    
                case AdvModel.Fh:
                    return SalesModel.PlannedProvision;
                case AdvModel.Mfh:
                    return SalesModel.MultiPlannedProvision;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
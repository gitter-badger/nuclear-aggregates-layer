﻿using System.Linq;

using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public class GetBillDtoService : GetDomainEntityDtoServiceBase<Bill>
    {
        private readonly ISecureFinder _finder;

        public GetBillDtoService(IUserContext userContext, ISecureFinder finder) : base(userContext)
        {
            _finder = finder;
        }

        protected override IDomainEntityDto<Bill> GetDto(long entityId)
        {
            return _finder.Find<Bill>(x => x.Id == entityId)
                          .Select(x => new BillDomainEntityDto
                              {
                                  Id = x.Id,
                                  OrderId = x.OrderId,
                                  BillNumber = x.BillNumber,
                                  BillDate = x.BillDate,
                                  BeginDistributionDate = x.BeginDistributionDate,
                                  EndDistributionDate = x.EndDistributionDate,
                                  PaymentDatePlan = x.PaymentDatePlan,
                                  PayablePlan = x.PayablePlan,
                                  Comment = x.Comment,
                                  IsOrderActive = x.Order.WorkflowStepId == OrderState.OnRegistration,
                                  OwnerRef = new EntityReference { Id = x.OwnerCode },
                                  CreatedByRef = new EntityReference { Id = x.CreatedBy },
                                  CreatedOn = x.CreatedOn,
                                  IsActive = x.IsActive,
                                  IsDeleted = x.IsDeleted,
                                  ModifiedByRef = new EntityReference { Id = x.ModifiedBy },
                                  ModifiedOn = x.ModifiedOn,
                                  Timestamp = x.Timestamp
                              })
                          .Single();
        }

        protected override IDomainEntityDto<Bill> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            return new BillDomainEntityDto();
        }
    }
}
﻿using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public class GetDealDtoService : GetDomainEntityDtoServiceBase<Deal>
    {
        private readonly ISecureFinder _finder;
        private readonly IUserContext _userContext;
        private readonly IGetBaseCurrencyService _currencyService;

        public GetDealDtoService(IUserContext userContext, ISecureFinder finder, IGetBaseCurrencyService currencyService)
            : base(userContext)
        {
            _finder = finder;
            _userContext = userContext;
            _currencyService = currencyService;
        }

        protected override IDomainEntityDto<Deal> GetDto(long entityId)
        {
            var modelDto = _finder.Find<Deal>(x => x.Id == entityId)
                                  .Select(entity => new DealDomainEntityDto
                                      {
                                          Id = entity.Id,
                                          ReplicationCode = entity.ReplicationCode,
                                          Name = entity.Name,
                                          ClientReplicationCode = entity.Client.ReplicationCode,
                                          ActualProfit = entity.ActualProfit,
                                          CloseDate = entity.CloseDate,
                                          CloseReasonOther = entity.CloseReasonOther,
                                          Comment = entity.Comment,
                                          CreatedOn = entity.CreatedOn,
                                          EstimatedProfit = entity.EstimatedProfit,
                                          CloseReason = (CloseDealReason)entity.CloseReason,
                                          DealStage = (DealStage)entity.DealStage,
                                          StartReason = (ReasonForNewDeal)entity.StartReason,
                                          MainFirmRef = new EntityReference { Id = entity.MainFirmId, Name = entity.Firm.Name },
                                          ClientRef = new EntityReference { Id = entity.ClientId, Name = entity.Client.Name },
                                          CurrencyRef = new EntityReference { Id = entity.CurrencyId, Name = entity.Currency.Name },
                                          OwnerRef = new EntityReference { Id = entity.OwnerCode, Name = null },
                                          Timestamp = entity.Timestamp,
                                          IsActive = entity.IsActive,
                                          IsDeleted = entity.IsDeleted,
                                          CreatedByRef = new EntityReference { Id = entity.CreatedBy, Name = null },
                                          ModifiedByRef = new EntityReference { Id = entity.ModifiedBy, Name = null },
                                          ModifiedOn = entity.ModifiedOn
                                      })
                                  .Single();

            return modelDto;
        }

        protected override IDomainEntityDto<Deal> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            var currencyId = _currencyService.GetBaseCurrency().Id;

            var dto = parentEntityName == EntityName.Client
                          ? _finder.Find<Client>(x => x.Id == parentEntityId)
                                   .Select(x => new DealDomainEntityDto
                                       {
                                           ClientRef = new EntityReference { Id = x.Id, Name = x.Name },
                                           MainFirmRef = new EntityReference { Id = x.MainFirmId, Name = x.Firm.Name },
                                           OwnerRef = new EntityReference { Id = x.OwnerCode, Name = null },
                                       })
                                   .Single()
                          : new DealDomainEntityDto
                              {
                                  OwnerRef = new EntityReference { Id = _userContext.Identity.Code },
                              };

            dto.CurrencyRef = new EntityReference(currencyId);
            dto.DealStage = DealStage.CollectInformation;

            return dto;
        }
    }
}
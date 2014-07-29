using System;
using System.Linq;

using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public class GetFirmDtoService : GetDomainEntityDtoServiceBase<Firm>
    {
        private readonly ISecureFinder _finder;

        public GetFirmDtoService(IUserContext userContext, ISecureFinder finder) : base(userContext)
        {
            _finder = finder;
        }

        protected override IDomainEntityDto<Firm> GetDto(long entityId)
        {
            var modelDto = _finder.Find<Firm>(x => x.Id == entityId)
                                  .Select(entity => new FirmDomainEntityDto
                                      {
                                          Id = entity.Id,
                                          Name = entity.Name,
                                          ClosedForAscertainment = entity.ClosedForAscertainment,
                                          ProductType = (ProductType)entity.ProductType,
                                          MarketType = (MarketType)entity.MarketType,
                                          UsingOtherMedia = (UsingOtherMediaOption)entity.UsingOtherMedia,
                                          BudgetType = (BudgetType)entity.BudgetType,
                                          Geolocation = (Geolocation)entity.Geolocation,
                                          InCityBranchesAmount = (InCityBranchesAmount)entity.InCityBranchesAmount,
                                          OutCityBranchesAmount = (OutCityBranchesAmount)entity.OutCityBranchesAmount,
                                          StaffAmount = (StaffAmount)entity.StaffAmount,
                                          PromisingScore = entity.PromisingScore,
                                          ReplicationCode = entity.ReplicationCode,
                                          Comment = entity.Comment,
                                          LastQualifyTime = entity.LastQualifyTime,
                                          LastDisqualifyTime = entity.LastDisqualifyTime,
                                          ClientRef = new EntityReference { Id = entity.ClientId, Name = entity.Client != null ? entity.Client.Name : null },
                                          ClientReplicationCode = entity.Client != null ? (Guid?)entity.Client.ReplicationCode : null,
                                          TerritoryRef = new EntityReference { Id = entity.TerritoryId, Name = entity.Territory.Name },
                                          OrganizationUnitRef = new EntityReference { Id = entity.OrganizationUnitId, Name = entity.OrganizationUnit.Name },
                                          OwnerRef = new EntityReference { Id = entity.OwnerCode, Name = null },
                                          CreatedByRef = new EntityReference { Id = entity.CreatedBy, Name = null },
                                          CreatedOn = entity.CreatedOn,
                                          IsActive = entity.IsActive,
                                          IsDeleted = entity.IsDeleted,
                                          ModifiedByRef = new EntityReference { Id = entity.ModifiedBy, Name = null },
                                          ModifiedOn = entity.ModifiedOn,
                                          Timestamp = entity.Timestamp
                                      })
                                  .Single();

            return modelDto;
        }

        protected override IDomainEntityDto<Firm> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            return new FirmDomainEntityDto();
        }
    }
}
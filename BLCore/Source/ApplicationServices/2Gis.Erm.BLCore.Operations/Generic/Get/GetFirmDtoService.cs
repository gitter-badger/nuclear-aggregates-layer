using System;
using System.Linq;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Security.API.UserContext;
using NuClear.Storage.Specifications;

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
            var modelDto = _finder.Find(new FindSpecification<Firm>(x => x.Id == entityId))
                                  .Select(entity => new FirmDomainEntityDto
                                      {
                                          Id = entity.Id,
                                          Name = entity.Name,
                                          ClosedForAscertainment = entity.ClosedForAscertainment,
                                          ProductType = entity.ProductType,
                                          MarketType = entity.MarketType,
                                          UsingOtherMedia = entity.UsingOtherMedia,
                                          BudgetType = entity.BudgetType,
                                          Geolocation = entity.Geolocation,
                                          InCityBranchesAmount = entity.InCityBranchesAmount,
                                          OutCityBranchesAmount = entity.OutCityBranchesAmount,
                                          StaffAmount = entity.StaffAmount,
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

        protected override IDomainEntityDto<Firm> CreateDto(long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {
            throw new NotificationException(BLResources.FirmCreationIsNotSupported);
        }
    }
}
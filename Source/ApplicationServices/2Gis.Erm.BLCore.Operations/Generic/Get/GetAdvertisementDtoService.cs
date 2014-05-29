﻿using System.Linq;
using System.Text.RegularExpressions;

using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public class GetAdvertisementDtoService : GetDomainEntityDtoServiceBase<Advertisement>
    {
        private readonly IUserContext _userContext;
        private readonly ISecureFinder _finder;
        private readonly IFinder _unsecureFinder;
        private readonly ISecurityServiceEntityAccessInternal _securityServiceEntityAccess;

        public GetAdvertisementDtoService(IUserContext userContext,
                                          ISecureFinder finder,
                                          IFinder unsecureFinder,
                                          ISecurityServiceEntityAccessInternal securityServiceEntityAccess)
            : base(userContext)
        {
            _userContext = userContext;
            _finder = finder;
            _unsecureFinder = unsecureFinder;
            _securityServiceEntityAccess = securityServiceEntityAccess;
        }

        protected override IDomainEntityDto<Advertisement> GetDto(long entityId)
        {
            var dto = _finder.Find<Advertisement>(x => x.Id == entityId)
                             .Select(entity => new AdvertisementDomainEntityDto
                                 {
                                     Id = entity.Id,
                                     Name = entity.Name,
                                     Comment = entity.Comment,
                                     IsSelectedToWhiteList = entity.IsSelectedToWhiteList,
                                     FirmRef = new EntityReference { Id = entity.Firm.Id, Name = entity.Firm.Name },
                                     AdvertisementTemplateRef = new EntityReference { Id = entity.AdvertisementTemplate.Id, Name = entity.AdvertisementTemplate.Name },
                                     HasAssignedOrder = entity.OrderPositionAdvertisements.Select(opa => opa.OrderPosition)
                                                              .Where(op => op.IsDeleted == false)
                                                              .Select(op => op.Order)
                                                              .Any(x => x.IsDeleted == false),

                                     IsReadOnlyTemplate = entity.AdvertisementElements.Any(y => !y.IsDeleted &&
                                                  (y.BeginDate.HasValue || y.EndDate.HasValue || y.Text != string.Empty || y.FileId.HasValue))
                                                  || entity.OrderPositionAdvertisements.All(y => y.Position.AdvertisementTemplateId == entity.AdvertisementTemplateId),

                                     OwnerRef = new EntityReference { Id = entity.OwnerCode, Name = null },
                                     CreatedByRef = new EntityReference { Id = entity.CreatedBy, Name = null },
                                     CreatedOn = entity.CreatedOn,
                                     IsDeleted = entity.IsDeleted,
                                     ModifiedByRef = new EntityReference { Id = entity.ModifiedBy, Name = null },
                                     ModifiedOn = entity.ModifiedOn,
                                     Timestamp = entity.Timestamp
                                 })
                             .Single();

            if (dto.FirmRef.Id.HasValue)
            {
                var firmInfo = GetFirm(dto.FirmRef.Id.Value);

                dto.UserDoesntHaveRightsToEditFirm = !_securityServiceEntityAccess.HasEntityAccess(EntityAccessTypes.Update,
                                                                                                   EntityName.Firm,
                                                                                                   _userContext.Identity.Code,
                                                                                                   firmInfo.Id,
                                                                                                   firmInfo.OwnerCode,
                                                                                                   firmInfo.OwnerCode);
            }

            return dto;
        }

        protected override IDomainEntityDto<Advertisement> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            var dto = new AdvertisementDomainEntityDto();
            dto.UserDoesntHaveRightsToEditFirm = false;

            switch (parentEntityName)
            {
                case EntityName.Firm:
                {
                    var firmInfo = GetFirm(parentEntityId.Value);
                    dto.FirmRef = new EntityReference
                        {
                            Id = parentEntityId.Value,
                            Name = firmInfo.Name
                        };

                    dto.UserDoesntHaveRightsToEditFirm = !_securityServiceEntityAccess.HasEntityAccess(EntityAccessTypes.Update,
                                                                                                   EntityName.Firm,
                                                                                                   _userContext.Identity.Code,
                                                                                                   firmInfo.Id,
                                                                                                   firmInfo.OwnerCode,
                                                                                                   firmInfo.OwnerCode);

                    break;
                }

                case EntityName.OrderPosition:
                {
                    long firmId;
                    if (!string.IsNullOrEmpty(extendedInfo) &&
                        long.TryParse(Regex.Match(extendedInfo, @"FirmId=(\d+)").Groups[1].Value, out firmId))
                    {
                        var firmInfo = GetFirm(firmId);
                        dto.FirmRef = new EntityReference
                            {
                                Id = firmId,
                                Name = firmInfo.Name
                            };

                        dto.UserDoesntHaveRightsToEditFirm = !_securityServiceEntityAccess.HasEntityAccess(EntityAccessTypes.Update,
                                                                                                   EntityName.Firm,
                                                                                                   _userContext.Identity.Code,
                                                                                                   firmInfo.Id,
                                                                                                   firmInfo.OwnerCode,
                                                                                                   firmInfo.OwnerCode);
                    }

                    long advertisementTemplateId;
                    if (!string.IsNullOrEmpty(extendedInfo) &&
                        long.TryParse(Regex.Match(extendedInfo, @"AdvertisementTemplateId=(\d+)").Groups[1].Value, out advertisementTemplateId))
                    {
                        dto.AdvertisementTemplateRef = new EntityReference
                            {
                                Id = advertisementTemplateId,
                                Name = _finder.Find<AdvertisementTemplate>(x => x.Id == advertisementTemplateId).Select(x => x.Name).Single()
                            };
                    }

                    break;
                }
                    
                case EntityName.None:
                {
                    long firmId;
                    if (!string.IsNullOrEmpty(extendedInfo) &&
                        long.TryParse(Regex.Match(extendedInfo, @"FirmId=(\d+)", RegexOptions.IgnoreCase).Groups[1].Value, out firmId))
                    {
                        dto.FirmRef = new EntityReference
                            {
                                Id = firmId,
                                Name = _finder.Find<Firm>(x => x.Id == firmId).Select(x => x.Name).Single()
                            };
                    }

                    break;
                }
            }

            return dto;
        }

        protected override void SetDtoProperties(IDomainEntityDto<Advertisement> domainEntityDto, long entityId, bool readOnly, long? parentEntityId, EntityName parentEntityName, string extendedInfo, long currentUserCode)
        {
            var dto = (AdvertisementDomainEntityDto)domainEntityDto;

            if (parentEntityName == EntityName.OrderPosition && dto.AdvertisementTemplateRef.Id.HasValue)
            {
                dto.IsReadOnlyTemplate = true;
            }
        }

        private FirmInfo GetFirm(long firmId)
        {
            return _unsecureFinder
                .Find(Specs.Find.ById<Firm>(firmId))
                .Select(x => new FirmInfo
                    {
                        Id = x.Id,
                        OwnerCode = x.OwnerCode,
                        Name = x.Name
                    })
                .Single();
        }

        private class FirmInfo
        {
            public long Id { get; set; }
            public long OwnerCode { get; set; }
            public string Name { get; set; }
        }
    }
}
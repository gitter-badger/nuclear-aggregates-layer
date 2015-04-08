using System.Linq;
using System.Text.RegularExpressions;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public class GetAdvertisementDtoService : GetDomainEntityDtoServiceBase<Advertisement>
    {
        private readonly ISecureFinder _secureFinder;
        private readonly IFirmReadModel _firmReadModel;

        public GetAdvertisementDtoService(IUserContext userContext,
                                          ISecureFinder secureFinder,
                                          IFirmReadModel firmReadModel)
            : base(userContext)
        {
            _secureFinder = secureFinder;
            _firmReadModel = firmReadModel;
        }

        protected override IDomainEntityDto<Advertisement> GetDto(long entityId)
        {
            var dto = _secureFinder.Find<Advertisement>(x => x.Id == entityId)
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

            return dto;
        }

        protected override IDomainEntityDto<Advertisement> CreateDto(long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {
            var dto = new AdvertisementDomainEntityDto();            

            if (parentEntityName.Equals(EntityType.Instance.Firm()))
            {
                var firmName = _firmReadModel.GetFirmName(parentEntityId.Value);
                dto.FirmRef = new EntityReference
                {
                    Id = parentEntityId.Value,
                    Name = firmName
                };

            }

            if (parentEntityName.Equals(EntityType.Instance.OrderPosition()))
            {
                long firmId;
                if (!string.IsNullOrEmpty(extendedInfo) &&
                    long.TryParse(Regex.Match(extendedInfo, @"FirmId=(\d+)").Groups[1].Value, out firmId))
                {
                    var firmName = _firmReadModel.GetFirmName(firmId);
                    dto.FirmRef = new EntityReference
                        {
                            Id = firmId,
                            Name = firmName
                        };
                }

                long advertisementTemplateId;
                if (!string.IsNullOrEmpty(extendedInfo) &&
                    long.TryParse(Regex.Match(extendedInfo, @"AdvertisementTemplateId=(\d+)").Groups[1].Value, out advertisementTemplateId))
                {
                    dto.AdvertisementTemplateRef = new EntityReference
                        {
                            Id = advertisementTemplateId,
                            Name = _secureFinder.Find<AdvertisementTemplate>(x => x.Id == advertisementTemplateId).Select(x => x.Name).Single()
                        };
                }
            }
                    
            if (parentEntityName.Equals(EntityType.Instance.None()))
            {
                long firmId;
                if (!string.IsNullOrEmpty(extendedInfo) &&
                    long.TryParse(Regex.Match(extendedInfo, @"FirmId=(\d+)", RegexOptions.IgnoreCase).Groups[1].Value, out firmId))
                {
                    dto.FirmRef = new EntityReference
                        {
                            Id = firmId,
                            Name = _secureFinder.Find<Firm>(x => x.Id == firmId).Select(x => x.Name).Single()
                        };
                }
            }

            return dto;
        }

        protected override void SetDtoProperties(IDomainEntityDto<Advertisement> domainEntityDto,
            long entityId, 
            bool readOnly, 
            long? parentEntityId, 
                                                 IEntityType parentEntityName,
            string extendedInfo)
        {
            var dto = (AdvertisementDomainEntityDto)domainEntityDto;

            if (parentEntityName.Equals(EntityType.Instance.OrderPosition()) && dto.AdvertisementTemplateRef.Id.HasValue)
            {
                dto.IsReadOnlyTemplate = true;
            }
        }
    }
}
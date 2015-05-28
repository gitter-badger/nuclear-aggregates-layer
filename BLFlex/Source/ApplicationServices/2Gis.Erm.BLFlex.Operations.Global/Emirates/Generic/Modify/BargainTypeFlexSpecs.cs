using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Emirates;
using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Emirates.Generic.Modify
{
    public class BargainTypeFlexSpecs
    {
        public static class BargainTypes
        {
            public static class Emirates
            {
                public static class Project
                {
                    public static MapSpecification<BargainType, EmiratesBargainTypeDomainEntityDto> DomainEntityDto()
                    {
                        return new MapSpecification<BargainType, EmiratesBargainTypeDomainEntityDto>(
                            x =>
                                {
                                    var dto = new EmiratesBargainTypeDomainEntityDto
                                        {
                                            Id = x.Id,
                                            Name = x.Name,
                                            Timestamp = x.Timestamp,
                                            CreatedByRef = new EntityReference { Id = x.CreatedBy },
                                            CreatedOn = x.CreatedOn,
                                            IsActive = x.IsActive,
                                            IsDeleted = x.IsDeleted,
                                            ModifiedByRef = new EntityReference { Id = x.ModifiedBy },
                                            ModifiedOn = x.ModifiedOn
                                        };

                                    return dto;
                                });
                    }
                }

                public static class Assign
                {
                    public static IAssignSpecification<EmiratesBargainTypeDomainEntityDto, BargainType> Entity()
                    {
                        return new AssignSpecification<EmiratesBargainTypeDomainEntityDto, BargainType>(
                            (dto, entity) =>
                                {
                                    entity.Name = dto.Name;
                                    entity.Timestamp = dto.Timestamp;
                                    entity.VatRate = decimal.Zero;
                                });
                    }
                }
            }
        }
    }
}